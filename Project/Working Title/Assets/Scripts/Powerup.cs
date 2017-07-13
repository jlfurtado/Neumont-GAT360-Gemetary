using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Powerup : MonoBehaviour {
    public int Value;
    public float StopTime;
    public IVec2 mazeLoc;
    public IVec2 sectionLoc;
    private ScoreManager scoreRef;
    private MazeScript maze;
    private FollowMazeSolution[] followEnemies;
    private DepthFirstExplore[] depthEnemies;

    //private Collider myCollider;
    // Use this for initialization
    void Start()
    {
        //myCollider = GetComponent<Collider>();
        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();
        maze = GameObject.FindGameObjectWithTag(Strings.MAZE_TAG).GetComponent<MazeScript>();
        GameObject[] followEnemyObjects = GameObject.FindGameObjectsWithTag(Strings.FOLLOW_ENEMY_TAG);
        GameObject[] depthEnemyObjects = GameObject.FindGameObjectsWithTag(Strings.DEPTH_ENEMY_TAG);

        followEnemies = new FollowMazeSolution[followEnemyObjects.Length];
        for (int i = 0; i < followEnemyObjects.Length; ++i)
        {
            followEnemies[i] = followEnemyObjects[i].GetComponent<FollowMazeSolution>();
        }

        depthEnemies = new DepthFirstExplore[depthEnemyObjects.Length];
        for (int i = 0; i < depthEnemyObjects.Length; ++i)
        {
            depthEnemies[i] = depthEnemyObjects[i].GetComponent<DepthFirstExplore>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(Strings.PLAYER_TAG) && scoreRef != null)
        {
            scoreRef.AddScore(Value);
            maze.EatAt(mazeLoc, sectionLoc);
            
            foreach (FollowMazeSolution enemy in followEnemies)
            {
                enemy.StopFor(StopTime);
            }

            foreach (DepthFirstExplore enemy in depthEnemies)
            {
                enemy.StopFor(StopTime);
            }

            gameObject.SetActive(false);
        }
    }
}
