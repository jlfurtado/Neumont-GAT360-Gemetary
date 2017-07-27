using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer))]
public class PlayerController : MonoBehaviour {
    public float Speed;
    public Material[] Colors;
    public Material DefaultMat;
    public Material DodgeMat;
    public float PowerupTime;
    public float DodgeTime;
    public float DodgeCooldown;
    private MazeScript mazeRef;
    private const float CLOSE_ENOUGH = 0.1f;
    private const float PAST = 0.01f;
    private Enemy[] enemies;

    public bool PoweredUp { get; private set; }
    public bool Dodging { get; private set; }
    private float remainingPowerTime;
    private Rigidbody myRigidBody = null;
    private Renderer myRenderer = null;
    private bool moving = false;
    private Vector3 fromPos, toPos;
    private bool horizLast;
    private float dodgeTime;
    private int endurance;
    private const int MAX_ENDURANCE = 100;
    private const int ENDURANCE_COST = 50;

    // Use this for initialization
    void Awake() {
        myRigidBody = GetComponent<Rigidbody>();
        myRenderer = GetComponent<Renderer>();
        mazeRef = GameObject.FindGameObjectWithTag(Strings.MAZE_TAG).GetComponent<MazeScript>();

        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag(Strings.ENEMY_TAG);

        enemies = new Enemy[enemyObjects.Length];
        for (int i = 0; i < enemyObjects.Length; ++i)
        {
            enemies[i] = enemyObjects[i].GetComponent<Enemy>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        int h = Mathf.RoundToInt(horiz);
        int v = Mathf.RoundToInt(vert);

        IVec2 dir = new IVec2(h, v);

        if (!moving && (dir.x != 0 || dir.z != 0))
        {
            Vector3 toHoriz = new Vector3(myRigidBody.position.x + dir.x, myRigidBody.position.y, myRigidBody.position.z);
            Vector3 toVert = new Vector3(myRigidBody.position.x, myRigidBody.position.y, myRigidBody.position.z + dir.z);

            MazeSectionGenerator toSectionH = mazeRef.SectionAt(toHoriz), toSectionV = mazeRef.SectionAt(toVert);
            MazeSectionGenerator fromSection = mazeRef.SectionAt(myRigidBody.position);
            IVec2 fromLoc = mazeRef.SectionLocFor(myRigidBody.position);
            IVec2 toLocH = mazeRef.SectionLocFor(toHoriz), toLocV = mazeRef.SectionLocFor(toVert);

            Vector3 tph = toSectionH.PositionAt(toLocH), tpv = toSectionV.PositionAt(toLocV), fp = fromSection.PositionAt(fromLoc);
            Vector3 fromPos = new Vector3(fp.x, myRigidBody.position.y, fp.z);

            Vector3 toPos = fromPos;
            if (horizLast)
            {
                if (!toSectionH.IsWall(toLocH) && dir.x != 0) { toPos = new Vector3(tph.x, myRigidBody.position.y, tph.z); horizLast = true; }
                if (!toSectionV.IsWall(toLocV) && dir.z != 0) { toPos = new Vector3(tpv.x, myRigidBody.position.y, tpv.z); horizLast = false; }
            }
            else
            {
                if (!toSectionV.IsWall(toLocV) && dir.z != 0) { toPos = new Vector3(tpv.x, myRigidBody.position.y, tpv.z); horizLast = false; }
                if (!toSectionH.IsWall(toLocH) && dir.x != 0) { toPos = new Vector3(tph.x, myRigidBody.position.y, tph.z); horizLast = true; }
            }            

            Vector3 vel = toPos - fromPos;
            myRigidBody.velocity = vel.normalized * Speed;

            moving = true;
            this.fromPos = fromPos;
            this.toPos = toPos;
        }
        
        if (moving && Vector3.Dot((toPos - myRigidBody.position).normalized, ((toPos - fromPos).normalized)) < PAST || (toPos - myRigidBody.position).magnitude < CLOSE_ENOUGH)
        {
            // do something on land!?!?!
            myRigidBody.velocity = Vector3.zero;
            myRigidBody.position = toPos;
            moving = false;
        }

        if (PoweredUp)
        {
            RaycastHit hit;
            if (Physics.Raycast(myRigidBody.position, new Vector3(dir.x, 0.0f, dir.z), out hit, 0.5f))
            {
                mazeRef.EatAt(hit.transform.position);
                hit.transform.gameObject.SetActive(false);
            }

            myRenderer.material = Colors[((int)Mathf.Floor(Mathf.Sqrt(remainingPowerTime) * 250)) % Colors.Length];
            remainingPowerTime -= Time.deltaTime;
            if (remainingPowerTime <= 0.0f) { Restore(); }
        }
        else if (Dodging)
        {
            dodgeTime -= Time.deltaTime;
            if (dodgeTime <= 0.0f) { EndDodge(); }
        }
        else 
        {
            if (endurance >= ENDURANCE_COST && Mathf.RoundToInt(Input.GetAxis("Jump")) > 0)
            {
                Dodge();
            }
        }

        mazeRef.GenerateAround(myRigidBody.position);
    }

    public float GetEndurancePercent()
    {
        return endurance / (1.0f * MAX_ENDURANCE);
    }

    public void PowerUp()
    {
        EndDodge();
        PoweredUp = true;
        remainingPowerTime = PowerupTime;

        foreach (Enemy enemy in enemies)
        {
            enemy.StopFor(PowerupTime);
        }

    }

    public void AddEndurance(int amount)
    {
        endurance = Mathf.Clamp(endurance + amount, 0, MAX_ENDURANCE);
    }

    public void Dodge()
    {
        endurance -= ENDURANCE_COST;
        Dodging = true;
        dodgeTime = DodgeTime;
        myRenderer.material = DodgeMat;
    }

    private void EndDodge()
    {
        Dodging = false;
        myRenderer.material = DefaultMat;
    }

    private void Restore()
    {
        PoweredUp = false;
        myRenderer.material = DefaultMat;
    }

    public IVec2 GetPos()
    {
        return mazeRef.SectionLocFor(myRigidBody.position);
    }
}
