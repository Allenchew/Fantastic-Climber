using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour {
    // movement variable
    public float MovementSpeed = 0.3f;
    public float MouseSensitivity = 5.0f;
    public float jumpSpeed = 0.5f;
    bool isGrounded = true;
    Rigidbody RB;

    // action varible
    bool ClimbAble = false;
    public GameObject Left;
    public GameObject Right;
    public Vector3 ClimbTargetPos;


    // Use this for initialization
    void Start()
    {
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

    public void Climb(GameObject climbTarget)
    {
        //ClimbTargetPos = new Vector3(this.transform.position.x,climbTarget.gameObject.GetComponent<Collider>().bounds.extents.y,0.05f);

        Vector3 temp = transform.position;
        temp.y = climbTarget.gameObject.transform.position.y + 0.05f;
        //climbTarget.gameObject.GetComponent<Collider>().bounds.extents.y;
        transform.position = (transform.forward * 0.1f) + temp;
        Debug.Log(transform.position);
    }


    // Update is called once per frame
    void Update()
    {
        Rigidbody RB = this.gameObject.GetComponent<Rigidbody>();
        

        //characater movement
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

        //jump + ground check
        CheckGround();
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            RB.AddForce(0, jumpSpeed, 0, ForceMode.Impulse);
        }

        // climb
        if (Input.GetKey(KeyCode.E))
        {
            if ((Left.GetComponent<HandAction>().LeftDetect) && (Right.GetComponent<HandAction>().RightDetect))
            {
                Debug.Log("execute");
                Climb(Right.GetComponent<HandAction>().ClimbTargetHand);
            }
        }


    }

    void DetectCollider()
    {

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
