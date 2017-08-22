using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Powerup : MonoBehaviour {
    public int Value;
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

    //private Collider myCollider;
    // Use this for initialization
    void Awake()
    {
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
