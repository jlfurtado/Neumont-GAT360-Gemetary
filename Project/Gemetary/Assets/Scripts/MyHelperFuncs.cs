using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyHelperFuncs
{
    public static void EnableRigid(Rigidbody rigid)
    {
        rigid.detectCollisions = true;
    }

    public static void DisableRigid(Rigidbody rigid)
    {
        rigid.detectCollisions = false;
    }

    //public static void StopRigid(Rigidbody rigid)
    //{
    //    rigid.isKinematic = true;
    //    rigid.velocity = Vector3.zero;
    //}

    //public static void StartRigid(Rigidbody rigid)
    //{
    //    rigid.isKinematic = false;
    //}
}
