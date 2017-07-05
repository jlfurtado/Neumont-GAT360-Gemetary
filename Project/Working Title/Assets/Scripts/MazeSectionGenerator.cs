﻿using System.Collections;
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
        END
    }

    public static float SquareSize;
    public static int Size;
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public GameObject PowerupPrefab;
    public GameObject GemPrefab;

    private MazeSquare[,] mazeSections;
    private static System.Random rand = new System.Random();

	// Use this for initialization
	void Start ()
    {
        GenerateMaze(Size);	
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

    private void GenerateMaze(int size)
    {
        // create tiles
        mazeSections = new MazeSquare[Size, Size];

        // create base map
        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                mazeSections[i, j] = ((i & j & 1) == 0) ? MazeSquare.WALL : MazeSquare.UNVISITED; 
            }
        }

        // get random start
        IVec2 start = RandMazeEdgeVal(size);

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
            if (currentPos.x + 2 < size && mazeSections[currentPos.x + 2, currentPos.z] == MazeSquare.UNVISITED) { moves[nextIdx++] = new IVec2(currentPos.x + 2, currentPos.z); }
            if (currentPos.x - 2 >= 0 && mazeSections[currentPos.x - 2, currentPos.z] == MazeSquare.UNVISITED) { moves[nextIdx++] = new IVec2(currentPos.x - 2, currentPos.z); }
            if (currentPos.z + 2 < size && mazeSections[currentPos.x, currentPos.z + 2] == MazeSquare.UNVISITED) { moves[nextIdx++] = new IVec2(currentPos.x, currentPos.z + 2); }
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
            IVec2 rmv = RandWallNode(size, i%2);
            mazeSections[rmv.x, rmv.z] = MazeSquare.VISITED;
        }

        // one giant floor object rather than tons of tiny ones - FPS++
        GameObject floor = MakeAt(FloorPrefab, Vector3.zero);
        floor.transform.localScale = new Vector3(size * SquareSize, floor.transform.localScale.y, size * SquareSize);

        int halfSize = size / 2;
        for (int x = 0; x < size; ++x)
        {
            for (int z = 0; z < size; ++z)
            {
                Vector3 location = new Vector3((x - halfSize) * SquareSize, 0.5f, (z - halfSize) * SquareSize);

                if (mazeSections[x, z] == MazeSquare.WALL)
                {
                    MakeAt(WallPrefab, location);
                }
                else if (mazeSections[x, z] == MazeSquare.VISITED)
                {
                    MakeAt(GemPrefab, location);
                }
            }
        }

        MakeAt(PowerupPrefab, new Vector3((longest.x - halfSize) * SquareSize, 1.0f, (longest.z - halfSize) * SquareSize));
    }

    private GameObject MakeAt(GameObject prefab, Vector3 location)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.transform.localPosition = location;
        return obj;
    }

}
