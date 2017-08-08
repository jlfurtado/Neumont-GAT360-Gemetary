using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
    public GameObject TargetToFollow;
    public Vector3 MinOffset;
    public Vector3 MaxOffset;
    public float ScrollSpeed;
    public float Speed;
    public float RotateSpeed;
    public float SlowdownRadius;
    private float zoom = 0.5f;

    private Vector3 vel;
    void LateUpdate()
    {
        zoom = Mathf.Clamp(zoom - ScrollSpeed * Time.deltaTime * Input.GetAxis("Mouse ScrollWheel"), 0.0f, 1.0f);
        Vector3 offset = Vector3.Lerp(MinOffset, MaxOffset, zoom);

        Vector3 toPos = TargetToFollow.transform.position + offset;
        Vector3 move = (toPos - transform.position);
        float dist = move.magnitude;
        if (dist > 0.0f)
        {
            float rampSpeed = Speed * (dist / SlowdownRadius);
            float clipSpeed = Mathf.Min(rampSpeed, Speed);
            vel = (clipSpeed / dist) * move;
            transform.position += vel * Time.deltaTime;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((TargetToFollow.transform.position - transform.position).normalized), RotateSpeed * Time.deltaTime);
    }
}
