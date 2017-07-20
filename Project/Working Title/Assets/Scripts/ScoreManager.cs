using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    public static int StoreScore;
    private static string PlayerName = "_____";
    public Text scoreText;
    public BonusText BonusText;
    public float DisplayTime;
    public int BonusThreshold;
    private int scoreValue = 0;
    private string[] highScores;
    //private bool didWin = false;

    public static void SetName(string name)
    {
        PlayerName = name == "" ? "_____" : name;
    }

    void Start()
    {
        highScores = new string[Strings.HIGH_SCORE_KEYS.Length];

        for (int i = 0; i < highScores.Length; ++i)
        {
            highScores[i] = PlayerPrefs.GetString(Strings.HIGH_SCORE_KEYS[i], "_____:0");
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
        if (scoreText != null)
        {
            scoreText.text = "Score: " + scoreValue;
        }
    }

    void OnDisable()
    {
        StoreScore = scoreValue;

        for (int i = 0; i < highScores.Length; ++i)
        {
            if (scoreValue > int.Parse(highScores[i].Substring(highScores[i].IndexOf(':') + 1)))
            {
                for (int j = highScores.Length - 2; j >= i; --j)
                {
                    highScores[j + 1] = highScores[j];
                }

                highScores[i] = PlayerName + ":" + scoreValue;
                break;
            }
        }

        for (int i = 0; i < highScores.Length; ++i)
        {
            PlayerPrefs.SetString(Strings.HIGH_SCORE_KEYS[i], highScores[i]);
        }

        PlayerPrefs.Save();
    }
}
