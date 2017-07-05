using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
    public GameObject TargetToFollow;
    public Vector3 Offset;

    void Update()
    {
        transform.position = TargetToFollow.transform.position + Offset;
    }
}
