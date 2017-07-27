using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer), typeof(Collider))]
public class ChaseEnemy : Enemy
{
    public int PlayerFollowMoves;
    public int FriendFollowMoves;
    private Stack<IVec2> path = null;
    private Stack<IVec2> playerPath = null;
    private Stack<IVec2>[] friendPaths = null;
    private const float MOVE_COST = 10.0f;
    private Enemy[] myFriends = null;
    private int friendCount;
    private IVec2[] moves = new IVec2[4]; // Don't re-make array over and over
    private AStarPathNode[] adjacentNodes = new AStarPathNode[4]; // don't re-make array over and over
    private AStarPathNode[] mySectionNodes = null;

    // Use this for initialization
    public override void Awake()
    {
        base.Awake(); // set base refs
        path = new Stack<IVec2>(mazeRef.SectionSize * mazeRef.SectionSize);
        playerPath = new Stack<IVec2>(mazeRef.SectionSize * mazeRef.SectionSize);

        if (mazeRef != null)
        {
            next = mazeRef.SectionLocFor(myRigidBody.position);
            from = mazeRef.SectionLocFor(myRigidBody.position);
        }

        // don't spam-re allocate array!
        myFriends = new Enemy[mazeRef.MaxEnemiesPerSection];

        // don't instantiate stacks over and over
        friendPaths = new Stack<IVec2>[mazeRef.MaxEnemiesPerSection];
        for (int i = 0; i < friendPaths.Length; ++i)
        {
            friendPaths[i] = new Stack<IVec2>(mazeRef.SectionSize * mazeRef.SectionSize);
        }

        mySectionNodes = new AStarPathNode[mazeRef.SectionSize * mazeRef.SectionSize];
        for (int i = 0; i < mySectionNodes.Length; ++i)
        {
            mySectionNodes[i] = new AStarPathNode(null, null, new IVec2(i / mazeRef.SectionSize, i % mazeRef.SectionSize), 0.0f);
        }
    }

    protected override void OnLandReturn(Vector3 toPos)
    {
        // snap
        base.OnLandReturn(toPos);

        GetPath(home, ref path);

        if (path.Count == 0)
        {
            next = mazeRef.SectionLocFor(myRigidBody.position);
            from = mazeRef.SectionLocFor(myRigidBody.position);
        }

        if (path != null && path.Count > 0)
        {
            from = mazeRef.SectionLocFor(myRigidBody.position);
            next = path.Pop();
        }
    }

    protected override void OnLand(Vector3 toPos)
    {
        // snap
        base.OnLand(toPos);

        if (path.Count == 0)
        {
            next = mazeRef.SectionLocFor(myRigidBody.position);
            from = mazeRef.SectionLocFor(myRigidBody.position);
        }

        bool playerFound = mazeRef.SectionAt(playerRef.transform.position) ==  mazeSection && GetPath(playerRef.GetPos(), ref playerPath);
        for (int i = 0; i < friendCount; ++i)
        {
            GetPath(myFriends[i].GetPos(), ref friendPaths[i]);
        }

        int selected = 0, num = friendPaths[0].Count;
        for (int i = 1; i < friendCount; ++i)
        {
            if (friendPaths[i].Count < num)
            {
                num = friendPaths[i].Count;
                selected = i;
            }
        }

        path = (playerFound && playerPath.Count <= num) ? playerPath : friendPaths[selected];

        if (path != null && path.Count > 0)
        {
            from = mazeRef.SectionLocFor(myRigidBody.position);
            next = path.Pop();
        }
    }

    public override void UpdateRef(MazeSectionGenerator mazeSection)
    {
        base.UpdateRef(mazeSection);
        if (path != null && path.Count > 0) { path.Clear(); }
        if (mazeRef != null && myRigidBody != null)
        { 
            next = mazeRef.SectionLocFor(myRigidBody.position);
            from = mazeRef.SectionLocFor(myRigidBody.position);
        }

        // my friends are all but me
        friendCount = 0;
        for (int i = 0; i < mazeSection.EnemyPool.count; ++i)
        {
            int w = mazeSection.EnemyPool.start + i;
            if (!(mazeSection.EnemyPool.reference[w] is ChaseEnemy))
            {
                myFriends[friendCount++] = mazeSection.EnemyPool.reference[w];
            }
        }

        // null out the rest of the friends
        for (int i = friendCount; i < myFriends.Length; ++i)
        {
            myFriends[i] = null;
        }
    }

    private void FindNodes(AStarPathNode currentNode, AStarPathNode endNode, out AStarPathNode[] nodes, out int nodeCount)
    {
        // arrived at a section
        int nextIdx = 0;
        IVec2 pos = currentNode.Location;

        // get moves
        if (pos.x + 1 < MazeSectionGenerator.Size && !mazeSection.IsWall(pos.x + 1, pos.z)) { moves[nextIdx++] = new IVec2(pos.x + 1, pos.z); }
        if (pos.x - 1 >= 0 && !mazeSection.IsWall(pos.x - 1, pos.z)) { moves[nextIdx++] = new IVec2(pos.x - 1, pos.z); }
        if (pos.z + 1 < MazeSectionGenerator.Size && !mazeSection.IsWall(pos.x, pos.z + 1)) { moves[nextIdx++] = new IVec2(pos.x, pos.z + 1); }
        if (pos.z - 1 >= 0 && !mazeSection.IsWall(pos.x, pos.z - 1)) { moves[nextIdx++] = new IVec2(pos.x, pos.z - 1); }
        nodes = adjacentNodes;
        nodeCount = nextIdx;

        for (int i = 0; i < nextIdx; ++i)
        {
            int nodeIdx = IdxFromXZ(moves[i].x, moves[i].z);
            AStarPathNode nodeRef = mySectionNodes[nodeIdx];
            nodeRef.ReInit(endNode, MOVE_COST + currentNode.TotalCost);
            adjacentNodes[i] = nodeRef;
        }

        for (int i = nextIdx; i < adjacentNodes.Length; ++i)
        {
            adjacentNodes[i] = null;
        }
    }

    private bool GetPath(IVec2 destinaton, ref Stack<IVec2> cPath)
    {
        bool pathFound = AStarPathFinder.FindPath(mazeRef.SectionLocFor(myRigidBody.position), destinaton, MOVE_COST, FindNodes, ref cPath);
        ResetNodes();
        return pathFound;
    }
    
    private void ResetNodes()
    {
        for (int i = 0; i < mySectionNodes.Length; ++i)
        {
            mySectionNodes[i].ReInit(null, 0.0f);
            mySectionNodes[i].ReParent(null);
        }
    }
}