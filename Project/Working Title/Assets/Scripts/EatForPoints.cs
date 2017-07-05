using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EatForPoints : MonoBehaviour {

    private Collider myCollider;
	// Use this for initialization
	void Start () {
        myCollider = GetComponent<Collider>();
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            Destroy(this.gameObject);
        }
    }
}
