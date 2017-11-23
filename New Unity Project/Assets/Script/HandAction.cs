using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAction : MonoBehaviour
{
    public LayerMask GrabAbleLayer;
    Vector3 GrabDirection;

    public bool LeftDetect = false;
    public bool RightDetect = false;
    public GameObject ClimbTargetHand;
    

    void ClimbableCheck()
    {
        LeftDetect = false;
        RightDetect = false;
        RaycastHit hit;
        float HandOutDist = this.gameObject.GetComponent<Collider>().bounds.extents.x;
        if (Physics.Raycast(transform.position, transform.forward, out hit,HandOutDist + 0.1f, GrabAbleLayer))
        {
            ClimbTargetHand = hit.transform.gameObject;
            Debug.Log("detected");
            if (this.gameObject.name == "Left")
            {
                LeftDetect = true;
            }
            else if (this.gameObject.name == "Right")
            {
                RightDetect = true;
            }
        }
    }

    
    //void OnTriggerEnter(Collider other)
    //{
    //    ClimbTargetHand = other.gameObject;
    //    if (this.gameObject.name == "Left")
    //    {
    //        LeftDetect = true;
    //    } else if (this.gameObject.name == "Right")
    //    {
    //        RightDetect = true;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    LeftDetect = false;
    //    RightDetect = false;
    //}

    // Update is called once per frame
    void Update()
    {
        ClimbableCheck();
    }
}
