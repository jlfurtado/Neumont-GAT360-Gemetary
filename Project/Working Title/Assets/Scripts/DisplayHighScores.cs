using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DisplayHighScores : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Text textComp = GetComponent<Text>();
        string scoreText = "High Scores:\n";

        for (int i = 0; i < Strings.HIGH_SCORE_KEYS.Length; ++i)
        {
            scoreText += (Strings.HIGH_SCORE_KEYS[i] + ": " + PlayerPrefs.GetInt(Strings.HIGH_SCORE_KEYS[i], 0) + "\n");
        }

        textComp.text = scoreText;
	}
}
