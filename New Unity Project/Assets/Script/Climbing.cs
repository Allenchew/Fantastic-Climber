using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour {
    public LayerMask Rope;
    public Vector3 hitpos;
    public bool Trigg = false;
    public Rigidbody rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void FixedUpdate()
    {
        RaycastHit FindParent;
        RaycastHit CurrentParent;
        RaycastHit PastParent;
        if (Physics.Raycast(transform.position,(transform.forward) * 0.05f + new Vector3(0, 0.05f, 0), out FindParent,1,Rope)){
            //transform.rotation = FindParent.transform.rotation;
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (Trigg)
                {
                    transform.position = FindParent.transform.position - (hitpos- transform.position);
                }
            }
        }
        if (Physics.Raycast(transform.position, (transform.forward) * 0.05f, out CurrentParent, 1, Rope))
        {
            Debug.Log("deteced");
            hitpos = CurrentParent.transform.position;
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (Trigg == false)
                {
                    Trigg = true;
                    rb.isKinematic = true;
                    transform.position = CurrentParent.transform.position - new Vector3(0, 0, 0.05f);
                } else if (Trigg == true)
                {
                    Trigg = false;
                    rb.isKinematic = false;
                }
            }
            if (Trigg)
            {


                if (Input.GetKey(KeyCode.C))
                {
                    transform.RotateAround(hitpos, Vector3.up, 90 * Time.deltaTime);
                }
                else if (Input.GetKey(KeyCode.V))
                {
                    transform.RotateAround(hitpos, Vector3.up, -90 * Time.deltaTime);
                }
            }

        }
        if (Physics.Raycast(transform.position, (transform.forward) * 0.05f - new Vector3(0, 0.05f, 0), out PastParent, 1, Rope))
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (Trigg)
                {
                    transform.position = PastParent.transform.position -(hitpos-transform.position );
                }
            }
        }

    }
}
