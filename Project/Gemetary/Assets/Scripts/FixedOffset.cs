using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedOffset : MonoBehaviour {
    public GameObject FixTo;
    public Vector3 offset;

	void LateUpdate()
    {
        //if (PauseManager.Paused) { return; }
        transform.position = FixTo.transform.position + offset;
    }
}
