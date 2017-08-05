using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class DisableHints : MonoBehaviour {
    private Toggle myToggle = null;

    void Awake()
    {
        myToggle = GetComponent<Toggle>();
        myToggle.isOn = HintText.AllHintsDisabled;
    }

    void OnDisable()
    {
        HintText.AllHintsDisabled = myToggle.isOn;
    }
}
