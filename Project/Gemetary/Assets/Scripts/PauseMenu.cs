using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    public bool Showing { get; private set; }
    private AxisInputHelper axisInput = null;

	void Awake ()
    {
        axisInput = GameObject.FindGameObjectWithTag(Strings.AXIS_INPUT_HELPER_TAG).GetComponent<AxisInputHelper>();
        Showing = false;
	}

    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        if (axisInput.AxisPressed(Strings.CANCEL_INDEX))
        {
            Hide();
        }
	}

    public void Hide()
    {
        Showing = false;
        PauseManager.OnlyOne.UnPause();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        Showing = true;
        PauseManager.OnlyOne.Pause();
        gameObject.SetActive(true);
    }
}
