using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer))]
public class PlayerController : MonoBehaviour {
    public float Speed;
    public Material[] Colors;
    public Material DefaultMat;
    public float PowerupTime;
    private MazeScript mazeRef;
    private const float CLOSE_ENOUGH = 0.1f;
    private const float PAST = 0.01f;
    private Enemy[] enemies;

    public bool PoweredUp { get; private set; }
    private float remainingPowerTime;
    private Rigidbody myRigidBody = null;
    private Renderer myRenderer = null;
    private bool moving = false;
    private Vector3 fromPos, toPos;
    private bool horizLast;

    // Use this for initialization
    void Start () {
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
        bool down = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        bool up = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        bool left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

        int horiz = 0;
        int vert = 0;

        if (down) { --vert; } if (up) { ++vert; }
        if (left) { --horiz; } if (right) { ++horiz; }

        IVec2 dir = new IVec2(horiz, vert);

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
            if (remainingPowerTime <= 0.0f) { PoweredUp = false; myRenderer.material = DefaultMat; }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            PowerUp();
        }
        mazeRef.GenerateAround(myRigidBody.position);

    }

    public void PowerUp()
    {
        PoweredUp = true;
        remainingPowerTime = PowerupTime;

        foreach (Enemy enemy in enemies)
        {
            enemy.StopFor(PowerupTime);
        }

    }

    public IVec2 GetPos()
    {
        return mazeRef.SectionLocFor(myRigidBody.position);
    }
}
