using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer), typeof(Collider))]
public class FollowMazeSolution : Enemy {
    private int goingTo, goingFrom;
    private bool forward = true;

    // Update is called once per frame
    public override void Update() {
        next = mazeSection.MazeSolution[goingTo];
        from = mazeSection.MazeSolution[goingFrom];

        base.Update();
	}

    protected override void OnLand(Vector3 toPos)
    {
        if (forward) { ++goingFrom; ++goingTo; }
        else { --goingFrom; --goingTo; }

        if (goingTo >= mazeSection.MazeSolution.Length)
        {
            forward = false;
            goingFrom = mazeSection.MazeSolution.Length - 1;
            goingTo = goingFrom - 1;
            myRigidBody.position = toPos;
            myRigidBody.velocity = Vector3.zero;
        }
        else if (goingTo < 0)
        {
            forward = true;
            goingFrom = 0;
            goingTo = 1;
        }
    }

    public override void UpdateRef(MazeSectionGenerator mazeSection)
    {
        base.UpdateRef(mazeSection);
        forward = true;
        goingTo = 1;
        goingFrom = 0;
    }
}
