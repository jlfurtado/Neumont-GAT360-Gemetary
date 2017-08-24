using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Renderer))]
public class EatForPoints : MonoBehaviour {
    public int EnduranceRegen;
    public float Speed = 5.0f;
    public Vector3 offset = 25.5f * Vector3.up;
    public float SlowdownRadius = 5.0f;
    public int Value;
    public IVec2 mazeLoc;
    public IVec2 sectionLoc;
    private ScoreManager scoreRef;
    private MazeScript maze;
    private Renderer myRenderer;
    private Material storeMat;
    private PlayerController playerRef;
    private AudioSource gemSFX;
    public bool Collected { get; private set; }
    private Vector3 startScale;
    private Rigidbody myRigidBody;
    private GemCollecterOptimizerThing collectorOptimizer;
    private float startZ;
    
    //private Collider myCollider;
	// Use this for initialization
	void Awake() {
        startScale = transform.localScale;
        myRigidBody = GetComponent<Rigidbody>();
        collectorOptimizer = GameObject.FindGameObjectWithTag(Strings.GEM_OPTIMIZER_COLLECTOR_TAG).GetComponent<GemCollecterOptimizerThing>();
        gemSFX = GameObject.FindGameObjectWithTag(Strings.GEM_SFX_TAG).GetComponent<AudioSource>();
        playerRef = GameObject.FindGameObjectWithTag(Strings.PLAYER_TAG).GetComponent<PlayerController>();
        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();
        maze = GameObject.FindGameObjectWithTag(Strings.MAZE_TAG).GetComponent<MazeScript>();
        myRenderer = GetComponent<Renderer>();
        if (storeMat != null) { myRenderer.material = storeMat; }
	}

    public void FlyTo(Camera c, Vector3 destination)
    {
        Vector3 myScreenPoint = c.WorldToScreenPoint(transform.position);

        Vector3 move = (destination - myScreenPoint);
        float dist = move.magnitude;
        if (dist > 0.0f)
        {
            float rampSpeed = Speed * (dist / SlowdownRadius);
            float clipSpeed = Mathf.Min(rampSpeed, Speed);
            Vector3 vel = (clipSpeed / dist) * move;
            myScreenPoint += vel * Time.deltaTime;

            transform.position = c.ScreenToWorldPoint(myScreenPoint);

            float f = 1.0f - (myScreenPoint.z - c.nearClipPlane) / startZ;

            transform.localScale = Vector3.Lerp(startScale, Vector3.one, f);

            if (move.magnitude < 10.0f)
            {
                Collected = false;
                gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Strings.PLAYER_TAG) && scoreRef != null && !playerRef.PlayerDead && !Collected)
        {
            AudioHelper.PlaySFX(gemSFX);
            playerRef.AddEndurance(EnduranceRegen);
            scoreRef.AddScore(Value);
            maze.EatAt(mazeLoc, sectionLoc);
            Collected = true;
            myRigidBody.detectCollisions = false;
            startZ = collectorOptimizer.BeginFlyGem(this);
         }
    }

    public void ResetMe()
    {
        Collected = false;
        transform.localScale = startScale;
        myRigidBody.detectCollisions = true;
    }

    public void SetMat(Material mat)
    {
        if (myRenderer != null) { myRenderer.material = mat; }
        else { storeMat = mat; }
    }
}
