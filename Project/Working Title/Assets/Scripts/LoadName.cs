using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class LoadName : MonoBehaviour {
    private InputField myInput = null;

	void Awake() {
        myInput = GetComponent<InputField>();
        myInput.text = ScoreManager.GetName();
	}
	
}
