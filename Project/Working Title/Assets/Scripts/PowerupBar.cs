using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupBar : MonoBehaviour {
    public Image backImage;
    public Image frontImage;
    private PlayerController playerRef;
    private MazeScript mazeRef;

    // Use this for initialization
    void Awake()
    {
        backImage.rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        playerRef = GameObject.FindGameObjectWithTag(Strings.PLAYER_TAG).GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float perc = playerRef.PoweredUp ? perc = playerRef.GetPowerupPercent()
                                         : playerRef.EmptySection() ? 0.0f : playerRef.GetGemPercent();

        if (!playerRef.PoweredUp) { frontImage.color = playerRef.GetCurrentMat().color; }

        frontImage.rectTransform.localScale = new Vector3(perc, 1.0f, 1.0f);
    }
}
