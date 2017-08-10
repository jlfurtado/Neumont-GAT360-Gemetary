using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class FollowMazeSolution : Enemy {
    private int goingTo, goingFrom;
    private bool forward = true;

    // Update is called once per frame
    public override void Update() {
        if (PauseManager.OnlyOne.Paused()) { CeaseMovement(); return; }
        next = mazeSection.MazeSolution[goingTo];
        from = mazeSection.MazeSolution[goingFrom];

        base.Update();
	}

    protected override void OnLandReturn(Vector3 toPos)
    {
        // snap
        base.OnLandReturn(toPos);

        forward = false;

        if (goingFrom < goingTo)
        {
            int t = goingTo;
            goingTo = goingFrom;
            goingFrom = t;
        }
        else
        {
            --goingFrom;
            --goingTo;

            if (goingTo < 0)
            {
                forward = true;
                goingFrom = 0;
                goingTo = 1;
            }
        }
    }

    protected override void OnLand(Vector3 toPos)
    {
        // snap :)
        base.OnLand(toPos);

        if (forward) { ++goingFrom; ++goingTo; }
        else { --goingFrom; --goingTo; }

        if (goingTo >= mazeSection.MazeSolution.Length)
        {
            forward = false;
            goingFrom = mazeSection.MazeSolution.Length - 1;
            goingTo = goingFrom - 1;
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
