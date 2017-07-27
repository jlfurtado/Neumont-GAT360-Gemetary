using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class AStarPathNode
{
    #region Declarations

    public AStarPathNode ParentNode { get; private set; }
    public AStarPathNode EndNode { get; private set; }
    public IVec2 Location { get; private set; }
    public float TotalCost { get; private set; }
    public float DirectCost { get; private set; }

    #endregion

    #region Constructor

    public AStarPathNode(AStarPathNode parentNode, AStarPathNode endNode, IVec2 location, float cost)
    {
        ParentNode = parentNode;
        EndNode = endNode;
        Location = new IVec2((int)location.x, (int)location.z);
        DirectCost = cost;

        if (endNode != null)
        {
            TotalCost = DirectCost + CalculateLinearCost();
        }
    }

    #endregion

    #region Helper Methods

    private float CalculateLinearCost()
    {
        return EndNode.Location.DistanceTo(Location);
    }

    #endregion

    #region Public Methods

    public bool IsEqualToNode(AStarPathNode other)
    {
        return (other.Location.Equals(Location));
    }

    #endregion

}

static class AStarPathFinder
{
    enum AStarPathNodeStatus
    {
        Open,
        Closed
    }

    private static List<AStarPathNode> openList = new List<AStarPathNode>();
    private static Dictionary<IVec2, AStarPathNodeStatus> statusTracker = new Dictionary<IVec2, AStarPathNodeStatus>();
    private static Dictionary<IVec2, float> costTracker = new Dictionary<IVec2, float>();

    private static void AddNodeToOpenList(AStarPathNode node)
    {
        int index = 0;
        float cost = node.TotalCost;

        while (openList.Count > index && cost < openList[index].TotalCost)
        {
            index += 1;
        }

        openList.Insert(index, node);
        costTracker[node.Location] = node.TotalCost;
        statusTracker[node.Location] = AStarPathNodeStatus.Open;
    }

    #region Public Methods

    public delegate void AdjacentNodes(AStarPathNode currentNode, AStarPathNode endNode, out AStarPathNode[] adjacentNodes);

    public static bool FindPath(IVec2 startSquare, IVec2 endSquare, float moveCost, AdjacentNodes nodeFinder, ref Stack<IVec2> path)
    {
        path.Clear();
        if (startSquare.Equals(endSquare))
        {
            return false;
        }

        openList.Clear();
        costTracker.Clear();
        statusTracker.Clear();

        AStarPathNode end = new AStarPathNode(null, null, endSquare, moveCost);
        AStarPathNode start = new AStarPathNode(null, end, startSquare, moveCost);

        AddNodeToOpenList(start);

        while (openList.Count > 0)
        {
            AStarPathNode currentNode = openList[openList.Count - 1];

            if (currentNode.IsEqualToNode(end))
            {

                while (currentNode.ParentNode != null)
                {
                    path.Push(currentNode.Location);
                    currentNode = currentNode.ParentNode;
                }

                return true;
            }

            openList.Remove(currentNode);
            costTracker.Remove(currentNode.Location);

            AStarPathNode[] nodes;
            nodeFinder(currentNode, end, out nodes);
            foreach (AStarPathNode node in nodes)
            {
                if (statusTracker.ContainsKey(node.Location))
                {
                    if (statusTracker[node.Location] == AStarPathNodeStatus.Closed) { continue; }

                    if (statusTracker[node.Location] == AStarPathNodeStatus.Open)
                    {
                        if (node.TotalCost >= costTracker[node.Location]) { continue; }
                    }
                }

                AddNodeToOpenList(node);
            }

            statusTracker[currentNode.Location] = AStarPathNodeStatus.Closed;
        }

        return false;
    }

    #endregion
}