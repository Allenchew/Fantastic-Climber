using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {
    public Transform lookAt;
    public Transform CamTransform;
    public Vector3 Pos;
    public float smoothness = 0.125f;
    public CollideHandler CH = new CollideHandler();
    public GameObject Target;
    public Vector3 destination = Vector3.zero;
    public bool Pulling = false;
    public bool Climbing = false;
    public bool PullBag = false;
    private Camera cam;

    private Vector3 dir;
    private float Y_max = 50.0f;
    private float Y_min = -9.0f;
    public float X_max = 0.0f;
    public float X_min = -50.0f;
    private float distance = 0.3f;
    private float AdjDistance = 0;
    private float CurrentX = 0.0f;
    private float CurrentY = 0.0f;
    private float SensitivityX = 4.0f;
    private float SensitivityY = 1.0f;
    // Use this for initialization
    void Start()
    {
        CamTransform = transform;
        cam = Camera.main;
        CH.initialize(cam);
        CH.UpdateCameraCP(cam.transform.position, cam.transform.rotation, ref CH.adjustedCamCP);
        CH.UpdateCameraCP(destination, cam.transform.rotation, ref CH.desiredCamCP);

    }

    // Update is called once per frame
    void Update()
    {
        CurrentX += Input.GetAxis("Mouse X");
        CurrentY += Input.GetAxis("Mouse Y");

        CurrentY = Mathf.Clamp(CurrentY, -30, 85);

    }

    void LateUpdate()
    {
        Vector3 SmoothedPosB;
        if (Pulling)
        {
            var FixedCam = lookAt.position + new Vector3(-0.95f, 0.4f, -0.4f);
            SmoothedPosB = Vector3.Lerp(transform.position, FixedCam, smoothness);

        }
        else if (Climbing)
        {
            var FixedCam = lookAt.position - lookAt.transform.forward * 0.5f;
            SmoothedPosB = Vector3.Lerp(transform.position, FixedCam, smoothness);
        }
        else if (PullBag)
        {
            var FixedCam = lookAt.position + new Vector3(-0.2f, -0.05f, 0.25f);
            SmoothedPosB = Vector3.Lerp(transform.position, FixedCam, smoothness);
        }
        else
        {
            dir = new Vector3(0, 0, -distance);
            destination = Quaternion.Euler(CurrentY * SensitivityY, CurrentX * SensitivityX, 0) * dir;
            destination += lookAt.position;
            if (CH.colliding)
            {
                if (AdjDistance < 0.05f)
                {
                    iTween.FadeTo(Target, 0.5f, 1);
                }
                else
                {
                    iTween.FadeTo(Target, 1, 1);
                }
                //Debug.Log("near:" + AdjDistance);
                Vector3 B = Quaternion.Euler(CurrentY * SensitivityY, CurrentX * SensitivityX, 0) * new Vector3(0, 0, -AdjDistance);
                B += lookAt.position;
                Debug.Log(AdjDistance);
                SmoothedPosB = Vector3.Lerp(transform.position, B, smoothness);
                // CamTransform.position = SmoothedPosB;
            }
            else
            {
                SmoothedPosB = Vector3.Lerp(transform.position, destination, smoothness);
                //CamTransform.position = SmoothedPosB;
            }
        }
        CamTransform.position = SmoothedPosB;
        CamTransform.LookAt(new Vector3(lookAt.position.x, lookAt.position.y + 0.05f, lookAt.position.z));

    }
    void FixedUpdate()
    {
        CH.UpdateCameraCP(cam.transform.position, cam.transform.rotation, ref CH.adjustedCamCP);
        CH.UpdateCameraCP(destination, cam.transform.rotation, ref CH.desiredCamCP);
        CH.checkingColliding(lookAt.position);
        for (int i = 0; i < 5; i++)
        {
            Debug.DrawLine(lookAt.position, CH.desiredCamCP[i], Color.white);
            Debug.DrawLine(lookAt.position, CH.adjustedCamCP[i], Color.green);
        }
        AdjDistance = CH.GetAdjustedDistance(lookAt.position);
    }
}

[System.Serializable]
public class CollideHandler
{
    public LayerMask collisionLayer;
    public float testNo;

    [HideInInspector]
    public bool colliding = false;
    [HideInInspector]
    public Vector3[] adjustedCamCP;
    [HideInInspector]
    public Vector3[] desiredCamCP;

    Camera camera;
    

    public void initialize(Camera cam)
    {
        camera = cam;
        adjustedCamCP = new Vector3[5];
        desiredCamCP = new Vector3[5];
    }
    public void UpdateCameraCP(Vector3 cameraPos, Quaternion atRotation, ref Vector3[] intoArray)
    {
        if (!camera)
            return;

        intoArray = new Vector3[5];

        float z = camera.nearClipPlane;
        float x = Mathf.Tan(camera.fieldOfView / testNo) * z;
        float y = x / camera.aspect;

        intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPos;
        intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPos;
        intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPos;
        intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPos;
        intoArray[4] = cameraPos - (camera.transform.forward*0.1f);
    }
    bool CollisionDetectedAtCP(Vector3[] clipPoints,Vector3 fromPos)
    {
        for(int i = 0; i < clipPoints.Length; i++)
        {
            Ray ray = new Ray(fromPos, clipPoints[i] - fromPos);
            float distance = Vector3.Distance(clipPoints[i], fromPos);
            if (Physics.Raycast(ray, distance, collisionLayer))
            {
                return true;
                
            }
            
        }
        return false;
    }
   
    public float GetAdjustedDistance(Vector3 from)
    {
        float distance = -1;
        for (int i = 0; i < desiredCamCP.Length; i++)
        {
            Ray ray = new Ray(from, desiredCamCP[i] - from);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (distance == -1)
                {
                    distance = hit.distance;
                }
                else
                {
                    if (hit.distance < distance) 
                        distance = hit.distance;
                }
            }
        }
        if (distance == -1)
            return 0;
        else
            return distance;
    }
    public void checkingColliding(Vector3 targetPos)
    {
        if (CollisionDetectedAtCP(desiredCamCP, targetPos))
        {
            colliding = true;
        }else
        {
            colliding = false;
        }
    }
}
