using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour {
    public GameObject baseRot;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.E)== true)
        {
            transform.RotateAround(baseRot.transform.position,new Vector3(1,0,0),95);
        }
	}
}
