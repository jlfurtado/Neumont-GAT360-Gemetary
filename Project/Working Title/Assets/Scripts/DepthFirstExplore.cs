using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer), typeof(Collider))]
public class DepthFirstExplore : MonoBehaviour {
    public int Value;
    public float Speed = 1.0f;
    public Material NormalMat;
    public Material StoppedMat;
    private MazeScript mazeRef;
    private MazeSectionGenerator mazeSection;
    private const float CLOSE_ENOUGH = 0.1f;
    private const float PAST = 0.01f;
    private Rigidbody myRigidBody;
    private Renderer myRenderer;
    private SceneMover sceneMoverRef;
    private ScoreManager scoreRef;
    private float stopTime = 0.0f;
    private bool stopped = false;
    private Stack<IVec2> explore = new Stack<IVec2>();
    private IVec2 next, from;
    private static System.Random rand = new System.Random();
    private bool[,] visited = null;

    // Use this for initialization
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        myRigidBody.velocity = Vector3.zero;
        myRenderer = GetComponent<Renderer>();
        sceneMoverRef = GameObject.FindGameObjectWithTag(Strings.SCENE_MOVER_TAG).GetComponent<SceneMover>();
        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();
        mazeRef = GameObject.FindGameObjectWithTag(Strings.MAZE_TAG).GetComponent<MazeScript>();
        visited = new bool[mazeRef.SectionSize, mazeRef.SectionSize];
    }

    // Update is called once per frame
    void Update()
    {
        if (explore.Count == 0)
        {
            next = mazeRef.SectionLocFor(transform.position);
            from = mazeRef.SectionLocFor(transform.position);
            ClearVisited();
            visited[from.x, from.z] = true;
            explore.Push(from);
        }

        Vector3 tp = mazeSection.PositionAt(next), fp = mazeSection.PositionAt(from);
        Vector3 toPos = new Vector3(tp.x, transform.position.y, tp.z), fromPos = new Vector3(fp.x, transform.position.y, fp.z);
        Vector3 moving = toPos - transform.position;

        if (stopped)
        {
            stopTime -= Time.deltaTime;
            myRenderer.material = Mathf.Sqrt(stopTime) * 100 % 7 < 2 ? NormalMat : StoppedMat;
            if (stopTime < 0.0f)
            {
                stopTime = 0.0f;
                stopped = false;
                myRenderer.material = NormalMat;
            }
        }
        else
        {
            if (Vector3.Dot((moving).normalized, ((toPos - fromPos).normalized)) < PAST || (moving).magnitude < CLOSE_ENOUGH)
            {
                // arrived at a section
                int nextIdx = 0;
                IVec2[] moves = new IVec2[4];

                // we're at the new next
                IVec2 current = next;

                // get moves
                if (current.x + 2 < MazeSectionGenerator.Size && !mazeSection.IsWall(current.x + 1, current.z) && !mazeSection.IsWall(current.x + 2, current.z) && !visited[current.x + 2, current.z]) { moves[nextIdx++] = new IVec2(current.x + 2, current.z); }
                if (current.x - 2 >= 0                        && !mazeSection.IsWall(current.x - 1, current.z) && !mazeSection.IsWall(current.x - 2, current.z) && !visited[current.x - 2, current.z]) { moves[nextIdx++] = new IVec2(current.x - 2, current.z); }
                if (current.z + 2 < MazeSectionGenerator.Size && !mazeSection.IsWall(current.x, current.z + 1) && !mazeSection.IsWall(current.x, current.z + 2) && !visited[current.x, current.z + 2]) { moves[nextIdx++] = new IVec2(current.x, current.z + 2); }
                if (current.z - 2 >= 0                        && !mazeSection.IsWall(current.x, current.z - 1) && !mazeSection.IsWall(current.x, current.z - 2) && !visited[current.x, current.z - 2]) { moves[nextIdx++] = new IVec2(current.x, current.z - 2); }

                // no moves
                if (nextIdx <= 0)
                {
                    if (explore.Count > 0)
                    {
                        from = explore.Pop();
                        visited[from.x, from.z] = true;
                        if (explore.Count > 0)
                        {
                            next = explore.Peek();
                        }
                        else
                        {
                            next = from;
                            ClearVisited();
                        }
                    }
                }
                else
                {
                    IVec2 moveChosen = moves[rand.Next(0, nextIdx)];
                    explore.Push(moveChosen);
                    from = next;
                    next = moveChosen;
                    visited[from.x, from.z] = true;
                }

                myRigidBody.velocity = Vector3.zero;
            }
            else
            {
                Vector3 vel = toPos - fromPos;
                myRigidBody.velocity = vel.normalized * Speed;
            }
        }

    }

    private void ClearVisited()
    {
        for (int x = 0; x < mazeRef.SectionSize; ++x)
        {
            for (int z = 0; z < mazeRef.SectionSize; ++z)
            {
                visited[x, z] = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Strings.PLAYER_TAG))
        {
            // only get comp if we hit the player
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            if (pc.PoweredUp || stopped)
            {
                // oh no, we died!
                scoreRef.AddScore(Value);
                gameObject.SetActive(false);
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
        stopped = true;
        stopTime = time;
        myRenderer.material = StoppedMat;
        myRigidBody.velocity = Vector3.zero;
    }

    public void UpdateRef(MazeSectionGenerator mazeSection)
    {
        this.mazeSection = mazeSection;
        if (explore != null && explore.Count > 0) { explore.Clear(); }
        if (myRigidBody != null) { myRigidBody.velocity = Vector3.zero; }
    }
}
