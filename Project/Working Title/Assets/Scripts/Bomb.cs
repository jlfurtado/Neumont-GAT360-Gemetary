﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(Rigidbody))]
public class Bomb : MonoBehaviour {
    public int Value;
    private IVec2 mazeLoc;
    private IVec2 sectionLoc;
    public float ExplodeTime;
    public Material DefaultMat;
    public Material FlashMat;

    private ScoreManager scoreRef;
    private MazeScript maze;
    private PlayerController playerRef;
    private bool exploding;
    private float flashTime;
    private Renderer myRenderer = null;
    private SceneMover sceneMoverRef = null;
    private Enemy[] enemies;

    // Use this for initialization
    void Awake() {
        scoreRef = GameObject.FindGameObjectWithTag(Strings.SCORE_MANAGER_TAG).GetComponent<ScoreManager>();
        maze = GameObject.FindGameObjectWithTag(Strings.MAZE_TAG).GetComponent<MazeScript>();
        playerRef = GameObject.FindGameObjectWithTag(Strings.PLAYER_TAG).GetComponent<PlayerController>();
        sceneMoverRef = GameObject.FindGameObjectWithTag(Strings.SCENE_MOVER_TAG).GetComponent<SceneMover>();
        myRenderer = GetComponent<Renderer>();

        myRenderer.material = DefaultMat;

        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag(Strings.ENEMY_TAG);

        enemies = new Enemy[enemyObjects.Length];
        for (int i = 0; i < enemyObjects.Length; ++i)
        {
            enemies[i] = enemyObjects[i].GetComponent<Enemy>();
        }
    }

    void Update()
    {
        if (exploding)
        {
            flashTime -= Time.deltaTime;
            myRenderer.material = Mathf.Sqrt(flashTime) * 100 % 7 < 2 ? DefaultMat : FlashMat;

            if (flashTime <= 0.0f)
            {
                exploding = false;
                Explode();
            }
        }
    }
    
    private void Explode()
    {
        scoreRef.AddScore(Value);
        maze.EatAt(mazeLoc, sectionLoc);
        gameObject.SetActive(false);
        BlowUp(transform.position, Vector3.zero);
        BlowUp(transform.position, Vector3.right);
        BlowUp(transform.position, Vector3.left);
        BlowUp(transform.position, Vector3.forward);
        BlowUp(transform.position, Vector3.back);
        BlowUp(transform.position, Vector3.right + Vector3.forward);
        BlowUp(transform.position, Vector3.left + Vector3.back);
        BlowUp(transform.position, Vector3.forward + Vector3.left);
        BlowUp(transform.position, Vector3.back + Vector3.right);
        //BlowUp(transform.position, 2.0f * Vector3.right);
        //BlowUp(transform.position, 2.0f * Vector3.left);
        //BlowUp(transform.position, 2.0f * Vector3.forward);
        //BlowUp(transform.position, 2.0f * Vector3.back);
    }

    private void BlowUp(Vector3 from, Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(from, dir, out hit, dir.magnitude))
        {
            maze.EatAt(hit.transform.position);
            hit.transform.gameObject.SetActive(false);
        }

        if ((playerRef.transform.position - (from + dir)).sqrMagnitude < 1.0f && !playerRef.PoweredUp && !playerRef.Dodging)
        {
            sceneMoverRef.MoveToGameOver();
        }

        foreach (Enemy e in enemies)
        {
            if ((e.transform.position - (from + dir)).sqrMagnitude < 1.0f)
            {
                e.BlowMeUp();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(Strings.PLAYER_TAG) && scoreRef != null)
        {
            exploding = true;
            flashTime = ExplodeTime;
        }
    }

    public void UpdateLoc(IVec2 mazeLoc, IVec2 sectionLoc)
    {
        this.mazeLoc = mazeLoc;
        this.sectionLoc = sectionLoc;
        if (myRenderer != null) { myRenderer.material = DefaultMat; }
        exploding = false;
        flashTime = 0.0f;
    }

}
