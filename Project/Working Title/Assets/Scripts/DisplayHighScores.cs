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
            string[] vals = PlayerPrefs.GetString(Strings.HIGH_SCORE_KEYS[i], Strings.DEFAULT_NAME + ":0").Split(':');
            scoreText += (vals[0] + " : " + vals[1] + "\n");
        }

        textComp.text = scoreText;
	}
}
