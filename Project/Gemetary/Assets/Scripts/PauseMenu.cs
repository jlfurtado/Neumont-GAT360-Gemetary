using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour {
    public bool Showing { get; private set; }
    public GameObject ToSelect;
    private AxisInputHelper axisInput = null;
    private WaitForSeconds wait = new WaitForSeconds(0.25f);

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
        StartCoroutine(HideDelayed());
    }

    private IEnumerator HideDelayed()
    {
        yield return wait;

        if (Showing)
        {
            Showing = false;
            PauseManager.OnlyOne.UnPause();
            EventSystem.current.SetSelectedGameObject(null);
            gameObject.SetActive(false);
        }
    }
    public void Show()
    {
        if (!Showing)
        {
            Showing = true;
            PauseManager.OnlyOne.Pause();
            gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(ToSelect); // NEEDS TO BE AFTER SET ACTIVE FOR HIGHLIGHT!!!
        }
    }
}
