using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeBarController : MonoBehaviour {
    public Image backImage;
    public Image frontImage;
    private PlayerController playerRef;
    private float heightStore;

	// Use this for initialization
	void Start () {
        heightStore = frontImage.rectTransform.sizeDelta.y;
        playerRef = GameObject.FindGameObjectWithTag(Strings.PLAYER_TAG).GetComponent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {
        float currentWidth = playerRef.GetEndurancePercent() * backImage.rectTransform.sizeDelta.x;
        frontImage.rectTransform.sizeDelta = new Vector2(currentWidth, heightStore);
        //frontImage.enabled = currentWidth > 0.0f;
	}
}
