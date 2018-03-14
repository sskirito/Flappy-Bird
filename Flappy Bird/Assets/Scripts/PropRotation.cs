using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRotation : MonoBehaviour {

    float speed = 3f;

	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * speed);
	}
}
