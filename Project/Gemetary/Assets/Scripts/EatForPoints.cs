﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EatForPoints : MonoBehaviour {
    public int EnduranceRegen;
    public int Value;
    public IVec2 mazeLoc;
    public IVec2 sectionLoc;
    private ScoreManager scoreRef;
    private MazeScript maze;
    private Renderer myRenderer;
    private Material storeMat;
    private PlayerController playerRef;
    private AudioSource gemSFX;

    //private Collider myCollider;
	// Use this for initialization
	void Awake() {
        gemSFX = GameObject.FindGameObjectWithTag(Strings.GEM_SFX_TAG).GetComponent<AudioSource>();
        playerRef = GameObject.FindGameObjectWithTag(Strings.PLAYER_TAG).GetComponent<PlayerController>();
        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();
        maze = GameObject.FindGameObjectWithTag(Strings.MAZE_TAG).GetComponent<MazeScript>();
        myRenderer = GetComponent<Renderer>();
        if (storeMat != null) { myRenderer.material = storeMat; }
	}

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Strings.PLAYER_TAG) && scoreRef != null && !playerRef.PlayerDead)
        {
            AudioHelper.PlaySFX(gemSFX);
            playerRef.AddEndurance(EnduranceRegen);
            scoreRef.AddScore(Value);
            maze.EatAt(mazeLoc, sectionLoc);
            gameObject.SetActive(false);
        }
    }

    public void SetMat(Material mat)
    {
        if (myRenderer != null) { myRenderer.material = mat; }
        else { storeMat = mat; }
    }
}
