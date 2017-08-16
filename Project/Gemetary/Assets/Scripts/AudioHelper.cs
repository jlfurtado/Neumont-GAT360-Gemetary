using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioHelper {
    public static float BGMMultiplier = 1.0f;
    public static float SFXMultiplier = 1.0f;
    public static bool BGMEnabled = true;
    public static bool SFXEnabled = true;

    public static void InitSFX(AudioSource sfx)
    {
        sfx.volume *= SFXMultiplier;
        if (sfx.isPlaying && !SFXEnabled) { sfx.Stop(); }
    }

    public static void PlaySFX(AudioSource sfx)
    {
        if (SFXEnabled) { sfx.Play(); }   
    }

    public static void InitBGM(AudioSource bgm)
    {
        bgm.volume *= BGMMultiplier;
        if (bgm.isPlaying && !BGMEnabled) { bgm.Stop(); }
    }

    public static void PlayBGM(AudioSource bgm)
    {
        if (BGMEnabled) { bgm.Play(); }
    }
}
