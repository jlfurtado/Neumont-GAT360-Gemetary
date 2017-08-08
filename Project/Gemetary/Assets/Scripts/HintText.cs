using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintText : MonoBehaviour {
    public float Duration;
    public static bool AllHintsDisabled = false;
    private float remainingTime = 0.0f;
    private bool displaying = false;
    private Text myText;
    private string currentOut = "";
    private const float DEAD_ZONE = 0.0f;

    void Awake()
    {
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
        if (displaying)
        {
            remainingTime -= Time.deltaTime;
            if ((remainingTime <= 0.0f) || (Mathf.Abs(Input.GetAxis(Strings.CANCEL_BUTTON)) > DEAD_ZONE) || (currentOut != "" && Mathf.Abs(Input.GetAxis(currentOut)) > DEAD_ZONE))
            {
                Hide();
            }
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        displaying = false;
        myText.text = "";
        PauseManager.Paused = false;
    }

	public void BeginHint(string hint)
    {
        BeginHint(hint, "");
    }

    public void BeginHint(string hint, string additionalInputOut)
    {
        if (!AllHintsDisabled)
        {
            gameObject.SetActive(true);
            remainingTime = Duration;
            displaying = true;
            myText.text = hint;
            currentOut = additionalInputOut;
            PauseManager.Paused = true;
        }
    }
}
