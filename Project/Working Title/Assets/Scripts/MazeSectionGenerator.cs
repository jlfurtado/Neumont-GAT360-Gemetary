using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct IVec2
{
    public IVec2(int x, int z) { this.x = x; this.z = z; }
    public int x, z;

    public bool Equals(IVec2 other)
    {
        return this.x == other.x && this.z == other.z;
    }
}

public struct RefArray<T>
{
    public RefArray(T[] reference, int start, int count) { this.reference = reference; this.start = start; this.count = count; }
    public int start, count;
    public T[] reference;
}

public class MazeSectionGenerator : MonoBehaviour {
    enum MazeSquare
    {
        WALL,
        UNVISITED,
        VISITED,
        SOLUTION,
        START,
        END,
        EMPTY
    }

    public int Value;
    public int RemoveCount = 1;
    public static float SquareSize;
    public static int Size;
    public RefArray<GameObject> FloorPool;
    public RefArray<GameObject> WallPool;
    public RefArray<Powerup> PowerupPool;
    public RefArray<EatForPoints> GemPool;
    public RefArray<FollowMazeSolution> FollowSolutionPool;
    public RefArray<DepthFirstExplore> DepthEnemyPool;
    public IVec2[] MazeSolution;
    public Material GemMat;

    private MazeSquare[,] mazeSections;
    private PlayerController playerRef;
    private ScoreManager scoreRef;
    private System.Random rand;
    private IVec2 powerupPos;
    private int numGems;
    private float diffMult;

    // Use this for initialization
    void Start ()
    {
        playerRef = GameObject.FindGameObjectWithTag(Strings.PLAYER_TAG).GetComponent<PlayerController>();
        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();
    }

    public void GenerateMaze(int seed)
    {
        rand = new System.Random(seed);
        GenerateMaze();
    }

    // returns a random node on the edge of a maze
    private IVec2 RandMazeEdgeVal(int size)
    {
        // pick a random wall
        int wallIdx = rand.Next(0, 4);

        // helper booleans for identifying which wall
        bool vert = (wallIdx & 1) != 0, begin = wallIdx < 2;
        int edge1 = 1, edge2 = size - 1, max = (size - 1) / 2;

        // pick x and y - one on the edge, other random node
        return new IVec2((vert ? (begin ? (edge1) : (edge2)) : (rand.Next(0, max) * 2 + 1)),
                        (vert ? (rand.Next(0, max) * 2 + 1) : (begin ? (edge1) : (edge2))));
    }

    private IVec2 RandWallNode(int size, int wallIdx)
    {
        // helper booleans for identifying which wall
        bool vert = (wallIdx & 1) != 0, begin = wallIdx < 2;
        int edge1 = 0, edge2 = size - 1, max = (size - 1) / 2;

        // pick x and y - one on the edge, other random node
        return new IVec2((vert ? (begin ? (edge1) : (edge2)) : (rand.Next(0, max) * 2 + 1)),
                        (vert ? (rand.Next(0, max) * 2 + 1) : (begin ? (edge1) : (edge2))));
    }

    public void EatAt(int x, int z)
    {
        if (mazeSections[x, z] == MazeSquare.VISITED || mazeSections[x, z] == MazeSquare.SOLUTION)
        {
            --numGems;
            if (numGems == 0)
            {
                // TODO: MORE AESTHETIC LIKE TEXT POPUP
                scoreRef.AddScore((int)(Mathf.Floor(Value * Difficulty())));
                playerRef.PowerUp();
            }
        }

        mazeSections[x, z] = MazeSquare.EMPTY;
    }

    private void GenerateMaze()
    {
        // create tiles
        mazeSections = new MazeSquare[Size, Size];

        // create base map
        for (int i = 0; i < Size; ++i)
        {
            for (int j = 0; j < Size; ++j)
            {
                mazeSections[i, j] = ((i & j & 1) == 0) ? MazeSquare.WALL : MazeSquare.UNVISITED; 
            }
        }

        // get random start
        IVec2 start = RandMazeEdgeVal(Size);

        // add start to traceback, no solution yet
        List<IVec2> traceback = new List<IVec2>(new IVec2[] { start });
        MazeSolution = new IVec2[0];

        // track longest route
        IVec2 longest = start;
        int mostMoves = 0;

        // mark start as visited
        mazeSections[start.x, start.z] = MazeSquare.VISITED;

        // depth-first generation
        while (traceback.Count > 0)
        {
            // get moves, current pos
            IVec2[] moves = new IVec2[4];
            int nextIdx = 0;
            IVec2 currentPos = traceback.Last();

            // check moves, add to array accordingly
            if (currentPos.x + 2 < Size && mazeSections[currentPos.x + 2, currentPos.z] == MazeSquare.UNVISITED) { moves[nextIdx++] = new IVec2(currentPos.x + 2, currentPos.z); }
            if (currentPos.x - 2 >= 0 && mazeSections[currentPos.x - 2, currentPos.z] == MazeSquare.UNVISITED) { moves[nextIdx++] = new IVec2(currentPos.x - 2, currentPos.z); }
            if (currentPos.z + 2 < Size && mazeSections[currentPos.x, currentPos.z + 2] == MazeSquare.UNVISITED) { moves[nextIdx++] = new IVec2(currentPos.x, currentPos.z + 2); }
            if (currentPos.z - 2 >= 0 && mazeSections[currentPos.x, currentPos.z - 2] == MazeSquare.UNVISITED) { moves[nextIdx++] = new IVec2(currentPos.x, currentPos.z - 2); }

            // if no moves pop
            if (nextIdx <= 0)
            {
                // update end... do we need this?
                if (traceback.Count > mostMoves)
                {
                    longest = currentPos;
                    MazeSolution = traceback.ToArray();
                    mostMoves = traceback.Count;
                }

                // backtrack
                traceback.RemoveAt(traceback.Count - 1);
            }
            else
            {
                // get wall pointsto modify
                IVec2 moveChosen = moves[rand.Next(0, nextIdx)];
                IVec2 between = new IVec2((moveChosen.x + currentPos.x) / 2, (moveChosen.z + currentPos.z) / 2);

                // we visited the node and take out wall between
                mazeSections[moveChosen.x, moveChosen.z] = MazeSquare.VISITED;
                mazeSections[between.x, between.z] = MazeSquare.VISITED;

                // make the move
                traceback.Add(moveChosen);
            }
        }

        for (int i = 0; i < MazeSolution.Length - 1; ++i)
        {
            IVec2 mv = MazeSolution[i], ot = MazeSolution[i + 1];
            IVec2 b = new IVec2((mv.x + ot.x) / 2, (mv.z + ot.z) / 2);

            // we visited the node and take out wall between
            mazeSections[mv.x, mv.z] = MazeSquare.SOLUTION;
            mazeSections[b.x, b.z] = MazeSquare.SOLUTION;
        }


        // mark start
        mazeSections[start.x, start.z] = MazeSquare.START;

        // mark end
        mazeSections[longest.x, longest.z] = MazeSquare.END;

        // remove chunks
        for (int i = 0; i < 2*RemoveCount; ++i)
        {
            IVec2 rmv = RandWallNode(Size, i%2);
            mazeSections[rmv.x, rmv.z] = MazeSquare.VISITED;
        }

        powerupPos = longest;
    }

    private GameObject MakeAt(RefArray<GameObject> obj, int idx, Vector3 location)
    {
        obj.reference[idx].SetActive(true);
        obj.reference[idx].transform.parent = transform;
        obj.reference[idx].transform.localPosition = location;
        return obj.reference[idx];
    }

    private GameObject MakeAt<T>(RefArray<T> obj, int idx, Vector3 location) where T : Component
    {
        obj.reference[idx].gameObject.SetActive(true);
        obj.reference[idx].gameObject.transform.parent = transform;
        obj.reference[idx].gameObject.transform.localPosition = location;
        return obj.reference[idx].gameObject;
    }

    public void RedoGeometry(IVec2 mazeLoc)
    {
        RedoMazeGeometry(mazeLoc);
    }

    private void RedoMazeGeometry(IVec2 mazeLoc)
    {
        // one giant floor object rather than tons of tiny ones - FPS++
        GameObject floor = MakeAt(FloorPool, FloorPool.start, Vector3.zero);
        floor.transform.localScale = new Vector3(Size * SquareSize, floor.transform.localScale.y, Size * SquareSize);

        int halfSize = Size / 2;
        int wallCount = WallPool.start, gemCount = GemPool.start;
        for (int x = 0; x < Size; ++x)
        {
            for (int z = 0; z < Size; ++z)
            {
                Vector3 location = new Vector3((x - halfSize) * SquareSize, 0.5f, (z - halfSize) * SquareSize);

                if (mazeSections[x, z] == MazeSquare.WALL)
                {
                    MakeAt(WallPool, wallCount++, location);
                }
                else if (mazeSections[x, z] == MazeSquare.VISITED || mazeSections[x, z] == MazeSquare.SOLUTION)
                {
                    MakeAt(GemPool, gemCount, location);
                    GemPool.reference[gemCount].gameObject.GetComponent<Renderer>().material = GemMat; // TODO: GET COMPONENT BEFORE SOMEHOW!!! 
                    GemPool.reference[gemCount].mazeLoc = mazeLoc;
                    GemPool.reference[gemCount].sectionLoc = new IVec2(x, z);
                    gemCount++;
                }
            }

        }

        numGems = gemCount - GemPool.start;

        if (mazeSections[powerupPos.x, powerupPos.z] != MazeSquare.EMPTY)
        {
            MakeAt(PowerupPool, PowerupPool.start, new Vector3((powerupPos.x - halfSize) * SquareSize, 1.0f, (powerupPos.z - halfSize) * SquareSize));
            PowerupPool.reference[PowerupPool.start].mazeLoc = mazeLoc;
            PowerupPool.reference[PowerupPool.start].sectionLoc = powerupPos;
        }

        float xPos = mazeLoc.x * Size * SquareSize, zPos = mazeLoc.z * Size * SquareSize;
        float dist = Mathf.Sqrt(xPos * xPos + zPos * zPos);
        diffMult = Mathf.Log(10.0f + dist);
        MakeAt(FollowSolutionPool, FollowSolutionPool.start, new Vector3((MazeSolution[0].x - halfSize) * SquareSize, 0.8f, (MazeSolution[0].z - halfSize) * SquareSize));
        FollowSolutionPool.reference[FollowSolutionPool.start].UpdateRef(this);
        FollowSolutionPool.reference[FollowSolutionPool.start].Speed = 1.25f * diffMult;

        MakeAt(DepthEnemyPool, DepthEnemyPool.start, new Vector3((MazeSolution[0].x - halfSize) * SquareSize, 0.8f, (MazeSolution[0].z - halfSize) * SquareSize));
        DepthEnemyPool.reference[DepthEnemyPool.start].UpdateRef(this);
        DepthEnemyPool.reference[DepthEnemyPool.start].Speed = 1.1f * diffMult;
    }

    public float Difficulty()
    {
        return diffMult;
    }

    public Vector3 PositionAt(IVec2 position)
    {
        int halfSize = Size / 2;
        return gameObject.transform.position + (new Vector3((position.x - halfSize) * SquareSize, 0.0f, (position.z - halfSize) * SquareSize));
    }


    public bool IsWall(IVec2 loc)
    {
        return IsWall(loc.x, loc.z);
    }

    public bool IsWall(int x, int z)
    {
        return mazeSections[x, z] == MazeSquare.WALL;
    }

    //public IVec2 PositionAt(Vector3 position)
    //{
    //    int halfSize = Size / 2;
    //    Vector3 alt = (position - gameObject.transform.position) / SquareSize;
    //    return new IVec2(halfSize + (int)Mathf.Floor(alt.x), halfSize + (int)Mathf.Floor(alt.z));
    //}

}
