using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Text), typeof(RectTransform))]
public class BonusText : MonoBehaviour {
    private Color goodColor = Color.green;
    private Color badColor = Color.red;
    private float timer = 0.0f;
    private float totalTime;
    private Text text;
    private RectTransform myRect;
    private float moveAmount;
    private Vector3 basePosition;
    private int maxLines;

    void Awake()
    {
        basePosition = transform.position;
        text = GetComponent<Text>();
        text.enabled = false;
        text.text = "";
        myRect = GetComponent<RectTransform>();
        maxLines = Mathf.FloorToInt(myRect.rect.height / text.preferredHeight);
    }

	public void ResetText(int score, float time, float moveAmount)
    {
        this.moveAmount = moveAmount;
        timer = time;
        totalTime = time;
        string colorHex = ColorUtility.ToHtmlStringRGB(((score < 0) ? badColor : goodColor));
        string baseText = text.text.Count(c => c == '\n') >= maxLines ? text.text.Substring(text.text.IndexOf('\n') + 1) : text.text;
        text.text = baseText + ("<color=" + "\"#" + colorHex + "\"" + ">")+ (score > 0 ? "+" : "") + score + "</color>\n";
        text.enabled = true;
    }

    void Update ()
    {
        timer -= Time.deltaTime;

        if (totalTime > 0.0f && text.enabled)
        {
            transform.position = basePosition + new Vector3(0.0f, (moveAmount * (totalTime - timer) / totalTime), 0.0f);
        }

        if (timer < 0.0f && text.enabled)
        {
            DisableText();
        }	
	}

    private void DisableText()
    {
        timer = 0.0f;
        text.enabled = false;
        text.text = "";
        transform.position = basePosition;
    }
}
