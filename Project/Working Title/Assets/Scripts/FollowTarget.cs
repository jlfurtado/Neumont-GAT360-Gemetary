using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
    public GameObject TargetToFollow;
    public Vector3 Offset;
    public float Speed;

    void LateUpdate()
    {
        transform.position = TargetToFollow.transform.position + Offset;
    }
}
