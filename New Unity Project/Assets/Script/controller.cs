using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour {
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
    bool TouchingDestinationX;
    public LayerMask GrabAbleLayer;

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
        //ClimbTargetPos = new Vector3(this.transform.position.x,climbTarget.gameObject.GetComponent<Collider>().bounds.extents.y,0.05f);

        Vector3 temp = transform.position;
        temp.y = climbTarget.gameObject.transform.position.y + 0.1f;
        //climbTarget.gameObject.GetComponent<Collider>().bounds.extents.y;
        transform.position = (transform.forward * 0.2f) + temp;
        Debug.Log(transform.position);
    }

    void ReachTopYCheck()
    {
       // Debug.Log("called");
        RaycastHit hit;
        float LegDistY = this.gameObject.GetComponent<Collider>().bounds.min.y;
        Vector3 LegPosition = new Vector3(0, LegDistY, 0);
        //Debug.DrawLine(LegPosition, transform.forward * 100,Color );
        //if (Physics.Raycast(transform.position, transform.forward, out hit, LegDist + 0.5f, GrabAbleLayer))
        //{
        //    Debug.Log("hitsomething on leg");
        //}
    }

    //IEnumerable ClimbToTargetX(Vector3 CurrPos, Vector3 targetPos, float XMove, float ZMove)
    //{
    //    Vector3 StartPos = CurrPos;
    //    while (CurrPos.x != XMove)
    //    {
    //        Vector3 NextXPos = new Vector3(CurrPos.x, XMove, CurrPos.z);
    //        CurrPos = Vector3.Lerp(CurrPos, NextXPos, ClimbingSpeed);
    //        yield return null;
    //    }
    //    StartCoroutine(ClimbToTargetZ(CurrPos, targetPos, XMove, ZMove));
    //}

    //IEnumerable ClimbToTargetZ(Vector3 CurrPos, Vector3 targetPos, float ZMove)
    //{
    //    Vector3 StartPos = CurrPos;
    //    while (CurrPos.z != ZMove)
    //    {
    //        Vector3 NextZPos = new Vector3(CurrPos.x, CurrPos.y, ZMove);
    //        CurrPos = Vector3.Lerp(CurrPos, NextZPos, ClimbingSpeed);
    //        yield return null;
    //    }
    //}

    public void Climb(GameObject climbTarget)
    {
        float CTHeight = climbTarget.gameObject.GetComponent<Collider>().bounds.extents.y;
        Debug.Log(CTHeight);
        if (CTHeight < 0.2 && PState == State.Idle)
        {
            PState = State.ClimbLow;
            Debug.Log("ShortClimb");
            Vector3 destination = new Vector3(gameObject.transform.position.x, CTHeight + 1f, transform.position.z+ 0.1f);
            transform.position += new Vector3 (0, CTHeight + 10f,0) * ClimbingSpeed  * Time.deltaTime;

            //float ClimbUpSP = CTHeight ;
            //transform.Translate(0, ClimbUpSP + 0.15f, 0);
            //transform.Translate(0, 0, 0.2f);
            //PState = State.Idle;
        }
        else if (CTHeight >= 0.1 && PState == State.Idle)
        {
            ReachTopYCheck();
            PState = State.ClimbHigh;
            Debug.Log("LongClimb");
            float MaxClimbX = climbTarget.gameObject.GetComponent<Collider>().bounds.max.x;
            float MinClimbX = climbTarget.gameObject.GetComponent<Collider>().bounds.min.x;
            float MaxClimbY = climbTarget.gameObject.GetComponent<Collider>().bounds.max.y;
            float MinClimbY = climbTarget.gameObject.GetComponent<Collider>().bounds.min.y;
            float ClimbZ = Right.GetComponent<HandAction>().HitDistance - 0.5f;
            

            if (transform.position.x > MaxClimbX)
            {
                transform.position = new Vector3(MaxClimbX, transform.position.y, ClimbZ);
            }
            if (transform.position.x < MinClimbX)
            {
                transform.position = new Vector3(MinClimbX, transform.position.y, ClimbZ);
            }
            if (transform.position.y > MaxClimbY - 0.03)
            {
                Debug.Log("reach top");
                transform.position = new Vector3(MaxClimbY, transform.position.y, ClimbZ);
            }
            if (transform.position.y < MinClimbY)
            {
                transform.position = new Vector3(MinClimbY, transform.position.y, ClimbZ);
            }
        }
    }




    // Update is called once per frame
    void Update()
    {
        ReachTopYCheck();
        // state change to idle if not moving
        //if (RB.velocity == Vector3.zero && PState != State.ClimbLow && PState != State.ClimbHigh) PState = State.Idle;

        //characater movement
        if (PState == State.Idle)
        {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * MovementSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y , 0);
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
        if (PState == State.ClimbHigh || PState == State.ClimbLow)
        {
            Cam.GetComponent<MainCamera>().Climbing = true;
            if (RB != null)
            {
                Debug.Log("remove gravity");
                RB.useGravity = false;
                RB.constraints = RigidbodyConstraints.FreezePositionZ & RigidbodyConstraints.FreezeRotation;
             }
            else
            {
                Debug.Log("no Rigidbody");
            }

        } else
        {
            RB.useGravity = true;
            RB.constraints = RigidbodyConstraints.FreezeRotation;
            Cam.GetComponent<MainCamera>().Climbing = false;
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
            {
                Debug.Log("execute");
                Climb(Right.GetComponent<HandAction>().ClimbTargetHand);
                //Teleport(Right.GetComponent<HandAction>().ClimbTargetHand);
            }
        }


    }


}
/*  public Camera cam;
    public bool Climbing=false;
    public Rigidbody rb;
    // Use this for initialization
    
    void Start () {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey("w")== true)
        {
            transform.position += transform.forward*0.01f;
            //transform.rotation *= new Quaternion(0,cam.transform.rotation.y,0,0);
            transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
        }
        else if (Input.GetKey("s") == true)
        {
            transform.position -= new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z) * 0.01f;
            transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
        }
        else if (Input.GetKey("a") == true)
        {
            transform.position -= transform.right*0.01f;
            //transform.rotation = new Quaternion(0, cam.transform.rotation.y, 0, 0);
        }
        else if(Input.GetKey("d") == true)
        {
            transform.position += transform.right*0.01f;
           // transform.rotation = new Quaternion(0, cam.transform.rotation.y, 0, 0);
        }else if (Input.GetKeyDown("space") == true)
        {
            rb.AddForce(0, 100, 0);
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Finish")
        {
            Climbing = true;
            rb.useGravity = false;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Finish")
        {
            Climbing = false;
            rb.useGravity = true;
        }
    }*/
