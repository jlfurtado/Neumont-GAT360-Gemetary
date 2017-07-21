using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeBarController : MonoBehaviour {
    public Image backImage;
    public Image frontImage;
    private PlayerController playerRef;
    private float heightStore;
    private Color32 defaultColor;
    private Color32 dodgeColor;

	// Use this for initialization
	void Start () {
        heightStore = frontImage.rectTransform.sizeDelta.y;
        defaultColor = frontImage.color;
        dodgeColor = new Color32(0, 255, 0, 255);
        playerRef = GameObject.FindGameObjectWithTag(Strings.PLAYER_TAG).GetComponent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {
        float perc = playerRef.GetEndurancePercent();
        float currentWidth = perc * backImage.rectTransform.sizeDelta.x;
        frontImage.color = perc < .5f ? defaultColor : dodgeColor;

        frontImage.rectTransform.sizeDelta = new Vector2(currentWidth, heightStore);
        //frontImage.enabled = currentWidth > 0.0f;
	}
}
