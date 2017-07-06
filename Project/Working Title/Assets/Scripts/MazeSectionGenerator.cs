using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct IVec2
{
    public IVec2(int x, int z) { this.x = x; this.z = z; }
    public int x, z;
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
        NO_GEM
    }

    public static float SquareSize;
    public static int Size;
    public GameObject[] FloorPool;
    public GameObject[] WallPool;
    public GameObject[] PowerupPool;
    public EatForPoints[] GemPool;

    private MazeSquare[,] mazeSections;
    private System.Random rand;
    private IVec2 powerupPos;

    // Use this for initialization
    void Start ()
    {
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

    public void EatGem(int x, int z)
    {
        mazeSections[x, z] = MazeSquare.NO_GEM;
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
        IVec2[] solution = new IVec2[0];

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
                    solution = traceback.ToArray();
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

        for (int i = 0; i < solution.Length - 1; ++i)
        {
            IVec2 mv = solution[i], ot = solution[i + 1];
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
        for (int i = 0; i < 4; ++i)
        {
            IVec2 rmv = RandWallNode(Size, i%2);
            mazeSections[rmv.x, rmv.z] = MazeSquare.VISITED;
        }

        powerupPos = longest;
    }

    private GameObject MakeAt(GameObject[] pool, int index, Vector3 location)
    {
        return MakeAt(pool[index], location);
    }

    private GameObject MakeAt(GameObject obj, Vector3 location)
    {
        obj.SetActive(true);
        obj.transform.parent = transform;
        obj.transform.localPosition = location;
        return obj;
    }
    
    public void RedoMazeGeometry(IVec2 mazeLoc)
    {
        // one giant floor object rather than tons of tiny ones - FPS++
        GameObject floor = MakeAt(FloorPool, 0, Vector3.zero);
        floor.transform.localScale = new Vector3(Size * SquareSize, floor.transform.localScale.y, Size * SquareSize);

        int halfSize = Size / 2;
        int wallCount = 0, gemCount = 0;
        for (int x = 0; x < Size; ++x)
        {
            for (int z = 0; z < Size; ++z)
            {
                Vector3 location = new Vector3((x - halfSize) * SquareSize, 0.5f, (z - halfSize) * SquareSize);

                if (mazeSections[x, z] == MazeSquare.WALL)
                {
                    MakeAt(WallPool, wallCount++, location);
                }
                else if (mazeSections[x, z] == MazeSquare.VISITED)
                {
                    MakeAt(GemPool[gemCount].gameObject, location);
                    GemPool[gemCount].mazeLoc = mazeLoc;
                    GemPool[gemCount].sectionLoc = new IVec2(x, z);
                    gemCount++;
                }
            }
        }

        MakeAt(PowerupPool, 0, new Vector3((powerupPos.x - halfSize) * SquareSize, 1.0f, (powerupPos.z - halfSize) * SquareSize));
    }

}
