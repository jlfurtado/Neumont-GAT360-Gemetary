using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
    public GameObject TargetToFollow;
    public Vector3 Offset;
    public float Speed;
    public float SlowdownRadius;

    private Vector3 vel;
    void LateUpdate()
    {
        Vector3 toPos = TargetToFollow.transform.position + Offset;
        Vector3 move = (toPos - transform.position);
        float dist = move.magnitude;
        float rampSpeed = Speed * (dist / SlowdownRadius);
        float clipSpeed = Mathf.Min(rampSpeed, Speed);
        vel = clipSpeed / dist * move;
        transform.position += vel * Time.deltaTime;
    }
}
