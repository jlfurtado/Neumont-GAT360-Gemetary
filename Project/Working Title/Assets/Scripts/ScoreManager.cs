using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    public static int StoreScore;
    public Text scoreText;
    public BonusText BonusText;
    public float DisplayTime;
    public int BonusThreshold;
    private int scoreValue = 0;
    private int[] highScores;
    //private bool didWin = false;

    void Start()
    {
        highScores = new int[Strings.HIGH_SCORE_KEYS.Length];

        for (int i = 0; i < highScores.Length; ++i)
        {
            highScores[i] = PlayerPrefs.GetInt(Strings.HIGH_SCORE_KEYS[i], 0);
        }

        SetScoreText();
    }

    public void AddScore(int value)
    {
        scoreValue += value;
        SetScoreText();
        if (Mathf.Abs(value) >= BonusThreshold)
        {
            BonusText.ResetText(value, DisplayTime, 100.0f);
        }
    }

    private void SetScoreText()
    {
        scoreText.text = "Score: " + scoreValue;
    }

    void OnDisable()
    {
        StoreScore = scoreValue;

        for (int i = 0; i < highScores.Length; ++i)
        {
            if (scoreValue > highScores[i])
            {
                for (int j = highScores.Length - 2; j >= i; --j)
                {
                    highScores[j + 1] = highScores[j];
                }

                highScores[i] = scoreValue;
                break;
            }
        }

        for (int i = 0; i < highScores.Length; ++i)
        {
            PlayerPrefs.SetInt(Strings.HIGH_SCORE_KEYS[i], highScores[i]);
        }

        PlayerPrefs.Save();
    }
}
