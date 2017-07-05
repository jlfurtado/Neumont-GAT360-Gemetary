using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EatForPoints : MonoBehaviour {
    public int Value;

    private ScoreManager scoreRef;
    private Collider myCollider;
	// Use this for initialization
	void Start () {
        myCollider = GetComponent<Collider>();
        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(Strings.PLAYER_TAG))
        {
            scoreRef.AddScore(Value);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
