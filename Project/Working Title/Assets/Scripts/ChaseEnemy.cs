using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer), typeof(Collider))]
public class ChaseEnemy : Enemy
{
    public int PlayerFollowMoves;
    public int FriendFollowMoves;
    private Stack<IVec2> path = null;
    private Stack<IVec2> playerPath = null, friendOnePath = null, friendTwoPath = null;
    private PlayerController playerRef = null;
    private const float MOVE_COST = 10.0f;
    private DepthFirstExplore myFriendOne = null;
    private FollowMazeSolution myFriendTwo = null;

    // Use this for initialization
    public override void Start()
    {
        base.Start(); // set base refs
        playerRef = GameObject.FindGameObjectWithTag(Strings.PLAYER_TAG).GetComponent<PlayerController>();
        path = new Stack<IVec2>(mazeRef.SectionSize * mazeRef.SectionSize);
        playerPath = new Stack<IVec2>(mazeRef.SectionSize * mazeRef.SectionSize);
        friendOnePath = new Stack<IVec2>(mazeRef.SectionSize * mazeRef.SectionSize);
        friendTwoPath = new Stack<IVec2>(mazeRef.SectionSize * mazeRef.SectionSize);

        if (mazeRef != null)
        {
            next = mazeRef.SectionLocFor(myRigidBody.position);
            from = mazeRef.SectionLocFor(myRigidBody.position);
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        // updates if necessary otherwise remove override

        base.Update();
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
        GetPath(myFriendOne.GetPos(), ref friendOnePath);
        GetPath(myFriendTwo.GetPos(), ref friendTwoPath);
        path = (playerFound && playerPath.Count <= friendTwoPath.Count) ? (playerPath.Count <= friendOnePath.Count ? playerPath : friendOnePath)
                                                                       : (friendTwoPath.Count <= friendOnePath.Count ? friendTwoPath : friendOnePath);

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

        myFriendOne = mazeSection.DepthEnemyPool.reference[mazeSection.DepthEnemyPool.start];
        myFriendTwo = mazeSection.FollowSolutionPool.reference[mazeSection.FollowSolutionPool.start];
    }

    private void FindNodes(AStarPathNode currentNode, AStarPathNode endNode, out AStarPathNode[] nodes)
    {
        // arrived at a section
        int nextIdx = 0;
        IVec2[] moves = new IVec2[4];
        IVec2 pos = currentNode.Location;

        // get moves
        if (pos.x + 1 < MazeSectionGenerator.Size && !mazeSection.IsWall(pos.x + 1, pos.z)) { moves[nextIdx++] = new IVec2(pos.x + 1, pos.z); }
        if (pos.x - 1 >= 0 && !mazeSection.IsWall(pos.x - 1, pos.z)) { moves[nextIdx++] = new IVec2(pos.x - 1, pos.z); }
        if (pos.z + 1 < MazeSectionGenerator.Size && !mazeSection.IsWall(pos.x, pos.z + 1)) { moves[nextIdx++] = new IVec2(pos.x, pos.z + 1); }
        if (pos.z - 1 >= 0 && !mazeSection.IsWall(pos.x, pos.z - 1)) { moves[nextIdx++] = new IVec2(pos.x, pos.z - 1); }
        nodes = new AStarPathNode[nextIdx];

        for (int i = 0; i < nextIdx; ++i)
        {
            nodes[i] = new AStarPathNode(currentNode, endNode, moves[i], MOVE_COST + currentNode.TotalCost);
        }
    }

    private bool GetPath(IVec2 destinaton, ref Stack<IVec2> cPath)
    {
        return AStarPathFinder.FindPath(mazeRef.SectionLocFor(myRigidBody.position), destinaton, MOVE_COST, FindNodes, ref cPath);
    }
}
