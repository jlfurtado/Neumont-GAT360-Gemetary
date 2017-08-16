using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class DisableMusic : MonoBehaviour {
    private Toggle myToggle = null;
    private AudioSource _titleSrc = null;
    private AudioSource optionsSrc
    {
        get
        {
            if (_titleSrc == null)
            {
                _titleSrc = GameObject.FindGameObjectWithTag(Strings.TITLE_MUSIC_TAG).GetComponent<AudioSource>();
            }

            return _titleSrc;
        }
    }

    void Awake()
    {
        myToggle = GetComponent<Toggle>();
        myToggle.isOn = !AudioHelper.BGMEnabled;
    }

    void OnDisable()
    {
        AudioHelper.BGMEnabled = !myToggle.isOn;
        if (optionsSrc != null) { optionsSrc.enabled = AudioHelper.BGMEnabled; }
    }

    public void OnToggleMe()
    {
        AudioHelper.BGMEnabled = !myToggle.isOn;
        if (optionsSrc != null) { optionsSrc.enabled = AudioHelper.BGMEnabled; }
    }
}
