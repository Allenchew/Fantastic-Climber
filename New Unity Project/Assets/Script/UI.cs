using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {
    /*if HandAction.LeftDetect & HandAction.RightDetect == true
     * then show UI.
      */
    // Use this for initialization
    public GameObject grab;
    public GameObject pull;
    public GameObject climb;
    public GameObject player;
    GameObject hitObject;
    bool isFront;

    public LayerMask GrabAbleLayer;

    private static bool Drag;
    void Start () {
        grab.SetActive(false);
        pull.SetActive(false);
        climb.SetActive(false);
    }

    void CheckFront()
    {
        Vector3 rayStart = new Vector3(player.transform.position.x, (player.transform.position.y + 0.07f), player.transform.position.z);
        Debug.DrawRay(rayStart, Camera.main.transform.forward, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(rayStart, Camera.main.transform.forward, out hit, 0.08f))
        {
            isFront = true;
            hitObject = hit.collider.gameObject;
            Debug.Log("something in front");
        }
        else
        {
            isFront = false;
        }
    }

    // Update is called once per frame
    void Update() {
        CheckFront();
        if (isFront == true)
        {
            Debug.Log("Entered");
            Debug.Log(hitObject.tag);
            if (hitObject.tag == "ClimbAble")
            {
                pull.SetActive(true);
                Debug.Log("climbable in front");
                if (hitObject.GetComponent<drawer>().isReachLimit)
                {
                    pull.SetActive(false);
                    climb.SetActive(true);
                }
            }
        }
        else  {
            pull.SetActive(false);
            climb.SetActive(false);
        }


        //Drag = controller.pulling;
        //BA.transform.LookAt(Mcam.transform);
        //BB.transform.LookAt(Mcam.transform);
        //if (Physics.Raycast(transform.position, transform.forward, out hit,0.1f , GrabAbleLayer) && !Drag){
        //    BA.SetActive(true);
        //    if(Drawer.transform.localPosition.z < -0.226f)
        //    {
        //        BB.SetActive(true);
        //    }

        //}else
        //{
        //    BB.SetActive(false);
        //    BA.SetActive(false);
        //}
    }
}
