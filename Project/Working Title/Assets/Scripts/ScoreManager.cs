using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    public Text scoreText;
    private int scoreValue = 0;
    //private int highScore = 0; TODO HIGH SCORES
    //private bool didWin = false;

    void Start()
    {
        SetScoreText();
    }

    public void AddScore(int value)
    {
        scoreValue += value;
        SetScoreText();
    }

    private void SetScoreText()
    {
        scoreText.text = "Score: " + scoreValue;
    }
}
