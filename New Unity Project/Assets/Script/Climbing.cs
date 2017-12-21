using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour {
    public LayerMask Rope;
    public LayerMask RopeEnd;
    public LayerMask Detection;
    public Vector3 hitpos;
    public GameObject TpRop;
    public bool Trigg = false;
    public Rigidbody rb;

    private bool EndClimb = true;
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

        if (Physics.Raycast(transform.position, (transform.forward) * 0.05f, out CurrentParent, 0.05f, Rope))
        {
            hitpos = CurrentParent.transform.position;
            if (Input.GetKeyDown(KeyCode.T))
            {
                EndClimb = false;
                if (Trigg == false)
                {
                    Trigg = true;
                    rb.isKinematic = true;
                    transform.position = CurrentParent.transform.position - new Vector3(0, 0, 0.05f);
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                }
                else if (Trigg == true)
                {
                    Trigg = false;
                    rb.isKinematic = false;
                }
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

            if (Physics.Raycast(transform.position, (transform.up), out HeadDetect, 0.1f, Detection))
            {
                HeadAlert = true;
            }
            else
            {
                HeadAlert = false;
            }
            if (Physics.Raycast(transform.position, (transform.forward) * 0.05f + new Vector3(0, 0.05f, 0), out FindParent, 1, Rope))
            {
                if (Input.GetKeyDown(KeyCode.Z) && !climbAnim && !HeadAlert)
                {
                    if (FindParent.transform.tag == "TopRope")
                    {
                        Debug.Log("Top");
                        var nextpos = FindParent.transform.position + new Vector3(0, 0, 0.05f);
                        var nextRot = new Quaternion(0, 180, 0, 0);
                        StartCoroutine(SmoothMov(nextpos));
                        StartCoroutine(TopRp(nextRot));
                    }
                    else if (FindParent.transform.tag != "TopRope")
                    {
                        Debug.Log("Norm");
                        var nextpos = FindParent.transform.position - (hitpos - transform.position);
                        StartCoroutine(SmoothMov(nextpos));
                    }
                    
                }
            }
            if (Physics.Raycast(transform.position, (transform.forward) * 0.05f + new Vector3(0, 0.05f, 0), out FindParent, 1, RopeEnd))
            {
                if (Input.GetKeyDown(KeyCode.Z) && !climbAnim && !HeadAlert)
                {
                    nextParent = FindParent.transform.position;
                    StartCoroutine(FinClimb(0, 1));
                    EndClimb = true;
                    Trigg = false;
                }
            }
            if (Physics.Raycast(transform.position, (transform.forward) * 0.05f - new Vector3(0, 0.05f, 0), out PastParent, 1, Rope) && !EndClimb)
            {
                if (Input.GetKeyDown(KeyCode.X) && !climbAnim && Trigg)
                {
                    var Prepos = PastParent.transform.position - (hitpos - transform.position);
                    StartCoroutine(SmoothMov(Prepos));


                }
            }
        }
        if (Physics.Raycast(transform.position, (transform.forward) * 0.05f, out CurrentParent, 0.05f, RopeEnd)&& EndClimb)
            {
            if (Input.GetKeyDown(KeyCode.T))
                {
                rb.isKinematic = true;
                nextParent = TpRop.transform.position;
                StartCoroutine(FinClimb(3, -1));
                EndClimb = false;
                Trigg = true;
            }

        }
    }
    IEnumerator FinClimb(int BaseNum, int Progression)
    {
        for (int i = (BaseNum == 0 ? BaseNum :BaseNum-1); (BaseNum == 0 ? i < 3 : i >= 0); i += Progression)
        {
            for (int j = 0; j < 10; j++)
            {
                switch (i)
                {
                    case 0:
                        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, nextParent.y, transform.position.z), 0.1f*j);
                        yield return new WaitForSeconds(0.01f);
                        break;
                    case 1:
                        transform.RotateAround(hitpos, Vector3.up,Progression* 720 * Time.deltaTime);
                        if (Progression<0) { transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(0, 180, 0, 0), 0.1f * j); }
                        yield return new WaitForSeconds(0.01f);
                        break;

                    case 2:
                        transform.position -= new Vector3(0, 0, 0.001f*Progression);
                        break;

                }
            }
        }
        rb.isKinematic = Progression > 0 ? false : true;
    }
    IEnumerator SmoothMov(Vector3 Dest)
    {
        climbAnim = true;
        for(int i = 0; i < 10; i++)
        {
            transform.position = Vector3.Lerp(transform.position, Dest, 0.1f*i);
            yield return new WaitForSeconds(0.01f);
        }
        StopCoroutine("SmoothMov");
        climbAnim = false;
    }
    IEnumerator TopRp(Quaternion Rot)
    {
        for (int i = 0; i < 10; i++)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Rot, 0.1f * i);
            yield return new WaitForSeconds(0.01f);
        }
        StopCoroutine("TopRp");
    }
}
