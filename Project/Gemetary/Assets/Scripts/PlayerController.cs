using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    public float Speed;
    public float SlerpRate = 10.0f;
    public GameObject PowerupParticlePrefab;
    public Material[] Colors;
    public float PowerupTime;
    public float DodgeTime;
    public float JumpHeight;
    public AudioSource GameBackgroundMusic;
    public AudioSource PowerupBackgroundMusic;
    public AudioSource PlayerDeadSFX;
    private MazeScript mazeRef;
    private const float CLOSE_ENOUGH = 0.1f;
    private const float PAST = 0.01f;
    private Enemy[] enemies;
    private bool playerDead;

    public bool PoweredUp { get; private set; }
    public bool Dodging { get; private set; }
    private float remainingPowerTime;
    private Rigidbody myRigidBody = null;
    private Renderer[] myRenderers = null;
    private bool moving = false;
    private Vector3 fromPos, toPos;
    private bool horizLast;
    private float dodgeTime;
    private int endurance = 0;
    private const int MAX_ENDURANCE = 100;
    private const int ENDURANCE_COST = 50;
    private GameObject myParticles;
    private Animator myAnimator;
    private Material[][] baseMaterials;
    private Material[] tempMats;
    private HintText hinter;
    private bool dodgeHinted = false;
    private bool powerupHinted = false;
    private PauseMenu pauseMenuRef = null;
    private AxisInputHelper axisInput = null;
    private SceneMover sceneMoverRef = null;

    private void Triggered(string trigger)
    {
        myAnimator.SetTrigger(trigger);
    }

    // Use this for initialization
    void Awake() {
        playerDead = false;
        sceneMoverRef = GameObject.FindGameObjectWithTag(Strings.SCENE_MOVER_TAG).GetComponent<SceneMover>();
        axisInput = GameObject.FindGameObjectWithTag(Strings.AXIS_INPUT_HELPER_TAG).GetComponent<AxisInputHelper>();
        pauseMenuRef = GameObject.FindGameObjectWithTag(Strings.PAUSE_MENU_TAG).GetComponent<PauseMenu>();
        hinter = GameObject.FindGameObjectWithTag(Strings.HINTER_TAG).GetComponent<HintText>();
        myAnimator = GetComponentInChildren<Animator>();
        myRigidBody = GetComponent<Rigidbody>();
        myRenderers = GetComponentsInChildren<Renderer>();
        mazeRef = GameObject.FindGameObjectWithTag(Strings.MAZE_TAG).GetComponent<MazeScript>();
        myParticles = Instantiate(PowerupParticlePrefab);
        myParticles.SetActive(false);
        myParticles.transform.parent = gameObject.transform;
        myParticles.transform.localPosition = Vector3.up * 1.5f;
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag(Strings.ENEMY_TAG);

        enemies = new Enemy[enemyObjects.Length];
        for (int i = 0; i < enemyObjects.Length; ++i)
        {
            enemies[i] = enemyObjects[i].GetComponent<Enemy>();
        }

        baseMaterials = new Material[myRenderers.Length][];
        int max = 0;
        for (int i = 0; i < myRenderers.Length; ++i)
        {
            baseMaterials[i] = new Material[myRenderers[i].materials.Length];

            for (int j = 0; j < baseMaterials[i].Length; ++j)
            {
                baseMaterials[i][j] = myRenderers[i].materials[j];
                if (j > max) { max = j; }
            }
        }

        tempMats = new Material[max];
    }

    void Start()
    {
        if (ScoreManager.IsCheating())
        {
            PowerupTime = int.MaxValue / 2.0f;
            PowerUp();
        }
    }

    private void RestoreMaterials()
    {
        for (int i = 0; i < myRenderers.Length; ++i)
        {
            for (int j = 0; j < baseMaterials[i].Length; ++j)
            {
                myRenderers[i].materials = baseMaterials[i];
            }
        }
    }

    private void SetMaterials(Material mat)
    {
        for (int i = 0; i < tempMats.Length; ++i)
        {
            tempMats[i] = mat;
        }

        for (int j = 0; j < myRenderers.Length; ++j)
        {
            myRenderers[j].materials = tempMats;
        }
    }

	// Update is called once per frame
	void Update () {
        if (PauseManager.OnlyOne.Paused() || playerDead) { myRigidBody.velocity = Vector3.zero; return; }

        float horiz = Input.GetAxis(Strings.HORIZ_AXIS_NAME);
        float vert = Input.GetAxis(Strings.VERT_AXIS_NAME);
        int h = Mathf.RoundToInt(horiz);
        int v = Mathf.RoundToInt(vert);

        IVec2 dir = new IVec2(h, v);

        if (!moving && (dir.x != 0 || dir.z != 0))
        {
            Vector3 toHoriz = new Vector3(myRigidBody.position.x + dir.x, myRigidBody.position.y, myRigidBody.position.z);
            Vector3 toVert = new Vector3(myRigidBody.position.x, myRigidBody.position.y, myRigidBody.position.z + dir.z);

            MazeSectionGenerator toSectionH = mazeRef.SectionAt(toHoriz), toSectionV = mazeRef.SectionAt(toVert);
            MazeSectionGenerator fromSection = mazeRef.SectionAt(myRigidBody.position);
            IVec2 fromLoc = mazeRef.SectionLocFor(myRigidBody.position);
            IVec2 toLocH = mazeRef.SectionLocFor(toHoriz), toLocV = mazeRef.SectionLocFor(toVert);

            Vector3 tph = toSectionH.PositionAt(toLocH), tpv = toSectionV.PositionAt(toLocV), fp = fromSection.PositionAt(fromLoc);
            Vector3 fromPos = new Vector3(fp.x, myRigidBody.position.y, fp.z);

            Vector3 toPos = fromPos;
            if (horizLast)
            {
                if (toSectionH.IsWalkable(toLocH) && dir.x != 0) { toPos = new Vector3(tph.x, myRigidBody.position.y, tph.z); horizLast = true; }
                if (toSectionV.IsWalkable(toLocV) && dir.z != 0) { toPos = new Vector3(tpv.x, myRigidBody.position.y, tpv.z); horizLast = false; }
            }
            else
            {
                if (toSectionV.IsWalkable(toLocV) && dir.z != 0) { toPos = new Vector3(tpv.x, myRigidBody.position.y, tpv.z); horizLast = false; }
                if (toSectionH.IsWalkable(toLocH) && dir.x != 0) { toPos = new Vector3(tph.x, myRigidBody.position.y, tph.z); horizLast = true; }
            }            

            moving = true;
            this.fromPos = fromPos;
            this.toPos = toPos;
            if (!Dodging) { Triggered(Strings.BEGIN_MOVE_ANIM); }

        }

        if (moving)
        {
            Vector3 vel = toPos - fromPos;
            myRigidBody.velocity = vel.normalized * Speed;
            if (vel.sqrMagnitude > 0.0f)
            {
                myRigidBody.rotation = Quaternion.Slerp(myRigidBody.rotation, Quaternion.LookRotation(vel.normalized), Time.deltaTime * SlerpRate);
            }
        }

        if (moving && Vector3.Dot((toPos - myRigidBody.position).normalized, ((toPos - fromPos).normalized)) < PAST || (toPos - myRigidBody.position).magnitude < CLOSE_ENOUGH)
        {
            // do something on land!?!?!
            myRigidBody.velocity = Vector3.zero;
            myRigidBody.position = toPos;
            moving = false;
            if (!Dodging && (dir.x == 0 && dir.z == 0)) { Triggered(Strings.END_MOVE_ANIM); }
        }

        if (PoweredUp)
        {
            RaycastHit hit;
            if (Physics.Raycast(myRigidBody.position + (Vector3.up * 0.5f), new Vector3(dir.x, 0.0f, dir.z), out hit, 1.0f))
            {
                mazeRef.EatAt(hit.transform.position);
                hit.transform.gameObject.SetActive(false);
            }

            SetMaterials(Colors[(Mathf.FloorToInt(Mathf.Sqrt(remainingPowerTime) * 250.0f)) % Colors.Length]);
            remainingPowerTime -= Time.deltaTime;
            if (remainingPowerTime <= 0.0f) { Restore(); }
        }
        else if (Dodging)
        {
            dodgeTime -= Time.deltaTime;
            float dtOverTwo = DodgeTime / 2.0f;
            float hPerc = (dodgeTime > dtOverTwo ? (-(dodgeTime / dtOverTwo) + 2.0f) : (dodgeTime / dtOverTwo));
            myRigidBody.position = new Vector3(myRigidBody.position.x, JumpHeight * hPerc, myRigidBody.position.z);
            if (dodgeTime <= 0.0f) { EndDodge(); }
        }
        else 
        {
            if (endurance >= ENDURANCE_COST && axisInput.AxisPressed(Strings.DODGE_INDEX))
            {
                Dodge();
            }
        }

        if (axisInput.AxisPressed(Strings.CANCEL_INDEX) && !pauseMenuRef.Showing && !hinter.Displaying)
        {
            pauseMenuRef.Show();
        }

        //if (Input.GetKey(KeyCode.Space))
        //{
        //    PowerUp();
        //}

        mazeRef.GenerateAround(myRigidBody.position);
    }

    public float GetEndurancePercent()
    {
        return endurance / (1.0f * MAX_ENDURANCE);
    }

    public void PowerUp()
    {
        PlayPowerMusic();
        EndDodge();
        PoweredUp = true;
        remainingPowerTime = PowerupTime;
        myParticles.SetActive(true);

        foreach (Enemy enemy in enemies)
        {
            enemy.StopFor(PowerupTime);
        }

        if (!powerupHinted)
        {
            powerupHinted = true;
            hinter.BeginHint(string.Concat(Strings.SECTION_CLEAR_HINT_PREFIX, ScoreManager.GetName(), Strings.SECTION_CLEAR_HINT_POSTFIX));
        }

    }

    public void Die()
    {
        if (!playerDead)
        {
            Triggered(Strings.DEATH_ANIM);
            myRigidBody.velocity = Vector3.zero;
            //PauseManager.OnlyOne.Pause();
            if (GameBackgroundMusic.isPlaying) { GameBackgroundMusic.Stop(); }
            if (PowerupBackgroundMusic.isPlaying) { PowerupBackgroundMusic.Stop(); }
            PlayerDeadSFX.Play();
            StartCoroutine(GoToGameOver(3.0f));
            playerDead = true;
        }
    }

    private IEnumerator GoToGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);
        sceneMoverRef.MoveToGameOver();
    }

    public void AddEndurance(int amount)
    {
        endurance = Mathf.Clamp(endurance + amount, 0, MAX_ENDURANCE);
        if (!dodgeHinted && endurance >= ENDURANCE_COST)
        {
            dodgeHinted = true;
            hinter.BeginHint(Strings.DODGE_HINT, Strings.DODGE_INDEX);
        }
    }

    public void Dodge()
    {
        endurance -= ENDURANCE_COST;
        Dodging = true;
        dodgeTime = DodgeTime;
        Triggered(Strings.JUMP_ANIM);
    }

    private void EndDodge()
    {
        Dodging = false;
        Triggered(moving ? Strings.BEGIN_MOVE_ANIM : Strings.END_MOVE_ANIM);
        myRigidBody.position = new Vector3(myRigidBody.position.x, 0.0f, myRigidBody.position.z);
        toPos = new Vector3(toPos.x, 0.0f, toPos.z);
    }

    private void Restore()
    {
        PlayGameMusic();
        PoweredUp = false;
        RestoreMaterials();
        myParticles.SetActive(false);
    }

    private void PlayGameMusic()
    {
        if (PowerupBackgroundMusic.isPlaying) { PowerupBackgroundMusic.Pause(); }
        if (!GameBackgroundMusic.isPlaying) { GameBackgroundMusic.Play(); }
    }


    private void PlayPowerMusic()
    {
        if (GameBackgroundMusic.isPlaying) { GameBackgroundMusic.Pause(); }
        if (!PowerupBackgroundMusic.isPlaying) { PowerupBackgroundMusic.Play(); }
    }

    public IVec2 GetPos()
    {
        return mazeRef.SectionLocFor(myRigidBody.position);
    }

    public float GetGemPercent()
    {
        return 1.0f - mazeRef.SectionAt(myRigidBody.position).GemPercent();
    }

    public bool EmptySection()
    {
        return mazeRef.SectionAt(myRigidBody.position).GemPercent() <= 0.0f;
    }

    public float GetPowerupPercent()
    {
        return remainingPowerTime / PowerupTime;
    }

    public Material GetCurrentMat()
    {
        return mazeRef.SectionAt(myRigidBody.position).GemMat;
    }
}
