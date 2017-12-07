using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {
    /*if HandAction.LeftDetect & HandAction.RightDetect == true
     * then show UI.
      */
    // Use this for initialization
    public GameObject BA;
    public GameObject BB;
    public GameObject Mcam;
    public GameObject Drawer;

    public LayerMask GrabAbleLayer;

    private static bool Drag;
    private static bool LDragging;
    private static bool RDragging;
    void Start () {
        BB.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        RaycastHit hit;

        LDragging = HandAction.Lhand;
        RDragging = HandAction.Rhand;
        Drag = controller.pulling;
        BA.transform.LookAt(Mcam.transform);
        BB.transform.LookAt(Mcam.transform);
        if (Physics.Raycast(transform.position, transform.forward, out hit,0.1f , GrabAbleLayer) && !Drag){
            BA.SetActive(true);
            if(Drawer.transform.localPosition.z < -0.226f)
            {
                BB.SetActive(true);
            }
            Debug.Log("set");
        }else
        {
            BB.SetActive(false);
            BA.SetActive(false);
        }
      /*  if (LDragging && RDragging)
        {
            
        }
        else if(BA.activeSelf == true && !(LDragging && RDragging))
        {
          
        }*/
    }
}
