using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer), typeof(Collider))]
public class Enemy : MonoBehaviour {
    public int Value;
    public float Speed = 1.0f;
    public Material NormalMat;
    public Material StoppedMat;
    public Material EatenMat;

    private float stopTime = 0.0f;
    private bool stopped = false;
    protected bool Eaten { get; private set; }
    protected MazeScript mazeRef;
    protected MazeSectionGenerator mazeSection;
    protected Rigidbody myRigidBody;
    protected Renderer myRenderer;
    protected SceneMover sceneMoverRef;
    protected ScoreManager scoreRef;
    protected static System.Random rand = new System.Random();
    protected IVec2 next, from;
    protected const float CLOSE_ENOUGH = 0.1f;
    protected const float PAST = 0.01f;
    protected IVec2 home;

    // Use this for initialization
    public virtual void Start () {
        Eaten = false;
        mazeRef = GameObject.FindGameObjectWithTag(Strings.MAZE_TAG).GetComponent<MazeScript>();
        myRigidBody = GetComponent<Rigidbody>();
        myRigidBody.velocity = Vector3.zero;
        myRenderer = GetComponent<Renderer>();
        sceneMoverRef = GameObject.FindGameObjectWithTag(Strings.SCENE_MOVER_TAG).GetComponent<SceneMover>();
        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();
    }

    protected int IdxFromXZ(int x, int z)
    {
        return x * mazeRef.SectionSize + z;
    }

    protected bool IsNode(IVec2 pos)
    {
        return (pos.x & pos.z & 1) != 0;
    }

    protected IVec2 ForceToBeNode(IVec2 pos)
    {
        if ((pos.x & pos.z & 1) == 0)
        {
            if (pos.x + 1 < MazeSectionGenerator.Size && !mazeSection.IsWall(pos.x + 1, pos.z)) { pos.x++; }
            else if (pos.x - 1 >= 0 && !mazeSection.IsWall(pos.x - 1, pos.z)) { pos.x--; }
            else if (pos.z + 1 < MazeSectionGenerator.Size && !mazeSection.IsWall(pos.x, pos.z + 1)) { pos.z++; }
            else if (pos.z - 1 >= 0 && !mazeSection.IsWall(pos.x, pos.z - 1)) { pos.z--; }
            // TODO: ASSUMES ONE UP DOWN LEFT OR RIGHT MOVE FROM NODE!!! DIAGONAL COMPUTATIONS COMPLEX IMPLEMENT IF NEEDED
        }

        return pos;
    }

    public IVec2 GetPos()
    {
        return mazeRef.SectionLocFor(myRigidBody.position);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (stopped)
        {
            stopTime -= Time.deltaTime;
            myRenderer.material = Mathf.Sqrt(stopTime) * 100 % 7 < 2 ? NormalMat : StoppedMat;
            if (stopTime < 0.0f)
            {
                UnStop();
            }
        }

        Vector3 tp = mazeSection.PositionAt(next), fp = mazeSection.PositionAt(from);
        Vector3 toPos = new Vector3(tp.x, myRigidBody.position.y, tp.z), fromPos = new Vector3(fp.x, myRigidBody.position.y, fp.z);
        Vector3 moving = toPos - myRigidBody.position;

        if (Vector3.Dot((moving).normalized, ((toPos - fromPos).normalized)) < PAST || (moving).magnitude < CLOSE_ENOUGH)
        {
            if (Eaten && next.Equals(home)) { Restore(); }
            if (stopped && next.Equals(home)) { UnStop(); }
            if (stopped || Eaten) { OnLandReturn(toPos); }
            else { OnLand(toPos); }
        }
        else
        {
            Move(toPos, fromPos);
        }
    }

    protected virtual void OnLand(Vector3 toPos)
    {
        myRigidBody.position = toPos;
        myRigidBody.velocity = Vector3.zero;
    }

    protected virtual void OnLandReturn(Vector3 toPos)
    {
        myRigidBody.position = toPos;
        myRigidBody.velocity = Vector3.zero;
    }

    private void Move(Vector3 toPos, Vector3 fromPos)
    {
        Vector3 vel = toPos - fromPos;
        myRigidBody.velocity = vel.normalized * Speed;
    }

    protected void UnStop()
    {
        stopTime = 0.0f;
        stopped = false;
        myRenderer.material = NormalMat;
    }

    protected void EatMe()
    {
        Eaten = true;
        UnStop();
        myRenderer.material = EatenMat;
    }

    protected void Restore()
    {
        Eaten = false;
        myRenderer.material = NormalMat;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Strings.PLAYER_TAG) && !Eaten)
        {
            // only get comp if we hit the player
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            if (pc.PoweredUp || stopped)
            {
                // oh no, we died!
                scoreRef.AddScore((int)Mathf.Floor(Value * Speed));
                EatMe();
            }
            else
            {
                // only move to game over if they aren't powered up
                sceneMoverRef.MoveToGameOver();
            }
        }
    }

    public void StopFor(float time)
    {
        if (!Eaten)
        {
            stopped = true;
            stopTime = time;
            myRenderer.material = StoppedMat;
            myRigidBody.velocity = Vector3.zero;
        }
    }

    public virtual void UpdateRef(MazeSectionGenerator mazeSection)
    {
        this.mazeSection = mazeSection;
        stopped = false;
        stopTime = 0.0f;
        Eaten = false;

        home = mazeSection.MazeSolution[0];

        if (myRenderer != null)
        {
            myRenderer.material = NormalMat;
            if (Eaten) { Restore(); }
            if (stopped) { UnStop(); }
        } 

        if (myRigidBody != null) { myRigidBody.velocity = Vector3.zero; myRigidBody.position = mazeSection.PositionAt(home); }
    }
}
