using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMe : MonoBehaviour {
    public float Speed;
    private Vector3 axis;
    private float radians;

	void Awake () {
        axis = Random.onUnitSphere;
        radians = 0.0f;
	}
	
	void Update () {
        radians += Speed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(axis.x * radians, axis.y * radians, axis.z * radians); // Mass lag on use... TODO: optimize or delete script :)
	}
}
