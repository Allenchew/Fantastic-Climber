using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class controller : MonoBehaviour
{

   //   Animator Anim;
    // movement variable
    //　動く配列
    public float MovementSpeed = 0.3f;
    public float ClimbingSpeed = 0.2f;
    public float MouseSensitivity = 5.0f;
    public float jumpSpeed = 0.5f;
    public static bool pulling = false;

    bool isGrounded = true;
    public Rigidbody RB;
    public GameObject Cam;

    //JoyStick
    bool JoyLeft;
    bool JoyRight;
    bool JoyUp;
    bool JoyDown;

    // action varible
    // アクシオン配列
    public GameObject Left;
    public GameObject Right;
    private Vector3 ClimbTargetPos;

    // climbing variable
    //　クライミング配列
    bool TouchingDestinationY;
    float MaxClimbX;
    float MinClimbX;
    float MaxClimbY;
    float MinClimbY;
    float ClimbZ;
    bool ReachTop;
    public float ClimbHeight = 0.2f;

    // handaction
    private Vector3 GrabDirection;
    private bool LeftDetect = false;
    private bool RightDetect = false;
    public GameObject frontCheckTarget;
    private float HitDistance;
    private bool isHit;

    // pull variable
    //引く配列
    GameObject PullTargetTemp;

    // pickup Variable
    public GameObject leg;
    bool isGrabbed;
    public GameObject grabTemp;
    bool isGrabPressed;

    // character state
    // プレイヤーの状態
    public enum State
    {
        Idle = 0,
        Move,
        ClimbLow,
        ClimbHigh,
        Death,
        pull,
    }

    public State PState = State.Idle;

    // Use this for initialization
    void Start()
    {
        //Anim = GetComponent<Animator>();
<<<<<<< HEAD
       // Anim.Stop();
=======
        //Anim.Stop();
>>>>>>> b7cfdf2133e440f5e568b719c59c2cd9952ffde5
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    //climb grab check

    void FrontCheck(GameObject fromObject)
    {
        isHit = false;
        frontCheckTarget = null;
        RaycastHit hit;
        float handOutPos = fromObject.GetComponent<Collider>().bounds.extents.x;
        if (Physics.Raycast(fromObject.transform.position, fromObject.transform.forward, out hit, handOutPos + 0.05f)) {
            if (hit.distance < 0.5f)
            {
                frontCheckTarget = hit.transform.gameObject;
            } else
            {
                Debug.Log("hit nothing");
            }
            isHit = true;
        }

        if (fromObject.name == "Leg")
        {
            Vector3 legRayBtm = new Vector3(leg.transform.position.x, leg.GetComponent<Collider>().bounds.min.y, leg.transform.position.z);
            float legOutPos = leg.GetComponent<Collider>().bounds.extents.x;
            
            if (Physics.Raycast(legRayBtm, leg.transform.forward, out hit, legOutPos + 0.08f, 1 << LayerMask.NameToLayer("GrabAble")))
            {                
                if (hit.distance < 1)
                {
                    frontCheckTarget = hit.transform.gameObject;
                    Debug.Log(frontCheckTarget.gameObject.name);
                } else
                {
                    Debug.Log("hit nothing");
                }
                isHit = true;
            }
        }
                
    }

    // force change angel when facing
    //壁とか引くもの前にちゃんと向かうために強制的にプレイヤーの角度を変更します。
    void CheckFaceAngle()
    {
        if (this.transform.rotation.y > -38 && this.transform.rotation.y < 38)
        {
            transform.eulerAngles = new Vector3(this.transform.rotation.x, 0, this.transform.rotation.z);
        }

        if (this.transform.rotation.y > 48 && this.transform.rotation.y < 128)
        {
            transform.eulerAngles = new Vector3(this.transform.rotation.x, 90, this.transform.rotation.z);
        }

        if (this.transform.rotation.y > 142 && this.transform.rotation.y < 218)
        {
            transform.eulerAngles = new Vector3(this.transform.rotation.x, 180, this.transform.rotation.z);
        }

        if (this.transform.rotation.y > 228 && this.transform.rotation.y < 308)
        {
            transform.eulerAngles = new Vector3(this.transform.rotation.x, 270, this.transform.rotation.z);
        }
    }

    // check if foot touch ground
    // ちゃんと床に触れるか確認
    void CheckGround()
    {
        Vector3 rayStart = leg.transform.position + Vector3.down * this.gameObject.GetComponent<Collider>().bounds.size.y * 0.5f;
        Debug.DrawRay(rayStart, -Vector3.up, Color.green);
        RaycastHit hit;
        float distToGround = this.gameObject.GetComponent<Collider>().bounds.extents.y;
        if (Physics.Raycast(rayStart, -Vector3.up, out hit))
        {
            Debug.Log(hit.collider.gameObject.name);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    
    // climbing function
    // クライミング関数
    IEnumerator Climbing(Vector3 NewPosY)
    {
        float TestSpeed = 0.5f;
        while (Vector3.Distance(transform.position, NewPosY) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, NewPosY, TestSpeed * Time.deltaTime);
            yield return null;
        }
        Vector3 newPoZ = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.2f);
        while (Vector3.Distance(transform.position, newPoZ) > 0.005f)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPoZ, TestSpeed * Time.deltaTime);
            yield return null;
        }
        CheckGround();
        if (isGrounded == true)
        {
            PState = State.Idle;
        }
        PState = State.Idle;
        ReachTop = false;
    }

    IEnumerator GrabItem(GameObject grabObject)
    {
        grabTemp = null;
        isGrabbed = false;
        while (Vector3.Distance(grabObject.transform.position, Right.transform.position) > 0.05f)
        {
            grabObject.GetComponent<Rigidbody>().useGravity = false;
            grabObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            grabObject.transform.position = Vector3.MoveTowards(grabObject.transform.position, Right.transform.position, 0.5f * Time.deltaTime);
            yield return null;
        }
        grabObject.transform.position = Right.transform.position;
        isGrabbed = true;
        grabTemp = grabObject;
    }


    void Climb(GameObject climbTarget)
    {
        float CTHeight = climbTarget.gameObject.GetComponent<Collider>().bounds.extents.y;
        if (CTHeight < 0.2 && PState == State.Idle)
        {
            PState = State.ClimbLow;
            Debug.Log("ShortClimb");
            ReachTop = true;
        }
        else if (CTHeight >= 0.1 && PState == State.Idle)
        {
            PState = State.ClimbHigh;
            Debug.Log("LongClimb");
        }
    }

    // pull fucntion
    // 引く関数
    void Pull(GameObject PullTarget)
    {
        PullTargetTemp = PullTarget;
        PState = State.pull;
    }


    // Update is called once per frame
    void Update()
    {
        switch (PState)
        {
            case State.Idle:
                {
                    Debug.Log("entered");
                    Cam.GetComponent<MainCamera>().Pulling = false;
                    Cam.GetComponent<MainCamera>().Climbing = false;
                    Cam.GetComponent<MainCamera>().PullBag = false;
                    Cam.GetComponent<MainCamera>().Onbag = false;

                    if (Input.GetKey(KeyCode.W) || JoyUp == true)
                    {
                        transform.position += transform.forward * MovementSpeed * Time.deltaTime;
                        transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
                    }            

                    if (Input.GetKey(KeyCode.S) || JoyDown == true)
                    {
                        transform.position -= transform.forward * MovementSpeed * Time.deltaTime;
                        transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
                    }

                    if (Input.GetKey(KeyCode.A) || JoyLeft == true)
                    {
                        transform.position -= transform.right * MovementSpeed * Time.deltaTime;
                    }

                    if (Input.GetKey(KeyCode.D) || JoyRight == true)
                    {
                        transform.position += transform.right * MovementSpeed * Time.deltaTime;
                    }

                    break;
                }
            case State.pull:
                {
                    if (PState == State.pull && (Input.GetKey(KeyCode.W) || JoyUp == true))
                    {
                        PullTargetTemp.GetComponent<drawer>().checkDrawer();
                        Cam.GetComponent<MainCamera>().Pulling = true;
                        transform.position += transform.forward * MovementSpeed * Time.deltaTime;
                        PullTargetTemp.transform.position += transform.forward * MovementSpeed * Time.deltaTime;

                    }
                    if (PState == State.pull && (Input.GetKey(KeyCode.S) || JoyDown == true))
                    {
                        PullTargetTemp.GetComponent<drawer>().checkDrawer();
                        Cam.GetComponent<MainCamera>().Pulling = true;
                        if (PullTargetTemp.GetComponent<drawer>().isReachLimit)
                        {
                            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
                            PullTargetTemp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
                        }
                        else
                        {
                            transform.position -= transform.forward * MovementSpeed * Time.deltaTime;
                            PullTargetTemp.transform.position -= transform.forward * MovementSpeed * Time.deltaTime;
                        }
                    }
                    break;
                }

        }

        // controller input
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("A");
        }
        if (Input.GetAxis("JoystickX") >= 0.8f)
        {
            JoyRight = true;
        }
        if (Input.GetAxis("JoystickX") <= -0.8f)
        {
            JoyLeft = true;
        }
        if (Input.GetAxis("JoystickX") >= -0.5f && Input.GetAxis("JoystickX") <= 0.5f)
        {
            JoyRight = false;
            JoyLeft = false;
        }
        if (Input.GetAxis("JoystickY") <= -0.8f)
        {
            JoyUp = true;
        }
        if (Input.GetAxis("JoystickY") >= 0.8f)
        {
            JoyDown = true;
        }
        if (Input.GetAxis("JoystickY") >= -0.5f && Input.GetAxis("JoystickY") <= 0.5f)
        {
            JoyUp = false;
            JoyDown = false;
        }


<<<<<<< HEAD
            if (Input.GetKey(KeyCode.W) || JoyUp == true)
            {
               // Anim.Play("walk");
             //.   RB.constraints = RigidbodyConstraints.FreezePositionY;
                transform.position += transform.forward * MovementSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            } /*else if (Input.GetKeyUp(KeyCode.W) || JoyUp == false)
            {
                RB.constraints = RigidbodyConstraints.None;
            }*/

            if (Input.GetKey(KeyCode.S) || JoyDown == true)
            {
                transform.position -= transform.forward * MovementSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            }

            if (Input.GetKey(KeyCode.A) || JoyLeft == true)
            {
                transform.position -= transform.right * MovementSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D) || JoyRight == true)
            {
                transform.position += transform.right * MovementSpeed * Time.deltaTime;
            }
        }
        // if state is climbing high
=======
>>>>>>> b7cfdf2133e440f5e568b719c59c2cd9952ffde5
        if (PState == State.ClimbHigh)
        {
            MaxClimbX = frontCheckTarget.gameObject.GetComponent<Collider>().bounds.size.x;
            MinClimbX = frontCheckTarget.gameObject.GetComponent<Collider>().bounds.min.x;
            MaxClimbY = frontCheckTarget.gameObject.GetComponent<Collider>().bounds.max.y;
            MinClimbY = frontCheckTarget.gameObject.GetComponent<Collider>().bounds.min.y;
            ClimbZ = transform.position.z;

            if (transform.position.x > MaxClimbX)
            {
                transform.position = new Vector3(MaxClimbX, transform.position.y, ClimbZ);
            }

            if (transform.position.x < MinClimbX)
            {
                transform.position = new Vector3(MinClimbX, transform.position.y, ClimbZ);
            }

            if (transform.position.y > MaxClimbY - 0.05)
            {
                ReachTop = true;
            }

            if (transform.position.y < MinClimbY)
            {
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

            if (Input.GetKey(KeyCode.W) || JoyUp == true)
            {
                transform.position += transform.up * ClimbingSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S) || JoyDown == true)
            {
                transform.position -= transform.up * ClimbingSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A) || JoyLeft == true)
            {
                transform.position -= transform.right * ClimbingSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D) || JoyDown == true)
            {
                transform.position += transform.right * ClimbingSpeed * Time.deltaTime;
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
        Vector3 CurrPost = new Vector3(transform.position.x, transform.position.y + ClimbHeight, transform.position.z); ;
        if (ReachTop == true)
        {
            StartCoroutine(Climbing(CurrPost));
            PState = State.ClimbLow;
            ReachTop = false;
        }
<<<<<<< HEAD

        if (PState == State.pull && (Input.GetKey(KeyCode.W) || JoyUp == true))
        {
            PullTargetTemp.GetComponent<drawer>().checkDrawer();
            Cam.GetComponent<MainCamera>().Pulling = true;
            transform.position += transform.forward * MovementSpeed * Time.deltaTime;
            PullTargetTemp.transform.position += transform.forward * MovementSpeed * Time.deltaTime;

        }
        if (PState == State.pull && (Input.GetKey(KeyCode.S) || JoyDown == true))
        {
            PullTargetTemp.GetComponent<drawer>().checkDrawer();
            Cam.GetComponent<MainCamera>().Pulling = true;
            if (PullTargetTemp.GetComponent<drawer>().isReachLimit)
            {
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
                PullTargetTemp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
            }
            else
            {
                transform.position -= transform.forward * MovementSpeed * Time.deltaTime;
                PullTargetTemp.transform.position -= transform.forward * MovementSpeed * Time.deltaTime;
            }
        }

        // climbing Mode
        //プレイヤーの状態はclimbhighになったら
        if (PState == State.ClimbHigh && (Input.GetKey(KeyCode.W) || JoyUp == true))
        {
            transform.position += transform.up * ClimbingSpeed * Time.deltaTime;
        }
        if (PState == State.ClimbHigh && (Input.GetKey(KeyCode.S) || JoyDown == true))
        {
            transform.position -= transform.up * ClimbingSpeed * Time.deltaTime;
        }
        if (PState == State.ClimbHigh && (Input.GetKey(KeyCode.A) || JoyLeft == true))
        {
            transform.position -= transform.right * ClimbingSpeed * Time.deltaTime;
        }
        if (PState == State.ClimbHigh && (Input.GetKey(KeyCode.D) || JoyDown == true))
        {
            transform.position += transform.right * ClimbingSpeed * Time.deltaTime;
        }

=======
        
>>>>>>> b7cfdf2133e440f5e568b719c59c2cd9952ffde5
        //jump + ground check
        //スパイスキー押すとgroundcheckとジャンプ作業
        CheckGround();
        if (isGrounded && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Joystick1Button3)) && (PState == State.Idle || PState == State.Move))
        {
            RB.AddForce(0, jumpSpeed, 0, ForceMode.Impulse);
        }

        // climb
        //Rボタン押すとくらいんクライミング作業
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            CheckFaceAngle();
            FrontCheck(Left);

            if (isHit && frontCheckTarget != null && (frontCheckTarget.layer == 9 || frontCheckTarget.layer == 13)) {
                Climb(frontCheckTarget);
            }
        }

        //pull
        //Eボタン押すと引く作業
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            CheckFaceAngle();
            FrontCheck(Left);

            if (isHit && PullTargetTemp == null && (frontCheckTarget.layer == 12 | frontCheckTarget.layer == 13))
            {
                Debug.Log("entered");
                pulling = true;
                Pull(frontCheckTarget);
            }
            else
            {
                PState = State.Idle;
                pulling = false;
                PullTargetTemp = null;
            }
        }
        // grab item
        // Q 押すとアイテム拾う作業
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Joystick1Button2))
        {            
            FrontCheck(leg);

            if (frontCheckTarget == null)
            {
                Debug.Log("touch nothing");
            }
            else if (frontCheckTarget.layer == 14 && grabTemp == null )
            {
                Debug.Log("touch pen");
                isGrabPressed = !isGrabPressed;
                StartCoroutine(GrabItem(frontCheckTarget));
                isGrabPressed = false;
            }

            if (isGrabPressed == false && grabTemp != null)
            {
                RaycastHit hit;
                float handOutPos = Right.GetComponent<Collider>().bounds.extents.x;
                isGrabbed = false;
                grabTemp.GetComponent<Rigidbody>().useGravity = true;
                Debug.Log("drop");
<<<<<<< HEAD
=======

>>>>>>> b7cfdf2133e440f5e568b719c59c2cd9952ffde5
                if (Physics.Raycast(Right.transform.position, Right.transform.forward, out hit, 0.44f) && hit.transform.name == "BookShelf")
                {
                    Debug.Log("drop in front shelf");
                    grabTemp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    grabTemp.transform.eulerAngles = new Vector3(0, 90, 18.06f);
                    grabTemp.transform.position = new Vector3(transform.position.x, 0.73f, -0.96f);
                }
                grabTemp = null;
            }
        }

        if (isGrabbed == true)
        {
            grabTemp.transform.rotation = transform.rotation;
            grabTemp.transform.position = new Vector3(Right.transform.position.x, Right.transform.position.y + 0.01f, Right.transform.position.z);
        }
        Debug.Log(isGrounded);
    }
}




