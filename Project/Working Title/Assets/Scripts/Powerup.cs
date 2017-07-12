using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour {
    public int Value;
    public float StopTime;
    public IVec2 mazeLoc;
    public IVec2 sectionLoc;
    private ScoreManager scoreRef;
    private MazeScript maze;
    private FollowMazeSolution[] enemies;

    //private Collider myCollider;
    // Use this for initialization
    void Start()
    {
        //myCollider = GetComponent<Collider>();
        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();
        maze = GameObject.FindGameObjectWithTag(Strings.MAZE_TAG).GetComponent<MazeScript>();
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag(Strings.ENEMY_TAG);
        enemies = new FollowMazeSolution[enemyObjects.Length];
        for (int i = 0; i < enemyObjects.Length; ++i)
        {
            enemies[i] = enemyObjects[i].GetComponent<FollowMazeSolution>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(Strings.PLAYER_TAG) && scoreRef != null)
        {
            scoreRef.AddScore(Value);
            maze.EatAt(mazeLoc, sectionLoc);
            
            foreach (FollowMazeSolution enemy in enemies)
            {
                enemy.StopFor(StopTime);
            }
            gameObject.SetActive(false);
        }
    }
}
