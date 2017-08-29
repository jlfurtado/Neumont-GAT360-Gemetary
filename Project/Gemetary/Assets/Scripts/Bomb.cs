using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(Rigidbody))]
public class Bomb : MonoBehaviour {
    public int Value;
    private IVec2 mazeLoc;
    private IVec2 sectionLoc;
    public float ExplodeTime;
    public Material DefaultMat;
    public Material FlashMat;
    public GameObject ExplosionPrefab;

    private AudioSource tombstoneTick;
    private AudioSource explosionSource;
    private ScoreManager scoreRef;
    private MazeScript maze;
    private PlayerController playerRef;
    private bool exploding;
    private float flashTime;
    private Renderer myRenderer = null;
    private Enemy[] enemies;
    private Explosion explosion = null;
    private HintText hinter;
    private FollowTarget mainCamera;
    private int raycastLayers;
    public static bool hinted = false;

    // Use this for initialization
    void Awake() {
        mainCamera = GameObject.FindGameObjectWithTag(Strings.MAIN_CAMERA_TAG).GetComponent<FollowTarget>();
        tombstoneTick = GetComponent<AudioSource>();
        AudioHelper.InitSFX(tombstoneTick);

        explosionSource = GameObject.FindGameObjectWithTag(Strings.EXPLOSION_AUDIO_TAG).GetComponent<AudioSource>();
        hinter = GameObject.FindGameObjectWithTag(Strings.HINTER_TAG).GetComponent<HintText>();
        explosion = Instantiate(ExplosionPrefab).GetComponent<Explosion>();
        explosion.transform.position = transform.position;
        explosion.gameObject.SetActive(false);

        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();
        maze = GameObject.FindGameObjectWithTag(Strings.MAZE_TAG).GetComponent<MazeScript>();
        playerRef = GameObject.FindGameObjectWithTag(Strings.PLAYER_TAG).GetComponent<PlayerController>();
        myRenderer = GetComponent<Renderer>();

        myRenderer.material = DefaultMat;

        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag(Strings.ENEMY_TAG);

        enemies = new Enemy[enemyObjects.Length];
        for (int i = 0; i < enemyObjects.Length; ++i)
        {
            enemies[i] = enemyObjects[i].GetComponent<Enemy>();
        }

        raycastLayers = ~((1 << LayerMask.NameToLayer(Strings.PLAYER_LAYER)) | Physics.IgnoreRaycastLayer | (1 << LayerMask.NameToLayer(Strings.NO_COLLIDE_NO_RAYCAST_LAYER)));
    }

    void Update()
    {
        if (exploding && !PauseManager.OnlyOne.Paused())
        {
            flashTime -= Time.deltaTime;
            myRenderer.material = Mathf.Sqrt(flashTime) * 100 % 7 < 2 ? DefaultMat : FlashMat;

            if (flashTime <= 0.0f)
            {
                exploding = false;
                Explode();
            }
        }
    }
    
    private void Explode()
    {
        mainCamera.BeginShake(.75f, 7.5f);
        AudioHelper.PlaySFX(explosionSource);
        scoreRef.AddScore(Value);
        maze.EatAt(mazeLoc, sectionLoc);
        gameObject.SetActive(false);
        Vector3 checkFrom = transform.position + (0.5f * Vector3.up);
        BlowUp(checkFrom, Vector3.zero);
        BlowUp(checkFrom, Vector3.right);
        BlowUp(checkFrom, Vector3.left);
        BlowUp(checkFrom, Vector3.forward);
        BlowUp(checkFrom, Vector3.back);
        BlowUp(checkFrom, Vector3.right + Vector3.forward);
        BlowUp(checkFrom, Vector3.left + Vector3.back);
        BlowUp(checkFrom, Vector3.forward + Vector3.left);
        BlowUp(checkFrom, Vector3.back + Vector3.right);
        BlowUp(checkFrom, 2.0f * Vector3.right);
        BlowUp(checkFrom, 2.0f * Vector3.left);
        BlowUp(checkFrom, 2.0f * Vector3.forward);
        BlowUp(checkFrom, 2.0f * Vector3.back);

        explosion.Explode();
    }

    private void BlowUp(Vector3 from, Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(from, dir, out hit, dir.magnitude, raycastLayers))
        {
            maze.EatAt(hit.transform.position);
            hit.transform.gameObject.SetActive(false);
        }

        if ((playerRef.transform.position - (from + dir)).sqrMagnitude < 1.0f && !playerRef.PoweredUp && !playerRef.Dodging && !playerRef.PlayerDead)
        {
            playerRef.Die();
        }

        foreach (Enemy e in enemies)
        {
            if ((e.transform.position - (from + dir)).sqrMagnitude < 1.0f)
            {
                e.BlowMeUp();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Strings.PLAYER_TAG) && scoreRef != null && !exploding && !playerRef.PlayerDead)
        {
            exploding = true;
            flashTime = ExplodeTime;
            AudioHelper.PlaySFX(tombstoneTick);

            if (!hinted)
            {
                hinted = true;
                hinter.BeginHint(Strings.TOMBSTONE_HINT);
            }
        }
    }

    public void UpdateLoc(IVec2 mazeLoc, IVec2 sectionLoc)
    {
        this.mazeLoc = mazeLoc;
        this.sectionLoc = sectionLoc;
        if (myRenderer != null) { myRenderer.material = DefaultMat; }
        exploding = false;
        flashTime = 0.0f;

        if (explosion != null)
        {
            explosion.transform.position = transform.position;
            explosion.gameObject.SetActive(false);
        }
    }

}
