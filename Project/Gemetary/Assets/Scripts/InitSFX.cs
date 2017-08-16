using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InitSFX : MonoBehaviour {
    void Awake(){
        AudioHelper.InitSFX(GetComponent<AudioSource>());
    }
}
