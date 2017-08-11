using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SelectionIndicator : MonoBehaviour {
    private RectTransform myRect;
    private GameObject lastSelected = null;

	// Use this for initialization
	void Awake () {
        myRect = GetComponent<RectTransform>();
        myRect.localScale = Vector3.zero;
        myRect.localPosition = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
        if (currentSelected != lastSelected)
        {
            if (currentSelected == null)
            {
                myRect.localScale = Vector3.zero;
                myRect.localPosition = Vector3.zero;
            }
            else
            {
                RectTransform rt = currentSelected.GetComponent<RectTransform>();
                myRect.position = rt.position + (Vector3.left * ((rt.rect.width /*- myRect.rect.width*/) / 2.0f)); // TODO: INVESTIGATE DIFFERENCES BETWEEN BUILD AND EDITOR HERE
                myRect.localScale = Vector3.one;
            }
        }

        lastSelected = currentSelected;
	}
}
