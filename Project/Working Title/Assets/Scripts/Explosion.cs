using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
    public float Length;
    private float timer;
    private bool exploding = false;

	// Update is called once per frame
	void Update () {
		if (exploding)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                timer = 0.0f;
                exploding = false;
                gameObject.SetActive(false);
            }
        }
	}

    public void Explode()
    {
        timer = Length;
        exploding = true;
        gameObject.SetActive(true);
    }
}
