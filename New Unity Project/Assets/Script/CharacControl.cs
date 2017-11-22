using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{

    Rigidbody RB;
    // movement variable
    public float MovementSpeed = 4.0f;
    public float MouseSensitivity = 4.0f;
    public float jumpSpeed = 10f;
    float VerticalRotation = 0f;
    public float MaxUpDown = 60f;
    float verticalVelocity = 0;

    bool isGrounded = true;
    // grab

    public Collider handCollider;
    Transform PullItem;
    bool grabbed = false;

    // Use this for initialization
    void Start()
    {
    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;
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

    // Update is called once per frame
    void Update()
    {
        Rigidbody RB = this.gameObject.GetComponent<Rigidbody>();

        //characater movement
        if (Input.GetKey(KeyCode.W))
        {
            RB.transform.position += transform.forward * MovementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            RB.transform.position -= transform.forward * MovementSpeed * Time.deltaTime;
            //RB.MovePosition(transform.position - transform.forward * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            RB.transform.position -= transform.right * MovementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            RB.transform.position += transform.right * MovementSpeed * Time.deltaTime;
        }

        // rotation

        //float RotLeftRight = Input.GetAxis("Mouse X") * MouseSensitivity;
        //this.gameObject.transform.Rotate(0, RotLeftRight, 0);

        //VerticalRotation -= Input.GetAxis("Mouse Y") * MouseSensitivity;
        //VerticalRotation = Mathf.Clamp(VerticalRotation, -MaxUpDown, MaxUpDown);
        //Camera.main.transform.localRotation = Quaternion.Euler(VerticalRotation, 0, 0);

        //jump + ground check
        CheckGround();
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            RB.AddForce(0, jumpSpeed, 0, ForceMode.Impulse);
        }


    }

    void DetectCollider()
    {

    }




}
