﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour
{
    // movement variable
    public float MovementSpeed = 0.3f;
    public float ClimbingSpeed = 0.2f;
    public float MouseSensitivity = 5.0f;
    public float jumpSpeed = 0.5f;
    bool isGrounded = true;
    public Rigidbody RB;
    public GameObject Cam;

    // action varible
    public GameObject Left;
    public GameObject Right;
    public Vector3 ClimbTargetPos;

    // climbing variable
    bool TouchingDestinationY;
    public LayerMask ClimbAbleLayer;
    float t;
    float MaxClimbX;
    float MinClimbX;
    float MaxClimbY;
    float MinClimbY;
    float ClimbZ;
    bool ReachTop;

    public enum State
    {
        Idle = 0,
        Move,
        ClimbLow,
        ClimbHigh,
        Death,
    }

    public State PState = State.Idle;

    // Use this for initialization
    void Start()
    {
        Rigidbody RB = this.gameObject.GetComponent<Rigidbody>();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    public void CheckGround()
    {
        float distToGround = this.gameObject.GetComponent<Collider>().bounds.extents.y;
        if (Physics.Raycast(transform.position, -Vector3.up, distToGround + 1 / 10))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void Teleport(GameObject climbTarget)
    {
        Vector3 temp = transform.position;
        temp.y = climbTarget.gameObject.transform.position.y + 0.1f;
        //climbTarget.gameObject.GetComponent<Collider>().bounds.extents.y;
        transform.position = (transform.forward * 0.2f) + temp;
        Debug.Log(transform.position);
    }

    void ReachTopYCheck()
    {
        //Debug.Log("called");
        RaycastHit hit;
        float LegDistY = this.gameObject.GetComponent<Collider>().bounds.min.y;
        Vector3 LegPosition = new Vector3(this.gameObject.transform.position.x, LegDistY, this.gameObject.transform.position.z);
        //Debug.DrawLine(LegPosition, transform.forward * 100, Color.red);
        if (Physics.Raycast(LegPosition, transform.forward, out hit, 0.1f, ClimbAbleLayer))
        {
            //Debug.Log("hitsomething on leg");
            TouchingDestinationY = true;
        }
        else TouchingDestinationY = false;
    }

    IEnumerator Climbing(Vector3 NewPosY)
    {
        ReachTopYCheck();
        float TestSpeed = 0.5f;
        Debug.Log(NewPosY);
        while (Vector3.Distance(transform.position, NewPosY) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, NewPosY, TestSpeed * Time.deltaTime);
            // transform.position = transform.up * TestSpeed;
            yield return null;
        }
        Vector3 newPoZ = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.2f);
        while (Vector3.Distance(transform.position, newPoZ) > 0.005f)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPoZ, TestSpeed * Time.deltaTime);
            yield return null;
        }
        PState = State.Idle;
        ReachTop = false;
    }



    public void Climb(GameObject climbTarget)
    {
        float CTHeight = climbTarget.gameObject.GetComponent<Collider>().bounds.extents.y;
        //Debug.Log(CTHeight);
        if (CTHeight < 0.2 && PState == State.Idle)
        {
            Vector3 tempTar = new Vector3(transform.position.x, CTHeight, transform.position.z);
            PState = State.ClimbLow;
            Debug.Log("ShortClimb");
            StartCoroutine(Climbing(tempTar));
        }
        else if (CTHeight >= 0.1 && PState == State.Idle)
        {
            PState = State.ClimbHigh;
            Debug.Log("LongClimb");
        }
    }


    // Update is called once per frame
    void Update()
    {
        //characater movement

        if (PState == State.Idle)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * MovementSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= transform.forward * MovementSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.position -= transform.right * MovementSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * MovementSpeed * Time.deltaTime;
            }
        }
        // if state is climbing high
        if (PState == State.ClimbHigh)
        {
            MaxClimbX = Right.GetComponent<HandAction>().ClimbTargetHand.gameObject.GetComponent<Collider>().bounds.size.x;
            MinClimbX = Right.GetComponent<HandAction>().ClimbTargetHand.gameObject.GetComponent<Collider>().bounds.min.x;
            MaxClimbY = Right.GetComponent<HandAction>().ClimbTargetHand.gameObject.GetComponent<Collider>().bounds.max.y;
            MinClimbY = Right.GetComponent<HandAction>().ClimbTargetHand.gameObject.GetComponent<Collider>().bounds.min.y;
            ClimbZ = transform.position.z;
            if (transform.position.x > MaxClimbX)
            {
                Debug.Log(MaxClimbX);
                Debug.Log("max Right");
                transform.position = new Vector3(MaxClimbX, transform.position.y, ClimbZ);
            }
            if (transform.position.x < MinClimbX)
            {
                Debug.Log(MinClimbX);
                Debug.Log("max left");
                transform.position = new Vector3(MinClimbX, transform.position.y, ClimbZ);
            }
            if (transform.position.y > MaxClimbY - 0.05)
            {
                Debug.Log(MaxClimbY);
                Debug.Log("reach top");
                ReachTop = true;
                //transform.position = new Vector3(transform.position.x, MaxClimbY, ClimbZ);
            }
            if (transform.position.y < MinClimbY)
            {
                Debug.Log(MinClimbY);
                Debug.Log("reach btm");
                transform.position = new Vector3(transform.position.x, MinClimbY, ClimbZ);
            }
            Cam.GetComponent<MainCamera>().Climbing = true;
            if (RB != null)
            {
                RB.useGravity = false;
                RB.constraints = RigidbodyConstraints.FreezePositionZ & RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                Debug.Log("no Rigidbody");
            }

        }
        // state climblow
        else if (PState == State.ClimbLow)
        {
            Cam.GetComponent<MainCamera>().Climbing = true;
            if (RB != null)
            {
                RB.useGravity = false;
                RB.constraints = RigidbodyConstraints.FreezePositionZ & RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                Debug.Log("no Rigidbody");
            }
        }
        else
        {
            RB.useGravity = true;
            Cam.GetComponent<MainCamera>().Climbing = false;
        }

        // starting climbing process
        Vector3 CurrPost = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z); ;
        if (ReachTop == true)
        {
            Debug.Log("calling coroutine");
            StartCoroutine(Climbing(CurrPost));
            PState = State.ClimbLow;
            ReachTop = false;
        }
        // climbing Mode
        if (PState == State.ClimbHigh && Input.GetKey(KeyCode.W))
        {
            transform.position += transform.up * ClimbingSpeed * Time.deltaTime;
        }
        if (PState == State.ClimbHigh && Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.up * ClimbingSpeed * Time.deltaTime;
        }
        if (PState == State.ClimbHigh && Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * ClimbingSpeed * Time.deltaTime;
        }
        if (PState == State.ClimbHigh && Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * ClimbingSpeed * Time.deltaTime;
        }

        //jump + ground check
        CheckGround();
        if (isGrounded && Input.GetButtonDown("Jump") && PState == State.Idle || PState == State.Move)
        {
            RB.AddForce(0, jumpSpeed, 0, ForceMode.Impulse);
        }

        // climb
        if (Input.GetKeyDown(KeyCode.R))
        {
            if ((Left.GetComponent<HandAction>().LeftDetect) && (Right.GetComponent<HandAction>().RightDetect))
                //Debug.Log("execute");
                Climb(Right.GetComponent<HandAction>().ClimbTargetHand);
            //Teleport(Right.GetComponent<HandAction>().ClimbTargetHand);
        }
    }


}




