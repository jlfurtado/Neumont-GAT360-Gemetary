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

    //private Collider myCollider;
    // Use this for initialization
    void Awake()
    {
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
        if (other.CompareTag(Strings.PLAYER_TAG) && scoreRef != null)
        {
            scoreRef.AddScore(Value);
            maze.EatAt(mazeLoc, sectionLoc);
            
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
        }
    }
}
