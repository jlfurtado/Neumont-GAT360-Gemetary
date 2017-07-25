using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    public int Value;
    public IVec2 mazeLoc;
    public IVec2 sectionLoc;

    private ScoreManager scoreRef;
    private MazeScript maze;
    private PlayerController playerRef;

	// Use this for initialization
	void Start () {
        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();
        maze = GameObject.FindGameObjectWithTag(Strings.MAZE_TAG).GetComponent<MazeScript>();
        playerRef = GameObject.FindGameObjectWithTag(Strings.PLAYER_TAG).GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(Strings.PLAYER_TAG) && scoreRef != null)
        {
            if (playerRef.CanPickupBomb())
            {
                scoreRef.AddScore(Value);
                maze.EatAt(mazeLoc, sectionLoc);

                playerRef.AddBomb();
                gameObject.SetActive(false);
            }

        }
    }
}
