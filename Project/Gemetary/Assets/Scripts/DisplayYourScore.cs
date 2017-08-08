using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DisplayYourScore : MonoBehaviour {

	// Use this for initialization
	void Awake() {
        Text textComp = GetComponent<Text>();
        string scoreText = "Your Score: " + ScoreManager.StoreScore;
        textComp.text = scoreText;
    }
}
