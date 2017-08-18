using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ButtonSFX : MonoBehaviour {
    public AudioClip Hover;
    public AudioClip Click;
    private AudioSource myAudio;

    void Awake()
    {
        myAudio = GetComponent<AudioSource>();
        AudioHelper.InitSFX(myAudio);
    }
   
    public void PlayClick()
    {
        if (myAudio != null)
        {
            myAudio.clip = Click;
            AudioHelper.PlaySFX(myAudio);
        }
    }

    public void PlayHover()
    {
        if (myAudio != null)
        {
            myAudio.clip = Hover;
            AudioHelper.PlaySFX(myAudio);
        }
    }
}
