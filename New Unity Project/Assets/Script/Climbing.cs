using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour {
    public LayerMask Rope;
    public LayerMask RopeEnd;
    public LayerMask Detection;
    public Vector3 hitpos;
    public bool Trigg = false;
    public Rigidbody rb;

    private bool EndClimb = false;
    private bool HeadAlert = false;
    private bool climbAnim = false;
    private Vector3 nextParent;
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
        RaycastHit HeadDetect;
        //transform.rotation = FindParent.transform.rotation;
        Debug.DrawRay(transform.position, (transform.up * 0.1f),Color.blue);
        if (Physics.Raycast(transform.position, (transform.up), out HeadDetect, 0.1f,Detection))
        {
            HeadAlert = true;
            Debug.Log("detected");
        }else
        {
            HeadAlert = false;
        }
       if (Input.GetKeyDown(KeyCode.Z) && !climbAnim && !HeadAlert)
            {
            if (Physics.Raycast(transform.position, (transform.forward) * 0.05f + new Vector3(0, 0.05f, 0), out FindParent, 1, Rope) && Trigg )
            {
                var nextpos = FindParent.transform.position - (hitpos - transform.position);
                //transform.position = Vector3.Slerp(transform.position, nextpos, 0.125f);//FindParent.transform.position - (hitpos- transform.position);
                StartCoroutine(SmoothMov(nextpos));
            }
            else if (Physics.Raycast(transform.position, (transform.forward) * 0.05f + new Vector3(0, 0.05f, 0), out FindParent, 1,RopeEnd) && Trigg)
            {
                nextParent = FindParent.transform.position;
                StartCoroutine("FinClimb");
                EndClimb = true;
                Trigg = false;
            }
        }
        if (Physics.Raycast(transform.position, (transform.forward) * 0.05f, out CurrentParent, 1, Rope) && !EndClimb)
        {
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
        if (Physics.Raycast(transform.position, (transform.forward) * 0.05f - new Vector3(0, 0.05f, 0), out PastParent, 1, Rope) && !EndClimb)
        {
            if (Input.GetKeyDown(KeyCode.X) && !climbAnim)
            {
                if (Trigg)
                {
                    var nextpos = PastParent.transform.position -(hitpos-transform.position );
                    StartCoroutine(SmoothMov(nextpos));
                }
            }
        }
        
    }
    IEnumerator FinClimb()
    {
        for (int i = 0; i < 10; i++)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, nextParent.y, transform.position.z), 0.125f);
            yield return new WaitForSeconds(0.01f);
        }
        for(int j = 0; j < 10; j++)
        {
            transform.RotateAround(hitpos, Vector3.up, 720 * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
        for(int k = 0; k < 10; k++)
        {
            transform.position -= new Vector3(0, 0, 0.001f);
        }
        rb.isKinematic = false;
    }
    IEnumerator SmoothMov(Vector3 Dest)
    {
        climbAnim = true;
        for(int i = 0; i < 10; i++)
        {
            transform.position = Vector3.Lerp(transform.position, Dest, 0.1f * i);
            yield return new WaitForSeconds(0.01f);
        }
        climbAnim = false;
        StopCoroutine("SmoothMov");
    }
}
