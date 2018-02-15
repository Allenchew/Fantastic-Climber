using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class controller : MonoBehaviour
{

    Animator Anim;
    // movement variable
    //　動く配列
    private float walkingspd = 0.0004f;
    private bool trigger = false;
    private bool ClimbAnim = false;

    public float MovementSpeed = 0.0003f;
    public float ClimbingSpeed = 0.2f;
    public float MouseSensitivity = 5.0f;
    public float jumpSpeed = 0.5f;
    public static bool pulling = false;
    public bool PlayM = false;
    public GameObject GrabRope;

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
    public float dist;
    private bool isHit;

    // pull variable
    //引く配列
    GameObject PullTargetTemp;

    // pickup Variable
    public GameObject leg;
    bool isGrabbed;
    public GameObject grabTemp;
    bool isGrabPressed;

    //climbing rope
    public LayerMask Rope;
    public LayerMask RopeEnd;
    public LayerMask Detection;
    public Vector3 hitpos;
    public GameObject TpRop;
    public bool Trigg = false;

    private bool EndClimb = true;
    private bool HeadAlert = false;
    
    private bool RopeAnim = false;
    private Vector3 nextParent;

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
        Anim = GetComponent<Animator>();
        RB = GetComponent<Rigidbody>();
        //Anim.Stop();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    //climb grab check

    //climb grab check

    public GameObject DistCheck(string CheckObject)
    {
        Debug.Log("entered");
        GameObject[] GrabObjects;
        GrabObjects = GameObject.FindGameObjectsWithTag(CheckObject);
        GameObject Closest = null;
        float distance = Mathf.Infinity;

        foreach (GameObject GO in GrabObjects)
        {
            Vector3 diff = GO.transform.position - transform.position;
            float curDist = diff.sqrMagnitude;
            if (curDist < distance)
            {
                Closest = GO;
                distance = curDist;
            }
        }
        return Closest;
    }
    void FrontCheck(GameObject fromObject)
    {
        isHit = false;
        frontCheckTarget = null;
        RaycastHit hit;
        float handOutPos = fromObject.GetComponent<Collider>().bounds.extents.x;
        if (Physics.Raycast(fromObject.transform.position, fromObject.transform.forward, out hit, handOutPos + 0.05f))
        {
            if (hit.distance < 0.5f)
            {
                frontCheckTarget = hit.transform.gameObject;
            }
            else
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
                }
                else
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
        Vector3 rayStart = new Vector3(transform.position.x, (transform.position.y + 0.01f), transform.position.z);
        Debug.DrawRay(rayStart, -Vector3.up, Color.red);
        RaycastHit hit;
        float distToGround = this.gameObject.GetComponent<Collider>().bounds.extents.y;
        if (Physics.Raycast(rayStart, -Vector3.up, out hit, 0.01f))
        {
            isGrounded = true;
            Anim.SetBool("Jump", false);
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
        yield return new WaitForSeconds(0.5f);
        while (Vector3.Distance(grabObject.transform.position, Left.transform.position) > 0.05f)
        {
            grabObject.GetComponent<Rigidbody>().useGravity = false;
            grabObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            grabObject.transform.position = Vector3.MoveTowards(grabObject.transform.position, Left.transform.position, 0.5f * Time.deltaTime);
            
            yield return null;
        }
        grabObject.transform.position = Left.transform.position;
        
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

    IEnumerator FinClimb(int BaseNum, int Progression)
    {
        for (int i = (BaseNum == 0 ? BaseNum : BaseNum - 1); (BaseNum == 0 ? i < 3 : i >= 0); i += Progression)
        {
            for (int j = 0; j < 10; j++)
            {
                switch (i)
                {
                    case 0:
                        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, nextParent.y, transform.position.z), 0.1f * j);
                        yield return new WaitForSeconds(0.01f);
                        break;
                    case 1:
                        transform.RotateAround(hitpos, Vector3.up, Progression * 720 * Time.deltaTime);
                        if (Progression < 0) { transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(0, 180, 0, 0), 0.1f * j); }
                        yield return new WaitForSeconds(0.01f);
                        break;

                    case 2:
                        transform.position -= new Vector3(0, 0, 0.001f * Progression);
                        break;

                }
            }
        }
        RB.isKinematic = Progression > 0 ? false : true;
    }
    IEnumerator SmoothMov(Vector3 Dest)
    {
        RopeAnim = true;
        for (int i = 0; i < 10; i++)
        {
            transform.position = Vector3.Lerp(transform.position, Dest, 0.1f * i);
            yield return new WaitForSeconds(0.01f);
        }
        StopCoroutine("SmoothMov");
        Anim.SetBool("RopeUp", false);
        RopeAnim = false;
    }
    IEnumerator TopRp(Quaternion Rot)
    {
        for (int i = 0; i < 10; i++)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Rot, 0.1f * i);
            yield return new WaitForSeconds(0.01f);
        }
        StopCoroutine("TopRp");
    }


    //private void OnCollisionEnter(Collision collision)
    //{
    //    float HitDistance;
    //    List<GameObject> hitObject = new List<GameObject>();

    //    foreach (GameObject HitItem in collision)
    //    {
    //        hitObject.Add(HitItem);
    //    }
    //    for (var i = 0; i < hitObject.Count; i++)
    //    {
    //        HitDistance = Vector3.Distance(transform.position, hitObject[i].transform.position);
    //    }
    //}


    // Update is called once per frame
    void Update()
    {
        if (!ClimbAnim)
        {
            // Debug.Log(Vector2.Distance(new Vector2(Input.GetAxis("JoystickX"), -Input.GetAxis("JoystickY")), new Vector2(0, 0)));
            // Debug.Log(Input.GetAxis("JoystickY"));
            switch (PState)
            {
                case State.Idle:
                    {

                        if (PlayM== true)
                        {
                            Cam.GetComponent<MainCamera>().Pulling = false;
                            Cam.GetComponent<MainCamera>().Climbing = false;
                            Cam.GetComponent<MainCamera>().PullBag = false;
                            Cam.GetComponent<MainCamera>().Onbag = false;
                            Vector2 dis = new Vector2(Input.GetAxis("JoystickX"), -Input.GetAxis("JoystickY"));
                            if (Vector2.Distance(dis, new Vector2(0, 0)) > 0.4f)
                            {
                                if (Vector2.Distance(dis, new Vector2(0, 0)) < 1f)
                                {
                                    walkingspd = 0.004f;
                                    Anim.SetBool("walking", true);
                                    Anim.SetBool("running", false);
                                }
                                else
                                {
                                    walkingspd = 0.008f;
                                    Anim.SetBool("running", true);
                                    Anim.SetBool("walking", false);
                                }

                                transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y + Mathf.Atan2(Input.GetAxis("JoystickX"), -Input.GetAxis("JoystickY")) * Mathf.Rad2Deg, 0);

                                transform.position += transform.forward * Vector2.Distance(dis, new Vector2(0, 0)) * walkingspd;
                            }
                            else
                            {
                                Anim.SetBool("walking", false);
                                Anim.SetBool("running", false);
                            }
                            // transform.position -= Camera.main.transform.forward * MovementSpeed / 2 * Time.deltaTime;
                            // transform.position += Camera.main.transform.right * MovementSpeed / 2 * Time.deltaTime;
                        }
                        else
                        {

                            bool KeyW;
                            bool KeyS;
                            bool KeyA;
                            bool KeyD;
                            bool keyWA;
                            bool keyWD;
                            bool keyAS;
                            bool keySD;



                            if (Input.GetKey(KeyCode.W))
                            {
                                KeyW = true;
                            }
                            else
                            {
                                KeyW = false;
                            }

                            if (Input.GetKey(KeyCode.S))
                            {
                                KeyS = true;
                            }
                            else
                            {
                                KeyS = false;
                            }

                            if (Input.GetKey(KeyCode.A))
                            {
                                KeyA = true;
                            }
                            else
                            {
                                KeyA = false;
                            }

                            if (Input.GetKey(KeyCode.D))
                            {
                                KeyD = true;
                            }
                            else
                            {
                                KeyD = false;
                            }

                            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
                            {
                                keyWA = true;
                                KeyW = false;
                                KeyA = false;
                            }
                            else
                            {
                                keyWA = false;
                            }

                            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
                            {
                                keyWD = true;
                                KeyW = false;
                                KeyD = false;
                            }
                            else
                            {
                                keyWD = false;
                            }

                            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
                            {
                                keyAS = true;
                                KeyA = false;
                                KeyS = false;
                            }
                            else
                            {
                                keyAS = false;
                            }

                            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
                            {
                                keySD = true;
                                KeyS = false;
                                KeyD = false;
                            }
                            else
                            {
                                keySD = false;
                            }



                            if (KeyW || JoyUp == true)
                            {
                                transform.position += Camera.main.transform.forward * MovementSpeed * Time.deltaTime;
                                transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
                            }

                            if (KeyS || JoyDown == true)
                            {
                                transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y + 180, 0);
                                transform.position -= Camera.main.transform.forward * MovementSpeed * Time.deltaTime;
                            }

                            if (KeyA || JoyLeft == true)
                            {
                                transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y + 270, 0);
                                transform.position -= Camera.main.transform.right * MovementSpeed * Time.deltaTime;
                            }

                            if (KeyD || JoyRight == true)
                            {
                                transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y + 90, 0);
                                transform.position += Camera.main.transform.right * MovementSpeed * Time.deltaTime;
                            }

                            if (keyWA)
                            {
                                transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y - 45, 0);
                                transform.position += Camera.main.transform.forward * MovementSpeed / 2 * Time.deltaTime;
                                transform.position -= Camera.main.transform.right * MovementSpeed / 2 * Time.deltaTime;
                            }

                            if (keyWD)
                            {
                                transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y + 45, 0);
                                transform.position += Camera.main.transform.forward * MovementSpeed / 2 * Time.deltaTime;
                                transform.position += Camera.main.transform.right * MovementSpeed / 2 * Time.deltaTime;
                            }

                            if (keyAS)
                            {
                                transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y + 225, 0);
                                transform.position -= Camera.main.transform.forward * MovementSpeed / 2 * Time.deltaTime;
                                transform.position -= Camera.main.transform.right * MovementSpeed / 2 * Time.deltaTime;
                            }

                            if (keySD)
                            {
                                transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y + 135, 0);
                                transform.position -= Camera.main.transform.forward * MovementSpeed / 2 * Time.deltaTime;
                                transform.position += Camera.main.transform.right * MovementSpeed / 2 * Time.deltaTime;
                            }
                        }
                        break;
                    }
                case State.pull:
                    {
                        if (PState == State.pull && (Input.GetKey(KeyCode.W) || JoyUp == true))
                        {
                            Anim.speed = 1;
                            if (!trigger)
                            {
                                Anim.SetBool("pushing", true);
                                trigger = true;
                            }
                            PullTargetTemp.GetComponent<drawer>().checkDrawer();
                            Cam.GetComponent<MainCamera>().Pulling = true;
                            transform.position += transform.forward * MovementSpeed * Time.deltaTime;
                            PullTargetTemp.transform.position += transform.forward * MovementSpeed * Time.deltaTime;
                        }
                        else if (PState == State.pull && (Input.GetKey(KeyCode.S) || JoyDown == true))
                        {
                            Anim.speed = 1;
                            if (!trigger)
                            {
                                Anim.SetBool("pushing", false);
                                trigger = true;
                            }
                            PullTargetTemp.GetComponent<drawer>().checkDrawer();
                            Cam.GetComponent<MainCamera>().Pulling = true;

                            if (PullTargetTemp.GetComponent<drawer>().isReachLimit)
                            {
                                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                                PullTargetTemp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                            }
                            else
                            {
                                transform.position -= transform.forward * MovementSpeed * Time.deltaTime;
                                PullTargetTemp.transform.position -= transform.forward * MovementSpeed * Time.deltaTime;
                            }
                        }
                        else
                        {
                            Anim.speed = 0;
                            trigger = false;
                        }
                        break;
                    }


            }

            // controller input
            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Joystick1Button7))
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

            //jump + ground check
            //スパイスキー押すとgroundcheckとジャンプ作業
            CheckGround();
            if (isGrounded && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Joystick1Button3)) && (PState == State.Idle || PState == State.Move))
            {
                Anim.SetBool("Jump", true);
                RB.AddForce(0, jumpSpeed, 0, ForceMode.Impulse);
            }

            // climb
            //Rボタン押すとくらいんクライミング作業
            if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            {

                CheckFaceAngle();
                FrontCheck(Left);

                if (isHit && frontCheckTarget != null && (frontCheckTarget.layer == 13 || frontCheckTarget.layer == 15))
                {
                    if (frontCheckTarget.layer == 15)
                    {
                        ClimbAnim = true;
                        StartCoroutine(Goup(1.5f));
                        Anim.SetBool("Climbing", true);
                    }
                    else
                    {
                        ClimbAnim = true;
                        StartCoroutine(Goup(1));
                        Anim.SetBool("Climbing", true);
                        //Climb(frontCheckTarget);
                    }
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
                    Anim.SetBool("walking", false);
                    Anim.SetBool("running", false);
                    Anim.SetBool("grabbing", true);
                    Pull(frontCheckTarget);
                }
                else
                {
                    PState = State.Idle;
                    pulling = false;
                    Anim.speed = 1;
                    Anim.SetBool("grabbing", false);
                    PullTargetTemp = null;
                }
            }
            // grab item
            // Q 押すとアイテム拾う作業
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                GameObject ClosestObject = null;
                ClosestObject = DistCheck("GrabAble");

                if (ClosestObject == null)
                {
                    Debug.Log("nothing");
                }
                else if (ClosestObject)
                {
                    dist = Vector3.Distance(ClosestObject.transform.position, transform.position);
                    Debug.Log("closet pen " + dist);
                    if (dist < 0.08 && grabTemp == null)
                    {
                        isGrabPressed = !isGrabPressed;
                        Anim.SetBool("grabPencil", true);
                        ClosestObject.GetComponent<Rigidbody>().isKinematic = true;
                        StartCoroutine(GrabItem(ClosestObject));
                        isGrabPressed = false;
                    }
                }
                if (isGrabPressed == false && grabTemp != null)
                {
                    RaycastHit hit;
                    float handOutPos = Right.GetComponent<Collider>().bounds.extents.x;
                    isGrabbed = false;
                    Anim.SetBool("grabPencil", false);
                    grabTemp.GetComponent<Rigidbody>().isKinematic = false;
                    grabTemp.GetComponent<Rigidbody>().useGravity = true;
                    Debug.Log("drop");
                    if (Physics.Raycast(Right.transform.position, Right.transform.forward, out hit, 0.5f) && hit.transform.name == "BookShelf")
                    {
                        Debug.Log("drop in front shelf");
                        grabTemp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        grabTemp.transform.eulerAngles = new Vector3(0,90, 18.06f);
                        grabTemp.transform.position = new Vector3(transform.position.x, 0.73f, -0.96f);
                    }
                    grabTemp = null;
                }
            }

            if (isGrabbed == true)
            {
                grabTemp.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y -90, transform.eulerAngles.z);
                grabTemp.transform.position = new Vector3(Left.transform.position.x, Left.transform.position.y + 0.01f, Left.transform.position.z);
            }
        }

    }
    IEnumerator Goup(float Range)
    {
        RB.useGravity = false;
        RB.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(0.6f);
        for(int i = 0; i < 10; i++)
        {
            transform.position += new Vector3(0, 0.01f*Range, 0);
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(0.3f);
        for(int j = 0; j < 10; j++)
        {
            transform.position += new Vector3(0, 0.005f * Range, 0.0025f);
            yield return new WaitForSeconds(0.018f);
        }
        yield return new WaitForSeconds(0.1f);
        for (int k = 0; k< 10; k++)
        {
            transform.position += new Vector3(0, 0.005f * Range, 0.0025f);
            yield return new WaitForSeconds(0.018f);
        }
        RB.useGravity = true;
        RB.constraints = RigidbodyConstraints.FreezeRotation;
        ClimbAnim = false;
        Anim.SetBool("Climbing", false);
    }

    void FixedUpdate()
    {
        RaycastHit FindParent;
        RaycastHit CurrentParent;
        RaycastHit PastParent;
        RaycastHit HeadDetect;
        //transform.rotation = FindParent.transform.rotation;
        Debug.DrawRay(transform.position, (transform.up * 0.1f), Color.blue);
        if(Physics.Raycast(transform.position - transform.forward * 0.06f, (transform.forward), out CurrentParent, 1, Rope))
        {
            hitpos = CurrentParent.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            hitpos =   GrabRope.transform.position;

            if (Vector3.Distance(transform.position, GrabRope.transform.position) < 0.07f)
            {
                EndClimb = false;
                if (Trigg == false)
                {
                    Trigg = true;
                    Anim.SetBool("grabRope", true);
                    RB.isKinematic = true;
                    transform.position =GrabRope.transform.position - new Vector3(0, 0, 0.006f);
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                }
                else if (Trigg == true)
                {
                    Trigg = false;
                    Anim.SetBool("grabRope", false);
                    RB.isKinematic = false;
                }
            }
        }
        if (Trigg)
        {

            if (Input.GetKey(KeyCode.C))
            {
                transform.RotateAround(hitpos, Vector3.up, 90 * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.V))
            {
                transform.RotateAround(hitpos, Vector3.up, -90 * Time.deltaTime);
            }

            if (Physics.Raycast(transform.position, (transform.up), out HeadDetect, 0.1f, Detection))
            {
                HeadAlert = true;
            }
            else
            {
                HeadAlert = false;
            }
            Debug.DrawRay(transform.position - transform.forward*0.05f, (transform.forward) * 0.06f + transform.up*0.06f,Color.green);
            if (Physics.Raycast(transform.position - transform.forward * 0.06f, (transform.forward) * 0.06f + transform.up * 0.06f, out FindParent, 1,Rope))
            {
               // Debug.Log(FindParent.transform.name);
                if (Input.GetKeyDown(KeyCode.Z) && !RopeAnim && !HeadAlert)
                {
                    if (FindParent.transform.tag == "TopRope")
                    {
                        Debug.Log("Top");
                        var nextpos = FindParent.transform.position - new Vector3(0, 0, 0.006f);
                        var nextRot = new Quaternion(0, 180, 0, 0);
                        Anim.SetBool("RopeUp", true);
                        StartCoroutine(SmoothMov(nextpos));
                        StartCoroutine(TopRp(nextRot));
                        
                    }
                    else if (FindParent.transform.tag != "TopRope")
                    {
                        Debug.Log("Norm");
                        var nextpos = FindParent.transform.position - (hitpos - transform.position);
                        Anim.SetBool("RopeUp", true);
                        StartCoroutine(SmoothMov(nextpos));
                    }

                }
            }
            if (Physics.Raycast(transform.position - transform.forward * 0.05f, (transform.forward) * 0.05f + transform.up * 0.06f, out FindParent, 1, RopeEnd))
            {
                if (Input.GetKeyDown(KeyCode.Z) && !RopeAnim && !HeadAlert)
                {
                    nextParent = FindParent.transform.position;
                    StartCoroutine(FinClimb(0, 1));
                    EndClimb = true;
                    Trigg = false;
                }
            }
            if (Physics.Raycast(transform.position - transform.forward * 0.05f, (transform.forward) * 0.05f - transform.up * 0.06f, out PastParent, 1, Rope) && !EndClimb)
            {
                Debug.Log(PastParent.transform.name);
                if (Input.GetKeyDown(KeyCode.X) && !RopeAnim && Trigg)
                {
                    var Prepos = PastParent.transform.position - (hitpos - transform.position);
                    StartCoroutine(SmoothMov(Prepos));
                }
            }
        }
        if (Physics.Raycast(transform.position, (transform.forward) * 0.05f, out CurrentParent, 0.05f, RopeEnd) && EndClimb)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                RB.isKinematic = true;
                nextParent = TpRop.transform.position;
                StartCoroutine(FinClimb(3, -1));
                EndClimb = false;
                Trigg = true;
            }

        }
    }

}




