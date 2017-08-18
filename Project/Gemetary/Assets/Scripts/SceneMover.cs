using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMover : MonoBehaviour {
    private WaitForSeconds wait = new WaitForSeconds(0.25f);

    private IEnumerator LoadDelayed(string name)
    {
        yield return wait;
        SceneManager.LoadScene(name);
    }

    public void MoveToTitle()
    {
        KeepTitleMusic();
        StartCoroutine(LoadDelayed(Strings.TITLE_SCENE_NAME));
    }

    public void MoveToGameOver()
    {
        DontKeepTitleMusic();
        StartCoroutine(LoadDelayed(Strings.GAME_OVER_SCENE_NAME));
    }

    public void MoveToOptions()
    {
        KeepTitleMusic();
        SetPlayerName();
        StartCoroutine(LoadDelayed(Strings.OPTIONS_SCENE_NAME));
    }

    public void MoveToHowToPlay()
    {
        KeepTitleMusic();
        SetPlayerName();
        StartCoroutine(LoadDelayed(Strings.HOW_TO_PLAY_SCENE_NAME));
    }

    public void MoveToMaze()
    {
        DontKeepTitleMusic();
        SetPlayerName();
        ResetHints();
        StartCoroutine(LoadDelayed(Strings.MAZE_SCENE_NAME));
    }

    private void KeepTitleMusic()
    {
        GameObject titleMusic = GameObject.FindGameObjectWithTag(Strings.TITLE_MUSIC_TAG);
        if (titleMusic != null) { DontDestroyOnLoad(titleMusic); }
    }

    private void DontKeepTitleMusic()
    {
        GameObject titleMusic = GameObject.FindGameObjectWithTag(Strings.TITLE_MUSIC_TAG);
        if (titleMusic != null) { Destroy(titleMusic); }
    }

    public void QuitGame()
    {
        StartCoroutine(QuitDelayed());
    }

    private IEnumerator QuitDelayed()
    {
        yield return wait;
        Application.Quit();
    }

    private void ResetHints()
    {
        Bomb.hinted = false;
        Powerup.hinted = false;
    }

    private void SetPlayerName()
    {
        GameObject inputObj = GameObject.FindGameObjectWithTag(Strings.PLAYER_NAME_INPUT_TAG);

        if (inputObj != null)
        {
            InputField inputField = inputObj.GetComponent<InputField>();

            if (inputField != null)
            {
                ScoreManager.SetName(inputField.text);
            }
        }
    }
}
