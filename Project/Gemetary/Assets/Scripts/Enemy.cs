using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(AudioSource))]
public class Enemy : MonoBehaviour {
    public GameObject GhostParticlePrefab;
    public int Value;
    public float SlerpRate = 10.0f;
    public float Speed = 1.0f;
    public float BaseSpeed = 1.0f;
    public Material NormalMat;
    public Material StoppedMat;
    public Material EatenMat;

    private float stopTime = 0.0f;
    private bool stopped = false;
    protected bool Eaten { get; private set; }
    protected MazeScript mazeRef;
    protected MazeSectionGenerator mazeSection;
    protected Rigidbody myRigidBody;
    protected Renderer[] myRenderers;
    protected ScoreManager scoreRef;
    protected static System.Random rand = new System.Random();
    protected IVec2 next, from;
    protected const float CLOSE_ENOUGH = 0.1f;
    protected const float PAST = 0.01f;
    protected IVec2 home;
    protected PlayerController playerRef = null;
    protected float SpeedMult = 1.0f;
    protected Animator myAnimator;
    protected Animation myAnim;
    protected AudioSource myAudioSFX;
    protected GameObject myGhostTrail;

    // Use this for initialization
    public virtual void Awake() {
        Eaten = false;
        myAudioSFX = GetComponent<AudioSource>();

        myGhostTrail = Instantiate(GhostParticlePrefab);
        myGhostTrail.transform.parent = transform;
        myGhostTrail.name = "GhostParticles";
        myGhostTrail.SetActive(false);

        AudioHelper.InitSFX(myAudioSFX);

        myAnimator = GetComponent<Animator>();
        myAnim = GetComponent<Animation>();
        playerRef = GameObject.FindGameObjectWithTag(Strings.PLAYER_TAG).GetComponent<PlayerController>();
        mazeRef = GameObject.FindGameObjectWithTag(Strings.MAZE_TAG).GetComponent<MazeScript>();
        myRigidBody = GetComponent<Rigidbody>();
        myRenderers = GetComponentsInChildren<Renderer>();
        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();
    }

    protected int IdxFromXZ(int x, int z)
    {
        return x * mazeRef.SectionSize + z;
    }

    protected bool IsNode(IVec2 pos)
    {
        return (pos.x & pos.z & 1) != 0;
    }

    protected IVec2 ForceToBeNode(IVec2 pos)
    {
        if ((pos.x & pos.z & 1) == 0)
        {
            if (pos.x + 1 < MazeSectionGenerator.Size && !mazeSection.IsWall(pos.x + 1, pos.z)) { pos.x++; }
            else if (pos.x - 1 >= 0 && !mazeSection.IsWall(pos.x - 1, pos.z)) { pos.x--; }
            else if (pos.z + 1 < MazeSectionGenerator.Size && !mazeSection.IsWall(pos.x, pos.z + 1)) { pos.z++; }
            else if (pos.z - 1 >= 0 && !mazeSection.IsWall(pos.x, pos.z - 1)) { pos.z--; }
            // TODO: ASSUMES ONE UP DOWN LEFT OR RIGHT MOVE FROM NODE!!! DIAGONAL COMPUTATIONS COMPLEX IMPLEMENT IF NEEDED
        }

        return pos;
    }

    public IVec2 GetPos()
    {
        return mazeRef.SectionLocFor(myRigidBody.position);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (PauseManager.OnlyOne.Paused())
        {
            CeaseMovement();
            return;
        }

        if (stopped)
        {
            stopTime -= Time.deltaTime;
            SetMat(Mathf.Sqrt(stopTime) * 100 % 7 < 2 ? NormalMat : StoppedMat);
            if (stopTime < 0.0f)
            {
                UnStop();
            }
        }

        Vector3 tp = mazeSection.PositionAt(next), fp = mazeSection.PositionAt(from);
        Vector3 toPos = new Vector3(tp.x, myRigidBody.position.y, tp.z), fromPos = new Vector3(fp.x, myRigidBody.position.y, fp.z);
        Vector3 moving = toPos - myRigidBody.position;

        if (Vector3.Dot((moving).normalized, ((toPos - fromPos).normalized)) < PAST || (moving).magnitude < CLOSE_ENOUGH)
        {
            if (Eaten && next.Equals(home)) { Restore(); }
            if (stopped && next.Equals(home)) { UnStop(); }
            if (stopped || Eaten) { OnLandReturn(toPos); }
            else { OnLand(toPos); }
        }
        else
        {
            Move(toPos, fromPos);
        }
    }

    protected virtual void OnLand(Vector3 toPos)
    {
        myRigidBody.position = toPos;
        myRigidBody.velocity = Vector3.zero;
    }

    protected virtual void OnLandReturn(Vector3 toPos)
    {
        myRigidBody.position = toPos;
        myRigidBody.velocity = Vector3.zero;
    }

    protected void CeaseMovement()
    {
        myRigidBody.velocity = Vector3.zero;
        Animate(false);
    }

    private void Move(Vector3 toPos, Vector3 fromPos)
    {
        Vector3 vel = toPos - fromPos;
        myRigidBody.velocity = vel.normalized * (Speed * SpeedMult);
        myRigidBody.rotation = Quaternion.Slerp(myRigidBody.rotation, Quaternion.LookRotation(vel.normalized), Time.deltaTime * SlerpRate);
        Animate(true);
    }

    private void Animate(bool doIt)
    {
        if (myAnim != null) { myAnim.enabled = doIt; myAnim[Strings.ENEMY_WALK_ANIM_NAME].speed = (Speed * SpeedMult * 0.6f); }
        if (myAnimator != null) { myAnimator.enabled = doIt; myAnimator.speed = (Speed * SpeedMult * 0.5f); }
    }

    protected void UnStop()
    {
        stopTime = 0.0f;
        stopped = false;
        SpeedMult = 1.0f;
        SetMat(NormalMat);
    }

    protected void EatMe()
    {
        AudioHelper.PlaySFX(myAudioSFX);
        Eaten = true;
        UnStop();
        SetMat(EatenMat);
        myGhostTrail.SetActive(true);
    }

    protected void Restore()
    {
        Eaten = false;
        SpeedMult = 1.0f;
        SetMat(NormalMat);
        myGhostTrail.SetActive(false);
    }

    public void BlowMeUp()
    {
        // oh no, we died!
        scoreRef.AddScore((int)Mathf.Floor(Value * Speed));
        EatMe();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Strings.PLAYER_TAG) && !Eaten)
        {
            // only get comp if we hit the player
            if (playerRef.PoweredUp || stopped)
            {
                // oh no, we died!
                scoreRef.AddScore((int)Mathf.Floor(Value * Speed));
                EatMe();
            }
            else if (!playerRef.Dodging)
            {
                // only move to game over if they aren't powered up
                playerRef.Die();
            }
        }
    }

    public void StopFor(float time)
    {
        if (!Eaten)
        {
            stopped = true;
            stopTime = time;
            SpeedMult = 0.66f;
            SetMat(StoppedMat);
            myRigidBody.velocity = Vector3.zero;
        }
    }

    public virtual void UpdateRef(MazeSectionGenerator mazeSection)
    {
        this.mazeSection = mazeSection;
        stopped = false;
        stopTime = 0.0f;
        Eaten = false;
        SpeedMult = 1.0f;

        home = mazeSection.MazeSolution[0];

        if (myRenderers != null)
        {
            SetMat(NormalMat);
            if (Eaten) { Restore(); }
            if (stopped) { UnStop(); }
        } 

        if (myRigidBody != null) { myRigidBody.velocity = Vector3.zero; myRigidBody.position = mazeSection.PositionAt(home); }
    }

    protected void SetMat(Material mat)
    {
        foreach (Renderer renderer in myRenderers)
        {
            renderer.material = mat;
        }
    }
}
