using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawer : MonoBehaviour {

    public GameObject limit;
    RaycastHit hit;
    public bool isReachLimit;

    // Use this for initialization
    void Start () {
		
	}

    public void checkDrawer()
    {
        Debug.DrawRay(transform.position, transform.up);
        if (Physics.Raycast(limit.transform.position, transform.up, out hit, 1f))
        {
            //Debug.Log("hit something");
            //Debug.Log(hit.transform.name);
            isReachLimit = false;
        }
        else
        {
            //Debug.Log("reach limit");
            this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            isReachLimit = true;
        }
        
    }
	
	// Update is called once per frame
	void Update () {

	}
}
