using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EatForPoints : MonoBehaviour {
    public int Value;
    public IVec2 mazeLoc;
    public IVec2 sectionLoc;
    private ScoreManager scoreRef;
    private MazeScript maze;

    //private Collider myCollider;
	// Use this for initialization
	void Start () {
        //myCollider = GetComponent<Collider>();
        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();
        maze = GameObject.FindGameObjectWithTag(Strings.MAZE_TAG).GetComponent<MazeScript>();
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(Strings.PLAYER_TAG) && scoreRef != null)
        {
            scoreRef.AddScore(Value);
            maze.EatGem(mazeLoc, sectionLoc);
            gameObject.SetActive(false);
        }
    }
}
