using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour {
    public static PauseManager OnlyOne { get; private set; }

    private bool paused;

    void Awake()
    {
        if (OnlyOne == null) { OnlyOne = this; } else { Debug.LogError("ERROR MORE THAN ONE PAUSE MANAGER!"); }
        paused = false;
    }

    public bool Paused()
    {
        return paused;
    }

    public void Pause()
    {
        StartCoroutine(DoPauseAtEndOfFrame());
    }

    public void UnPause()
    {
        StartCoroutine(DoUnPauseAtEndOfFrame());
    }

    private IEnumerator DoPauseAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        paused = true;
    }

    private IEnumerator DoUnPauseAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        paused = false;
    }
}
