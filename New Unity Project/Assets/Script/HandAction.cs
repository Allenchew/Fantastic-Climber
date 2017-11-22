using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAction : MonoBehaviour
{

    Vector3 hand = new Vector3(1, 1, 1);
    public LayerMask GrabAbleLayer;
    Vector3 GrabDirection;
    bool climbable = false;

    public bool LeftDetect = false;
    public bool RightDetect = false;
    public GameObject ClimbTargetHand;

    void ClimbableCheck()
    {
        float HandOutDist = this.gameObject.GetComponent<Collider>().bounds.extents.x;
        if (Physics.Raycast(transform.position, transform.forward, HandOutDist + 0.1f, GrabAbleLayer))
        {
            climbable = true;
        }        
    }


    void OnTriggerEnter(Collider other)
    {
        ClimbTargetHand = other.gameObject;
        if (this.gameObject.name == "Left")
        {
            LeftDetect = true;
        } else if (this.gameObject.name == "Right")
        {
            RightDetect = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        LeftDetect = false;
        RightDetect = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
