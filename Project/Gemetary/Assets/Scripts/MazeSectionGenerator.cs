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

    public float DistanceTo(IVec2 other)
    {
        float xDiff = this.x - other.x;
        float zDiff = this.z - other.z;
        return Mathf.Sqrt(xDiff * xDiff + zDiff * zDiff);
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
        EMPTY,
        SPECIAL
    }

    public float FloorScale;
    public Vector3 FloorOffset;
    public int Value;
    public static float SquareSize;
    public static int Size;
    public static int BombsPerSection;
    public RefArray<Bomb> BombPool;
    public RefArray<GameObject> RestorerPool;
    public RefArray<Renderer> FloorPool;
    public RefArray<GameObject> PillarPool;
    public RefArray<GameObject> FencePool;
    public RefArray<Powerup> PowerupPool;
    public RefArray<EatForPoints> GemPool;
    public RefArray<Enemy> EnemyPool;
    public RefArray<GameObject> FogPool;
    public IVec2[] MazeSolution;
    public Material GemMat;
    public Material FloorMat;

    private MazeSquare[] mazeSections;
    private PlayerController playerRef;
    private ScoreManager scoreRef;
    private System.Random rand;
    private IVec2 powerupPos;
    private int startGems = -1;
    private int numGems;
    private float diffMult;
    public bool Generating { get; private set; }

    // Use this for initialization
    void Awake()
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
        return RandMazeEdgeVal(size, rand.Next(0, 4));
    }

    private IVec2 RandMazeEdgeVal(int size, int wallIdx)
    {
        // helper booleans for identifying which wall
        bool vert = (wallIdx & 1) != 0, begin = wallIdx < 2;
        int edge1 = 1, edge2 = size - 1, max = (size - 1) / 2;

        // pick x and y - one on the edge, other random node
        return new IVec2((vert ? (begin ? (edge1) : (edge2)) : (rand.Next(0, max) * 2 + 1)),
                        (vert ? (rand.Next(0, max) * 2 + 1) : (begin ? (edge1) : (edge2))));
    }

    private IVec2 RandBetweenNodes(int size)
    {
        int max = (size - 1) / 2;
        return new IVec2((rand.Next(0, max) * 2),
                         (rand.Next(0, max) * 2));
    }

    private int IdxFromXZ(int x, int z)
    {
        return x * Size + z;
    }

    public void EatAt(int x, int z)
    {
        int idx = IdxFromXZ(x, z);
        if (mazeSections[idx] == MazeSquare.VISITED || mazeSections[idx] == MazeSquare.SOLUTION)
        {
            --numGems;
            if (numGems == 0)
            {
                scoreRef.AddScore((int)(Mathf.Floor(Value * Difficulty())));
                playerRef.PowerUp();
            }
        }

        mazeSections[idx] = MazeSquare.EMPTY;
    }

    public int NumGems()
    {
        return numGems;
    }

    private void GenerateMaze()
    {
        // create tiles
        mazeSections = new MazeSquare[Size * Size];

        // create base map
        for (int i = 0, x = 0; i < Size; ++i)
        {
            for (int j = 0; j < Size; ++j)
            {
                mazeSections[x++] = ((i & j & 1) == 0) ? MazeSquare.WALL : MazeSquare.UNVISITED; 
            }
        }

        // get random start
        IVec2 start = RandMazeEdgeVal(Size);
        int startIdx = IdxFromXZ(start.x, start.z);

        // add start to traceback, no solution yet
        Stack<IVec2> traceback = new Stack<IVec2>(Size * Size); // make default size greater than we ever expect it to be, avoid cost of dynamic resize copy
        traceback.Push(start);
        MazeSolution = new IVec2[0];

        // track longest route
        IVec2 longest = start;
        int mostMoves = 0;

        // mark start as visited
        mazeSections[startIdx] = MazeSquare.VISITED;

        // depth-first generation
        while (traceback.Count > 0)
        {
            // get moves, current pos
            IVec2[] moves = new IVec2[4];
            int nextIdx = 0;
            IVec2 currentPos = traceback.Peek();

            // check moves, add to array accordingly
            if (currentPos.x + 2 < Size && mazeSections[IdxFromXZ(currentPos.x + 2, currentPos.z)] == MazeSquare.UNVISITED) { moves[nextIdx++] = new IVec2(currentPos.x + 2, currentPos.z); }
            if (currentPos.x - 2 >= 0 && mazeSections[IdxFromXZ(currentPos.x - 2, currentPos.z)] == MazeSquare.UNVISITED) { moves[nextIdx++] = new IVec2(currentPos.x - 2, currentPos.z); }
            if (currentPos.z + 2 < Size && mazeSections[IdxFromXZ(currentPos.x, currentPos.z + 2)] == MazeSquare.UNVISITED) { moves[nextIdx++] = new IVec2(currentPos.x, currentPos.z + 2); }
            if (currentPos.z - 2 >= 0 && mazeSections[IdxFromXZ(currentPos.x, currentPos.z - 2)] == MazeSquare.UNVISITED) { moves[nextIdx++] = new IVec2(currentPos.x, currentPos.z - 2); }

            // if no moves pop
            if (nextIdx <= 0)
            {
                // update end... do we need this?
                if (traceback.Count > mostMoves)
                {
                    longest = currentPos;
                    MazeSolution = traceback.Reverse().ToArray();
                    mostMoves = traceback.Count;
                }

                // backtrack
                traceback.Pop();
            }
            else
            {
                // get wall pointsto modify
                IVec2 moveChosen = moves[rand.Next(0, nextIdx)];
                IVec2 between = new IVec2((moveChosen.x + currentPos.x) / 2, (moveChosen.z + currentPos.z) / 2);

                // we visited the node and take out wall between
                mazeSections[IdxFromXZ(moveChosen.x, moveChosen.z)] = MazeSquare.VISITED;
                mazeSections[IdxFromXZ(between.x, between.z)] = MazeSquare.VISITED;

                // make the move
                traceback.Push(moveChosen);
            }
        }

        for (int i = 0; i < MazeSolution.Length - 1; ++i)
        {
            IVec2 mv = MazeSolution[i], ot = MazeSolution[i + 1];
            IVec2 b = new IVec2((mv.x + ot.x) / 2, (mv.z + ot.z) / 2);

            // we visited the node and take out wall between
            mazeSections[IdxFromXZ(mv.x, mv.z)] = MazeSquare.SOLUTION;
            mazeSections[IdxFromXZ(b.x, b.z)] = MazeSquare.SOLUTION;
        }

        // mark start
        mazeSections[startIdx] = MazeSquare.START;

        // mark end
        mazeSections[IdxFromXZ(longest.x, longest.z)] = MazeSquare.END;

        powerupPos = longest;

        for (int i = 0; i < BombsPerSection; ++i)
        {
            IVec2 specialPos;

            do
            {
                specialPos = RandMazeEdgeVal(Size, (i + IdxFromXZ(longest.x, longest.z)) % 4);
            } while (specialPos.Equals(powerupPos) || specialPos.Equals(MazeSolution[0]));

            mazeSections[IdxFromXZ(specialPos.x, specialPos.z)] = MazeSquare.SPECIAL;
        }
    }

    private bool WallOrEdge(int x, int z)
    {
        return (x <= 0 || x >= (Size - 1) || z <= 0 || z >= (Size - 1) || IsWall(x, z));
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

    public void RedoGeometry(int x, int z, bool sync)
    {
        StartCoroutine(RedoMazeGeometry(new IVec2(x, z), sync));
    }

    public void CalcDiff(IVec2 mazeLoc)
    {
        float xPos = mazeLoc.x * Size * SquareSize, zPos = mazeLoc.z * Size * SquareSize;
        float distSqrd = xPos * xPos + zPos * zPos;
        float m = ScoreManager.IsHardcore() ? 1.5f : 1.0f;
        diffMult = m * Mathf.Log10(10.0f + m*distSqrd);
    }

    private IEnumerator SpawnEnemy(int index, float delay)
    {
        int halfSize = Size / 2;
        int q = EnemyPool.start + index;

        yield return new WaitForSeconds(delay);

        MakeAt(EnemyPool, q, new Vector3((MazeSolution[0].x - halfSize) * SquareSize, 0.0f, (MazeSolution[0].z - halfSize) * SquareSize));
        Enemy e = EnemyPool.reference[q];
        e.UpdateRef(this);
        e.Speed = e.BaseSpeed * diffMult * (float)(rand.NextDouble() * 0.2f + 0.9f);
    }

    private IEnumerator RedoMazeGeometry(IVec2 mazeLoc, bool sync)
    {
        int halfSize = Size / 2;
        Generating = true;

        // one giant floor object rather than tons of tiny ones - FPS++
        GameObject floor = MakeAt(FloorPool, FloorPool.start, FloorOffset);
        floor.transform.localScale = new Vector3(Size * SquareSize * FloorScale, floor.transform.localScale.y, Size * SquareSize * FloorScale);
        FloorPool.reference[FloorPool.start].material = FloorMat;

        GameObject fog = MakeAt(FogPool, FogPool.start, FloorOffset);

        Vector3 dir = new Vector3(MazeSolution[1].x - MazeSolution[0].x, 0.0f, MazeSolution[1].z - MazeSolution[0].z);
        MakeAt(RestorerPool, RestorerPool.start, new Vector3((MazeSolution[0].x - halfSize) * SquareSize, 0.5f, (MazeSolution[0].z - halfSize) * SquareSize)).transform.rotation = Quaternion.LookRotation(dir);

        if (mazeSections[IdxFromXZ(powerupPos.x, powerupPos.z)] != MazeSquare.EMPTY)
        {
            MakeAt(PowerupPool, PowerupPool.start, new Vector3((powerupPos.x - halfSize) * SquareSize, 1.0f, (powerupPos.z - halfSize) * SquareSize));
            PowerupPool.reference[PowerupPool.start].mazeLoc = mazeLoc;
            PowerupPool.reference[PowerupPool.start].sectionLoc = powerupPos;
        }
     
        int pillarCount = PillarPool.start, gemCount = GemPool.start, fenceCount = FencePool.start, bombCount = BombPool.start, idx = 0;
        for (int x = 0; x < Size; ++x)
        {
            for (int z = 0; z < Size; ++z)
            {
                Vector3 location = new Vector3((x - halfSize) * SquareSize, 1.0f, (z - halfSize) * SquareSize);

                if (mazeSections[idx] == MazeSquare.WALL)
                {
                    // depending on even even or even odd or odd odd make fence, vert fence, or pillar
                    if (((x | z) & 1) == 0) { MakeAt(PillarPool, pillarCount++, location + (Vector3.down * 1.0f)); }
                    else
                    {
                        MakeAt(FencePool, fenceCount++, location).transform.localRotation = Quaternion.Euler(0.0f, (z & 1) == 0 ? 90.0f : 0.0f, 0.0f);
                    }
                }
                else if (mazeSections[idx] == MazeSquare.VISITED || mazeSections[idx] == MazeSquare.SOLUTION)
                {
                    MakeAt(GemPool, gemCount, location);
                    GemPool.reference[gemCount].SetMat(GemMat);
                    GemPool.reference[gemCount].mazeLoc = mazeLoc;
                    GemPool.reference[gemCount].sectionLoc = new IVec2(x, z);
                    gemCount++;
                }
                else if (mazeSections[idx] == MazeSquare.SPECIAL)
                {
                    MakeAt(BombPool, bombCount, location + (Vector3.down * 1.0f));
                    BombPool.reference[bombCount++].UpdateLoc(mazeLoc, new IVec2(x, z));
                }

                idx++;
                if (!sync) { yield return null; }
            }
        }

        numGems = gemCount - GemPool.start;
        if (startGems == -1) { startGems = numGems; } // hack!

        for (int i = 0; i < EnemyPool.count; ++i)
        {
            StartCoroutine(SpawnEnemy(i, 1.0f * i));
        }

        Generating = false;
        fog.SetActive(false);
    }

    public float GemPercent()
    {
        return (float)numGems / startGems;
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
        return Generating || mazeSections[IdxFromXZ(x, z)] == MazeSquare.WALL;
    }

    public bool IsWalkable(IVec2 loc)
    {
        return IsWalkable(loc.x, loc.z);
    }

    public bool IsWalkable(int x, int z)
    {
        return !IsWall(x, z) && (MazeSolution[0].x != x || MazeSolution[0].z != z);
    }
}
