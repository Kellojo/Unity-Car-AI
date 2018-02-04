using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {
    float panSpeed = 20;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 pos = transform.position;

        //move forward
		if (Input.GetKey("w")) {
            pos.z += panSpeed * Time.deltaTime;
        }
        //move back
        if (Input.GetKey("s")) {
            pos.z -= panSpeed * Time.deltaTime;
        }
        //move left
        if (Input.GetKey("a")) {
            pos.x -= panSpeed * Time.deltaTime;
        }
        //move right
        if (Input.GetKey("d")) {
            pos.x += panSpeed * Time.deltaTime;
        }

        transform.position = pos;
    }
}
