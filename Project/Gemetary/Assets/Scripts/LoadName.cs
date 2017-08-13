using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InputField))]
public class LoadName : MonoBehaviour {
    private InputField myInput = null;
    //private AxisInputHelper axisInput = null;

	void Awake() {
        //axisInput = GameObject.FindGameObjectWithTag(Strings.AXIS_INPUT_HELPER_TAG).GetComponent<AxisInputHelper>();
        myInput = GetComponent<InputField>();
        myInput.text = ScoreManager.GetName();
	}
    
    //void Update()
    //{
    //}	
}
