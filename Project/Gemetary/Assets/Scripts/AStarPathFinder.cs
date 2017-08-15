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
        Location = location;
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

    public void ReInit(AStarPathNode endNode, float cost)
    {
        EndNode = endNode;
        DirectCost = cost;

        if (endNode != null)
        {
            TotalCost = DirectCost + CalculateLinearCost();
        }
    }

    public void ReLocate(IVec2 location)
    {
        Location = location;
    }

    public void ReParent(AStarPathNode parent)
    {
        ParentNode = parent;
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

    private static List<AStarPathNode> openList = new List<AStarPathNode>(10000);
    private static Dictionary<IVec2, AStarPathNodeStatus> statusTracker = new Dictionary<IVec2, AStarPathNodeStatus>(10000);
    private static Dictionary<IVec2, float> costTracker = new Dictionary<IVec2, float>(10000);
    private static AStarPathNode startNode = new AStarPathNode(null, null, new IVec2(), 0.0f);
    private static AStarPathNode endNode = new AStarPathNode(null, null, new IVec2(), 0.0f);
     
    private static void AddNodeToOpenList(AStarPathNode node)
    {
        int index = 0;
        float cost = node.TotalCost;

        while (openList.Count > index && cost < openList[index].TotalCost) { ++index; }

        openList.Insert(index, node);
        costTracker[node.Location] = node.TotalCost;
        statusTracker[node.Location] = AStarPathNodeStatus.Open;
    }

    #region Public Methods

    public delegate void AdjacentNodes(AStarPathNode currentNode, AStarPathNode endNode, out AStarPathNode[] adjacentNodes, out int count);

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

        // clear start/end
        startNode.ReParent(null);
        endNode.ReParent(null);

        startNode.ReLocate(startSquare);
        endNode.ReLocate(endSquare);

        startNode.ReInit(endNode, moveCost);
        endNode.ReInit(null, moveCost);

        AddNodeToOpenList(startNode);

        while (openList.Count > 0)
        {
            AStarPathNode currentNode = openList[openList.Count - 1];

            if (currentNode.IsEqualToNode(endNode))
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
            int nodeCount;
            nodeFinder(currentNode, endNode, out nodes, out nodeCount);
            for (int i = 0; i < nodeCount; ++i)
            {
                IVec2 location = nodes[i].Location;
                if (statusTracker.ContainsKey(location))
                {
                    if (statusTracker[location] == AStarPathNodeStatus.Closed) { continue; }

                    if (statusTracker[location] == AStarPathNodeStatus.Open)
                    {
                        if (nodes[i].TotalCost >= costTracker[location]) { continue; }
                    }
                }

                nodes[i].ReParent(currentNode);
                AddNodeToOpenList(nodes[i]);
            }

            statusTracker[currentNode.Location] = AStarPathNodeStatus.Closed;
        }

        return false;
    }

    #endregion
}