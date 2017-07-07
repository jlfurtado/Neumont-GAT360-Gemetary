using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FollowMazeSolution : MonoBehaviour {
    public float speed = 1.0f;
    private MazeSectionGenerator mazeSection;
    private int goingTo, goingFrom;
    private bool forward = true;
    private const float CLOSE_ENOUGH = 0.25f;
    private Rigidbody myRigidBody;

	// Use this for initialization
	void Start () {
        myRigidBody = GetComponent<Rigidbody>();
        myRigidBody.velocity = Vector3.zero;
	}

    // Update is called once per frame
    void Update() {
        IVec2 to = mazeSection.MazeSolution[goingTo], from = mazeSection.MazeSolution[goingFrom];
        Vector3 tp = mazeSection.PositionAt(to), fp = mazeSection.PositionAt(from);
        Vector3 toPos = new Vector3(tp.x, transform.position.y, tp.z), fromPos = new Vector3(fp.x, transform.position.y, fp.z);

        if ((transform.position - toPos).magnitude < CLOSE_ENOUGH)
        {
            if (forward) { ++goingFrom; ++goingTo; }
            else { --goingFrom; --goingTo; }

            if (goingTo >= mazeSection.MazeSolution.Length)
            {
                forward = false;
                goingFrom = mazeSection.MazeSolution.Length - 1;
                goingTo = goingFrom - 1;
                myRigidBody.velocity = Vector3.zero;
            }
            else if (goingTo < 0)
            {
                forward = true;
                goingFrom = 0;
                goingTo = 1;
            }
        }
        else
        {
            Vector3 vel = toPos - fromPos;
            myRigidBody.velocity = vel.normalized * speed;
        }
	}

    public void UpdateRef(MazeSectionGenerator mazeSection)
    {
        this.mazeSection = mazeSection;
        forward = true;
        goingTo = 1;
        goingFrom = 0;
        if (myRigidBody != null) { myRigidBody.velocity = Vector3.zero; }
    }
}
