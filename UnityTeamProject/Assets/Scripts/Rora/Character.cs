using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// == 상태 ==
[HideInInspector]
public enum PlayerState
{
    Idle = 0,
    Move, //bool로 체크하기
    Jump,
    MainAttack,SubAttack,VSkill,
    QSkill,ESkill,Teleport,FSkill,
    CC,
    Die
}

public class Character : MonoBehaviour
{
    //상태
    // == 상태 ==
    [HideInInspector] public PlayerState state;


    //이동
    CharacterController characterController;
    Vector3 Direction;
    [Range(0.0f, 15.0f)]
    public float MoveSpeed = 7.0f;


    // 회전
    // ==방향 회전 ==
    [Range(0.1f, 2.0f)]
    public float MouseSensitivity = 2.0f;
    float VerticalAngle = 0.0f;
    float HorizontalAngle;
    float RotationSpeed = 360.0f;
    Vector3 currentAngles;

    // == 카메라 회전 ==
    Vector3 c_currentAngles;
    public GameObject camera;
    [Range(0.0f, 90.0f)]
    public float CameraAngle;

    // == 변수
    public bool IsTurnCameraEnable = true;
    public bool IsTurnPlayerEnable = true;



    // 중력
    [Range(0.0f, 15.0f)] public float DefaultGravity = 10.0f;
    float Gravity = 10.0f;
    [Range(-10.0f, 10.0f)] public float VerticalSpeed = 0.0f;
    bool IsGrounded = true;



    //변수
    public Animator animator;
    private bool IsMoving = false;





    private void Start()
    {
        Initialized();
    }


    public void Initialized()
    {
        //animator = this.GetComponent<Animator>();
        characterController = this.GetComponent<CharacterController>();
        state = PlayerState.Idle;


        // == 회전
        VerticalAngle = 0.0f;
        HorizontalAngle = transform.localEulerAngles.y;
        currentAngles = transform.localEulerAngles;
        //== 중력
        Gravity = DefaultGravity;
    }

    private void Update()
    {
        // 물리
        GetGravity();

        //캐릭터 
        MovePlayer();
        Skill();

        if (IsTurnCameraEnable) TurnCamera();
        if (IsTurnPlayerEnable) TurnPlayer();

        if (Input.GetKeyDown(KeyCode.Space)) state = PlayerState.Jump;


        // 점프 관련

        if (IsJumping && IsGrounded)//바닥에 착지
        {
            IsJumping = false;
            animator.SetBool("Jump", IsJumping);
            GroundJumpCount = 0;//점프 카운트 초기화
            AirJumpCount = 0;
            VerticalSpeed = 0;
        }

    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case PlayerState.Idle: { IdleState(); } break;
            case PlayerState.Move:
                {
                    MovePlayer();

                } break;
            case PlayerState.MainAttack: { MainAttack(); } break;
            case PlayerState.SubAttack: { SubAttack(); } break;
            case PlayerState.VSkill: { VSkill(); } break;
            case PlayerState.QSkill: { QSkill(); } break;
            case PlayerState.ESkill: { ESkill(); } break;
            case PlayerState.Teleport: { Teleport(); } break;
            case PlayerState.FSkill: { FSkill(); } break;
            case PlayerState.Jump: { JumpPlayer(); } break; //
            case PlayerState.Die: { Die(); } break;
        }
    }

    public void IdleState()
    {
        Direction = Vector3.zero;
        Direction = new Vector3(Input.GetAxis("Horizontal"), VerticalSpeed, Input.GetAxis("Vertical"));

        if (Direction.sqrMagnitude > 0)
        {
            state = PlayerState.Move;
        }

        else
        {
            state = PlayerState.Idle;
        }
    }



    int SkillNumber = 0;

    void Skill()
    {
        if (Input.GetMouseButtonDown(0)) //주공격
        {
            SkillNumber = 1;
            state = PlayerState.MainAttack;
            animator.SetTrigger("SkillTrigger");
        }
        else if (Input.GetMouseButtonDown(1)) //보조공격
        {
            SkillNumber = 2;
            state = PlayerState.SubAttack;
            animator.SetTrigger("SkillTrigger");
        }
        else if (Input.GetKeyDown(KeyCode.V))  //근접공격
        {
            SkillNumber = 3;
            state = PlayerState.VSkill;
            animator.SetTrigger("SkillTrigger");
        }
        else if (Input.GetKeyDown(KeyCode.Q)) //Q스킬
        {
            SkillNumber = 4;
            state = PlayerState.QSkill;
            animator.SetTrigger("SkillTrigger");
        }
        else if (Input.GetKeyDown(KeyCode.E)) //E스킬
        {
            SkillNumber = 5;
            state = PlayerState.ESkill;
            animator.SetTrigger("SkillTrigger");
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift)) //텔레포트
        {
            SkillNumber = 6;
            state = PlayerState.Teleport;
            animator.SetTrigger("SkillTrigger");
        }
        else if (Input.GetKeyDown(KeyCode.F)) //F스킬(마나 응축탄)
        {
            SkillNumber = 7;
            state = PlayerState.FSkill;
            animator.SetTrigger("SkillTrigger");
        }

        animator.SetInteger("SkillNumber", SkillNumber);

    }




    void MainAttack()
    {
        Debug.Log("레이저 빔 쏘기");
    }

    void SubAttack()
    {
        Debug.Log("산탄총 공격");
    }

    void VSkill()
    {
        Debug.Log("근접공격 타격");
    }

    void QSkill()
    {
        Debug.Log("포탑 생성");
    }

    void ESkill()
    {
        Debug.Log("블랙홀");
    }

    public void Teleport()
    {
        Debug.Log("텔레포트");
        // 캐릭터 앞으로 이동


    }
    public void teleportMove()
    {
        Direction = Vector3.zero;

        //방향 잡아주기
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        // Direction = new Vector3(10.0f, 0,10.0f);
        Direction = new Vector3(horizontal+10.0f, 0, vertical + 10.0f);
        Direction = Direction * MoveSpeed * Time.deltaTime;
        Direction = transform.TransformDirection(Direction);
        characterController.Move(Direction);
    }

    void FSkill()
    {
        Debug.Log("마나 응축탄");
    }

    void Die()
    {
        Debug.Log("죽음");
    }





    void MovePlayer()
    {
        Direction = Vector3.zero;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Direction = new Vector3(horizontal, 0, vertical);
       
        if (Direction.sqrMagnitude > 1.0f) // 빠르게 속도가 증가하는 것을 막아줌.
        {
            Direction.Normalize();
        }

        Direction = Direction * MoveSpeed * Time.deltaTime;
        Direction = transform.TransformDirection(Direction);
        characterController.Move(Direction);

        //== 애니메이션--> 4방향
        if ( Direction.sqrMagnitude > 0.0f)
        {
            IsMoving = true;
            animator.SetFloat("Move_FB", vertical);
            animator.SetFloat("Move_RL", horizontal);
            animator.SetBool("IsMoving", IsMoving);
        }
        else
        {
            IsMoving = false;
            animator.SetBool("IsMoving", IsMoving);
        }
       
      
    }







    void GetGravity()
        {
            Gravity = DefaultGravity;
            VerticalSpeed -= Gravity * Time.deltaTime;

            if (VerticalSpeed < -10.0f)
            {
                VerticalSpeed = -10.0f; //VerticalSpeed 최솟값 -10으로 고정
            }
            
            var verticalMove = new Vector3(0, VerticalSpeed * Time.deltaTime, 0);

            var flag = characterController.Move(verticalMove);

            if ((flag & CollisionFlags.Below) != 0) 
            {
                VerticalSpeed = 0;
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
          
        }




    void TurnCamera()
    {
        var turnCam = -Input.GetAxis("Mouse Y");

        turnCam *= MouseSensitivity;

        VerticalAngle = Mathf.Clamp(turnCam + VerticalAngle, -CameraAngle, CameraAngle);

        c_currentAngles = camera.GetComponent<Transform>().localEulerAngles;
        c_currentAngles.x = VerticalAngle;
        camera.GetComponent<Transform>().localEulerAngles = c_currentAngles;

    }

    void TurnPlayer()
    {
        // 회전
        float PlayerTurn = Input.GetAxis("Mouse X") * MouseSensitivity;
        HorizontalAngle += PlayerTurn;

        if (HorizontalAngle > 360) HorizontalAngle -= RotationSpeed;
        if (HorizontalAngle < 0) HorizontalAngle += RotationSpeed;

        currentAngles.y = HorizontalAngle;
        transform.localEulerAngles = currentAngles;
    }













    


    // ==== 미 완 성
    
    [Range(5.0f, 10.0f)]
    public float GroundJumpPower = 7.0f; //점프 파워
    public float AirJumpPower = 7.0f;      //점프 파워
    int MaxGroundJumpCount = 1;
    int MaxAirJumpCount = 1;

    float LimitSpaceTime = 0.25f;
    float AirJumpCoolTime;
    int GroundJumpCount = 0;
    int AirJumpCount = 0;
    float SpaceTime = 0;

   public float KeepAirGravity = 0.1f;
   public float GroundJumpGravity = 7f;
    bool StartJump = false;
    bool IsJumping = false;



    void StartJumpReset()
    {
        if (StartJump)
        {
            StartJump = false;
            animator.SetBool("StartJump", StartJump);
        }
    }
    void KeepAir()//공중유지
    {
        if (Input.GetKey(KeyCode.Space) && VerticalSpeed < 1)//스페이스 누르고 있고 하강중
        {
            SpaceTime += Time.deltaTime;
            if (SpaceTime > LimitSpaceTime)//0.25초 동안 스페이스 입력 확인
            {
                Gravity = KeepAirGravity;//공중유지 중력
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SpaceTime = 0;
            Gravity = DefaultGravity;//원래 중력으로
        }
    }
    //  ===  미완성




    //
    int JumpState = 0;
    bool IsDoubleJumpEnable = false;
    public float JumpSpeed = 5.0f; 
    public float DoubleJumpSpeed = 7.0f;
    //[Range(-10.0f, 10.0f)]
   // public float VerticalSpeed = 0.0f;

    void JumpPlayer()
    {
        switch (JumpState)
        {
            case 0: { Gravity = DefaultGravity; } break; // 디폴트
            case 1: { Gravity = DefaultGravity; } break; // 1초동안 떨어지도록 코드 수정 
            case 2: { Gravity = DefaultGravity * (0.9f); } break; //이단 점프시 더 천천히 떨어지도록 + 3초 동안 떨어지도록 코드 수정 
            case 3: { Gravity = DefaultGravity * (0.9f); } break; //이단 점프시 더 천천히 떨어지도록 + 3초 동안 떨어지도록 코드 수정
        }


        if (Direction.sqrMagnitude > 0.0f) { MovePlayer(); }


            if (Input.GetButtonDown("Jump"))
        {
            if (IsGrounded && !IsDoubleJumpEnable) JumpState = 1;
            else if (!IsGrounded && IsDoubleJumpEnable) JumpState = 2;

            switch (JumpState)
            {
                case 0:
                    {
                        IsJumping = false; //점프 애니메이션 변수
                    }
                    break;
                case 1:
                    {
                        VerticalSpeed = JumpSpeed;
                        IsDoubleJumpEnable = true;
                        IsJumping = true; //점프 애니메이션 변수
                    }
                    break;
                case 2:
                    {
                        VerticalSpeed = DoubleJumpSpeed;
                        IsDoubleJumpEnable = false;
                        JumpState = 3;
                        IsJumping = true; //점프 애니메이션 변수
                    }
                    break;
                case 3:
                    {
                        IsJumping = true;//점프 애니메이션 변수
                        if (IsGrounded == true) JumpState = 0;
                    }
                    break;
            }

        }

        //== 애니메이션
        animator.SetBool("IsJumping", IsJumping);
    }
}


  

 

   

    




