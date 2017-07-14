using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer), typeof(Collider))]
public class DepthFirstExplore : Enemy {
    private Stack<IVec2> explore = new Stack<IVec2>();
    private bool[,] visited = null;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        visited = new bool[mazeRef.SectionSize, mazeRef.SectionSize];
    }

    // Update is called once per frame
    public override void Update()
    {
        if (explore.Count == 0)
        {
            next = mazeRef.SectionLocFor(transform.position);
            from = mazeRef.SectionLocFor(transform.position);
            ClearVisited();
            visited[from.x, from.z] = true;
            explore.Push(from);
        }

        base.Update();
    }
    
    protected override void OnLand(Vector3 toPos)
    {
        // arrived at a section
        int nextIdx = 0;
        IVec2[] moves = new IVec2[4];

        // we're at the new next
        IVec2 current = next;

        // get moves
        if (current.x + 2 < MazeSectionGenerator.Size && !mazeSection.IsWall(current.x + 1, current.z) && !mazeSection.IsWall(current.x + 2, current.z) && !visited[current.x + 2, current.z]) { moves[nextIdx++] = new IVec2(current.x + 2, current.z); }
        if (current.x - 2 >= 0 && !mazeSection.IsWall(current.x - 1, current.z) && !mazeSection.IsWall(current.x - 2, current.z) && !visited[current.x - 2, current.z]) { moves[nextIdx++] = new IVec2(current.x - 2, current.z); }
        if (current.z + 2 < MazeSectionGenerator.Size && !mazeSection.IsWall(current.x, current.z + 1) && !mazeSection.IsWall(current.x, current.z + 2) && !visited[current.x, current.z + 2]) { moves[nextIdx++] = new IVec2(current.x, current.z + 2); }
        if (current.z - 2 >= 0 && !mazeSection.IsWall(current.x, current.z - 1) && !mazeSection.IsWall(current.x, current.z - 2) && !visited[current.x, current.z - 2]) { moves[nextIdx++] = new IVec2(current.x, current.z - 2); }

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

    public override void UpdateRef(MazeSectionGenerator mazeSection)
    {
        base.UpdateRef(mazeSection);
        if (explore != null && explore.Count > 0) { explore.Clear(); }
    }
}
