using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintText : MonoBehaviour {
    public float Duration;
    public static bool AllHintsDisabled = false;
    private float remainingTime = 0.0f;
    public bool Displaying { get; private set; }
    private Text myText;
    private int currentOutIdx = -1;
    private AxisInputHelper axisInput = null;

    void Awake()
    {
        axisInput = GameObject.FindGameObjectWithTag(Strings.AXIS_INPUT_HELPER_TAG).GetComponent<AxisInputHelper>();
        Displaying = false;
        myText = GetComponentInChildren<Text>();
    }

    void Start()
    {
        if (AllHintsDisabled)
        {
            gameObject.SetActive(false);
        }
        else
        {
            BeginHint("Hi!\nI'm an annoying hint!\nI'll go away eventually.\nOr you can make me\nby pressing 'escape'!\nAnnoying hints can be disabled\nin the game options.");
        }
    }

    void Update()
    {
        if (Displaying)
        {
            remainingTime -= Time.deltaTime;
            if ((remainingTime <= 0.0f) || (axisInput.AxisPressed(Strings.CANCEL_INDEX)) || (currentOutIdx >= 0 && axisInput.AxisPressed(currentOutIdx)))
            {
                Hide();
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Displaying = false;
        myText.text = "";
        PauseManager.OnlyOne.UnPause();
    }

	public void BeginHint(string hint)
    {
        BeginHint(hint, -1);
    }

    public void BeginHint(string hint, int extraOutIdx)
    {
        if (!AllHintsDisabled)
        {
            gameObject.SetActive(true);
            remainingTime = Duration;
            Displaying = true;
            myText.text = hint;
            currentOutIdx = extraOutIdx;
            PauseManager.OnlyOne.Pause();
        }
    }
}
