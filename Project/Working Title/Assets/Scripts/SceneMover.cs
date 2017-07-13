using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMover : MonoBehaviour {

    public void MoveToTitle()
    {
        KeepTitleMusic();
        SceneManager.LoadScene(Strings.TITLE_SCENE_NAME);
    }

    public void MoveToGameOver()
    {
        DontKeepTitleMusic();
        SceneManager.LoadScene(Strings.GAME_OVER_SCENE_NAME);
    }

    public void MoveToOptions()
    {
        KeepTitleMusic();
        SceneManager.LoadScene(Strings.OPTIONS_SCENE_NAME);
    }

    public void MoveToHowToPlay()
    {
        KeepTitleMusic();
        SceneManager.LoadScene(Strings.HOW_TO_PLAY_SCENE_NAME);
    }

    public void MoveToMaze()
    {
        DontKeepTitleMusic();
        SceneManager.LoadScene(Strings.MAZE_SCENE_NAME);
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
        Application.Quit();
    }
}
