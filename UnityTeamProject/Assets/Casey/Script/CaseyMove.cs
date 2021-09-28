using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseyMove : MonoBehaviour
{
    //== 이동 ==
    [Range(0.0f, 15.0f)]
    public float runSpeed = 6.0f;           //이동 속도

    //== 회전 ==
    public float rotSpeed = 200.0f;         //회전 속도
    [Range(0.1f, 2.0f)]
    public float MouseSensitivity = 1.0f;   //마우스 감도

    float rotX = 0;  //X축 회전
    float rotY = 0;  //Y축 회전

    //== 중력 ==
    public float defaultGravity = -10.0f;   //기본 중력
    public float keepAirGravity = -0.1f;    //공중유지상태 중력
    public float CaseyGravity;              //케이시 중력 변수
    bool isGrounded = true;                 //땅인지 체크
    //bool usingGravity = false;            //중력 사용중인지 체크

    //== 점프 ==
    public int MaxGroundJumpCount = 1;  //땅점프 횟수
    public int MaxAirJumpCount = 4;     //공중점프 횟수
    public int GroundJumpCount = 0;     //땅점프 카운트
    public int AirJumpCount = 0;        //공중 점프 카운트

    public float GroundJumpPower = 7.0f; //점프 파워
    public float AirJumpPower = 7.0f;      //점프 파워
    public float yVelocity = 0;             //수직 속력
    bool isJumping = false;
    bool startJump = false;

    //== 중력유지 ==
    public float limitSpaceTime = 0.25f;
    public float SpaceTime = 0;

    Vector3 direction;
    CharacterController characterController;
    Animator animator;

    //public AudioClip itemClip;
    //private AudioSource audioSource;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        CaseyGravity = defaultGravity;
        //audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Grounded();             //땅인지 체크
        Move();                 // 키보드 입력에 따라 이동
        CameraRotation();       // 마우스를 위아래(Y) 움직임에 따라 카메라 X 축 회전 
        CharacterRotation();    // 마우스 좌우(X) 움직임에 따라 캐릭터 Y 축 회전 
        Jump();
        KeepAir();
    }

    void Grounded()//땅인지 체크
    {
        if ((characterController.collisionFlags & CollisionFlags.Below) != 0)
        {
            isGrounded = true;
            animator.SetBool("isGrounded", isGrounded);
        }
        else
        {
            isGrounded = false;
            animator.SetBool("isGrounded", isGrounded);
            Gravity();
        }
    }

    void Gravity()//중력 적용
    {
        Debug.Log("UsingGravity");
        yVelocity += CaseyGravity * Time.deltaTime;
    }


    void KeepAir()//공중유지
    {
        if(Input.GetKey(KeyCode.Space)&&yVelocity<1)//스페이스 누르고 있고 하강중
        {
            SpaceTime += Time.deltaTime;
            if(SpaceTime>limitSpaceTime)//0.25초 동안 스페이스 입력 확인
            {
                CaseyGravity = keepAirGravity;//공중유지 중력
                Debug.Log("KeepAir");
            }
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            SpaceTime = 0;
            CaseyGravity = defaultGravity;//원래 중력으로
            Debug.Log("StdAir");
        }
    }
    
    void Jump()//점프
    {
        //space 입력시 점프
        if (Input.GetKeyDown(KeyCode.Space))//스페이스키 누르면
        {

            if(MaxAirJumpCount > AirJumpCount)//점프 가능
            {
                startJump = true;
                animator.SetBool("startJump", startJump);
                isJumping = true;
                animator.SetBool("Jump", isJumping);

                if (isGrounded)              //땅 점프
                {
                    yVelocity = GroundJumpPower;  //땅 점프 파워
                    Debug.Log("땅 점프");
                    ++GroundJumpCount;
                }
                else                        //공중 점프
                {
                    yVelocity = AirJumpPower; //공중점프 파워
                    Debug.Log("공중 점프");
                    ++AirJumpCount;

                }
                direction.y = yVelocity * Time.deltaTime;
                Invoke("startJumpReset", 0.5f);//0.5초후에 실행
                characterController.Move(direction * Time.deltaTime);
            }
        }
        else 
        {
            if (isJumping && isGrounded)//바닥에 착지
            {
                isJumping = false;
                animator.SetBool("Jump", isJumping);
                GroundJumpCount = 0;//점프 카운트 초기화
                AirJumpCount = 0;
                yVelocity = 0;
            }
        }
    }

    void startJumpReset()
    {
        if (startJump)
        {
            startJump = false;
            animator.SetBool("startJump", startJump);
        }
    }

    void Move()
    {
        //키보드 입력 값
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if(isGrounded)
        {
            animator.SetFloat("Speed_F", v);
            animator.SetFloat("Speed_B", v);
            animator.SetFloat("Speed_L", h);
            animator.SetFloat("Speed_R", h);
        }  
        else
        {
            
        }
        direction = new Vector3(h, yVelocity, v);
        direction = transform.TransformDirection(direction);    //기준으로 방향 변환
        direction.Normalize();//정규화
        animator.SetFloat("Speed_Y", direction.y);
        direction.y = yVelocity;
        characterController.Move(direction * runSpeed * Time.deltaTime);

        //착지상태 확인
        if ((characterController.collisionFlags & CollisionFlags.Below) != 0)
            print("Touching ground!");
        else
            print("~Touching ground!");
    }

    void CameraRotation()
    {
        float limit_rotX_min = -60.0f;  //X축 회전 각도 최솟값
        float limit_rotX_max = 80.0f;   //X축 회전 각도 최댓값

        // 마우스 Y 이동 값
        float mouseY = MouseSensitivity * Input.GetAxis("Mouse Y");

        rotX += mouseY * rotSpeed * Time.deltaTime;
        
        rotX = Mathf.Clamp(rotX, limit_rotX_min, limit_rotX_max);
        Camera.main.transform.localEulerAngles = new Vector3(-rotX, 0, 0);
    }

    void CharacterRotation()
    {
        // 마우스 X 이동 값
        float mouseX = MouseSensitivity * Input.GetAxis("Mouse X");

        rotY += mouseX * rotSpeed * Time.deltaTime;
        transform.localEulerAngles = new Vector3(0, rotY, 0);
    }
}
