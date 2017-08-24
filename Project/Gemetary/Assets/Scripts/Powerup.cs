using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Powerup : MonoBehaviour {
    public int Value;
    public float MinIntensity;
    public float MaxIntensity;
    public float CycleLength;
    public float StopTime;
    public float FlashTime;
    public IVec2 mazeLoc;
    public IVec2 sectionLoc;
    private ScoreManager scoreRef;
    private Flasher flasherRef;
    private MazeScript maze;
    private Enemy[] enemies;
    public static bool hinted = false;
    private HintText hinter;
    private AudioSource myAudioSFX;
    private bool flashing = false;
    private PlayerController playerRef;
    private Light lightRef;
    private bool brighten;
    private float timer;
    private FollowTarget mainCamera;

    //private Collider myCollider;
    // Use this for initialization
    void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag(Strings.MAIN_CAMERA_TAG).GetComponent<FollowTarget>();
        lightRef = GetComponentInChildren<Light>();
        playerRef = GameObject.FindGameObjectWithTag(Strings.PLAYER_TAG).GetComponent<PlayerController>();
        myAudioSFX = GameObject.FindGameObjectWithTag(Strings.THUNDER_SFX_TAG).GetComponent<AudioSource>();
        hinter = GameObject.FindGameObjectWithTag(Strings.HINTER_TAG).GetComponent<HintText>();
        flasherRef = GameObject.FindGameObjectWithTag(Strings.FLASHER_TAG).GetComponent<Flasher>();
        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();
        maze = GameObject.FindGameObjectWithTag(Strings.MAZE_TAG).GetComponent<MazeScript>();
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag(Strings.ENEMY_TAG);

        enemies = new Enemy[enemyObjects.Length];
        for (int i = 0; i < enemyObjects.Length; ++i)
        {
            enemies[i] = enemyObjects[i].GetComponent<Enemy>();
        }
        
        lightRef.intensity = MinIntensity;
        brighten = true;
    }

    private void ToggleBrightDir()
    {
        brighten = !brighten;
        timer = CycleLength;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            ToggleBrightDir();
        }
        else
        {
            float perc = (1.0f - (timer / CycleLength));
            lightRef.intensity = brighten ? Mathf.Lerp(MinIntensity, MaxIntensity, perc)
                                          : Mathf.Lerp(MaxIntensity, MinIntensity, perc);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Strings.PLAYER_TAG) && scoreRef != null && !flashing && !playerRef.PlayerDead)
        {
            StartCoroutine(DelayedFlash(0.25f));
        }
    }

    private IEnumerator DelayedFlash(float delay)
    {
        flashing = true;
        scoreRef.AddScore(Value);
        maze.EatAt(mazeLoc, sectionLoc);
        AudioHelper.PlaySFX(myAudioSFX);

        yield return new WaitForSeconds(delay);

        mainCamera.BeginShake(0.25f, 2.5f);
        foreach (Enemy enemy in enemies)
        {
            enemy.StopFor(StopTime);
        }

        flasherRef.Flash();

        if (!hinted)
        {
            hinted = true;
            hinter.BeginHint(Strings.LANTERN_HINT);
        }

        gameObject.SetActive(false);
        flashing = false;
    }
}
