using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer), typeof(Collider))]
public class FollowMazeSolution : MonoBehaviour {
    public int Value;
    public float Speed = 1.0f;
    public Material NormalMat;
    public Material StoppedMat;
    private MazeSectionGenerator mazeSection;
    private int goingTo, goingFrom;
    private bool forward = true;
    private const float CLOSE_ENOUGH = 0.1f;
    private const float PAST = 0.01f;
    private Rigidbody myRigidBody;
    private Renderer myRenderer;
    private SceneMover sceneMoverRef;
    private ScoreManager scoreRef;
    private float stopTime = 0.0f;
    private bool stopped = false;

    // Use this for initialization
    void Start () {
        myRigidBody = GetComponent<Rigidbody>();
        myRigidBody.velocity = Vector3.zero;
        myRenderer = GetComponent<Renderer>();
        sceneMoverRef = GameObject.FindGameObjectWithTag(Strings.SCENE_MOVER_TAG).GetComponent<SceneMover>();
        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();

    }

    // Update is called once per frame
    void Update() {
        IVec2 to = mazeSection.MazeSolution[goingTo], from = mazeSection.MazeSolution[goingFrom];
        Vector3 tp = mazeSection.PositionAt(to), fp = mazeSection.PositionAt(from);
        Vector3 toPos = new Vector3(tp.x, transform.position.y, tp.z), fromPos = new Vector3(fp.x, transform.position.y, fp.z);
        Vector3 moving = toPos - transform.position;

        if (stopped)
        {
            stopTime -= Time.deltaTime;
            myRenderer.material = Mathf.Sqrt(stopTime) * 100 % 7 < 2 ? NormalMat : StoppedMat;
            if (stopTime < 0.0f)
            {
                stopTime = 0.0f;
                stopped = false;
                myRenderer.material = NormalMat;
            }
        }
        else
        {
            if (Vector3.Dot((moving).normalized, ((toPos - fromPos).normalized)) < PAST || (moving).magnitude < CLOSE_ENOUGH)
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
                myRigidBody.velocity = vel.normalized * Speed;
            }
        }

	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Strings.PLAYER_TAG))
        {
            // only get comp if we hit the player
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            if (pc.PoweredUp || stopped)
            {
                // oh no, we died!
                scoreRef.AddScore(Value);
                gameObject.SetActive(false);
            }
            else
            {
                // only move to game over if they aren't powered up
                sceneMoverRef.MoveToGameOver();
            }
        }
    }

    public void StopFor(float time)
    {
        stopped = true;
        stopTime = time;
        myRenderer.material = StoppedMat;
        myRigidBody.velocity = Vector3.zero;
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
