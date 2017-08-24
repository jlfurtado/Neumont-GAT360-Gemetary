using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    public static int StoreScore;
    private static string PlayerName = Strings.DEFAULT_NAME;
    private static bool cheating = false;
    private static bool hardMode = false;
    public Text scoreText;
    public BonusText BonusText;
    public float DisplayTime;
    public int BonusThreshold;
    private int scoreValue = 0;
    private string[] highScores;
    //private bool didWin = false;

    public static void SetName(string name)
    {
        PlayerName = name == "" ? Strings.DEFAULT_NAME : name;
        cheating = ScoreManager.GetName().Equals(Strings.CHEAT_NAME);
        hardMode = ScoreManager.GetName().Equals(Strings.HARD_NAME);
    }

    public static string GetName()
    {
        return PlayerName;
    }

    public static bool IsCheating()
    {
        return cheating;
    }

    public static bool IsHardcore()
    {
        return hardMode;
    }

    void Awake()
    {
        highScores = new string[Strings.HIGH_SCORE_KEYS.Length];

        for (int i = 0; i < highScores.Length; ++i)
        {
            highScores[i] = PlayerPrefs.GetString(Strings.HIGH_SCORE_KEYS[i], Strings.DEFAULT_SCORE_TEXT);
        }

        SetScoreText();
    }

    public void AddScore(int value)
    {
        value = (value / 5) * 5;
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
            scoreText.text = string.Concat(Strings.SCORE_PREFIX, scoreValue);
        }
    }

    void OnDisable()
    {
        if (cheating) { StoreScore = 0; return; }
        StoreScore = scoreValue;

        for (int i = 0; i < highScores.Length; ++i)
        {
            if (scoreValue > int.Parse(highScores[i].Substring(highScores[i].IndexOf(':') + 1)))
            {
                for (int j = highScores.Length - 2; j >= i; --j)
                {
                    highScores[j + 1] = highScores[j];
                }

                highScores[i] = string.Concat(PlayerName, Strings.COLON, scoreValue);
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
