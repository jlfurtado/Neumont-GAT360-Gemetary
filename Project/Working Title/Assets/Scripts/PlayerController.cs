using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    public MazeScript Maze;
    public float Speed;

    private Rigidbody myRigidBody = null;

	// Use this for initialization
	void Start () {
        myRigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(horiz, 0.0f, vert);
        move.Normalize();
        myRigidBody.velocity = Speed * move;
        Maze.GenerateAround(transform.position);
	}
}
