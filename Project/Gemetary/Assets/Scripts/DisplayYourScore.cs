using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DisplayYourScore : MonoBehaviour {

	// Use this for initialization
	void Awake() {
        Text textComp = GetComponent<Text>();
        string scoreText = string.Concat(Strings.YOUR_SCORE_TEXT_PREFIX, ScoreManager.StoreScore, (ScoreManager.IsCheating() ? Strings.YOUR_SCORE_CHEATER_POSTFIX : (ScoreManager.IsHardcore() ? Strings.YOUR_SCORE_HARDCORE_POSTFIX : string.Empty)));
        textComp.text = scoreText;
    }
}
