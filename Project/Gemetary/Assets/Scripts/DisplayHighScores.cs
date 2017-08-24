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
            bool yourScore = vals[1].Equals(ScoreManager.StoreScore.ToString()) && vals[0].Equals(ScoreManager.GetName());
            string colorHex = ColorUtility.ToHtmlStringRGB((yourScore) ? Color.red : Color.black);
            scoreText = string.Concat(scoreText, Strings.OPEN_COLOR_PREFIX, colorHex, Strings.OPEN_COLOR_POSTFIX, vals[0], Strings.SPACE_COLON_SPACE, vals[1], Strings.CLOSE_COLOR); 
        }

        textComp.text = scoreText;
	}
}
