using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer))]
public class PlayerController : MonoBehaviour {
    public MazeScript Maze;
    public float Speed;
    public Material[] Colors;
    public Material DefaultMat;
    public float PowerupTime;

    public bool PoweredUp { get; private set; }
    private float remainingPowerTime;
    private Rigidbody myRigidBody = null;
    private Renderer myRenderer = null;
    
    // Use this for initialization
    void Start () {
        myRigidBody = GetComponent<Rigidbody>();
        myRenderer = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(horiz, 0.0f, vert);
        move.Normalize();
        myRigidBody.velocity = Speed * move;
        Maze.GenerateAround(transform.position);

        if (PoweredUp)
        {
            IVec2 dir = new IVec2((int)Mathf.Sign(horiz), (int)Mathf.Sign(vert));
            RaycastHit hit;
            if (Physics.Raycast(transform.position, move, out hit, 0.5f))
            {
                Maze.EatAt(hit.transform.position);
                hit.transform.gameObject.SetActive(false);
            }

            myRenderer.material = Colors[((int)Mathf.Floor(remainingPowerTime*25)) % Colors.Length];
            remainingPowerTime -= Time.deltaTime;
            if (remainingPowerTime <= 0.0f) { PoweredUp = false; myRenderer.material = DefaultMat; }
        }
    }

    public void PowerUp()
    {
        PoweredUp = true;
        remainingPowerTime = PowerupTime;
    }
}
