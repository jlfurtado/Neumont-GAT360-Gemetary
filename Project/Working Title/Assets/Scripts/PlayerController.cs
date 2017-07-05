using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public MazeScript maze;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        transform.localPosition += new Vector3(horiz, 0.0f, vert);
        maze.GenerateAround(transform.position, maze.Radius);
	}
}
