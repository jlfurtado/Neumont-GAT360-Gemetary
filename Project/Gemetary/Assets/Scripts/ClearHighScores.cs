using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearHighScores : MonoBehaviour {

    public void ClearScores()
    {
        for (int i = 0; i < Strings.HIGH_SCORE_KEYS.Length; ++i)
        {
            PlayerPrefs.SetString(Strings.HIGH_SCORE_KEYS[i], Strings.DEFAULT_SCORE_TEXT);
        }
    }
}
