using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DisplayHighScores : MonoBehaviour {

	// Use this for initialization
	void Awake() {
        Text textComp = GetComponent<Text>();
        string scoreText = Strings.BASE_HIGH_SCORE_TEXT;

        for (int i = 0; i < Strings.HIGH_SCORE_KEYS.Length; ++i)
        {
            string[] vals = PlayerPrefs.GetString(Strings.HIGH_SCORE_KEYS[i], Strings.DEFAULT_SCORE_TEXT).Split(':');
            scoreText = string.Concat(scoreText, vals[0], Strings.SPACE_COLON_SPACE, vals[1], Strings.NEWLINE);
        }

        textComp.text = scoreText;
	}
}
