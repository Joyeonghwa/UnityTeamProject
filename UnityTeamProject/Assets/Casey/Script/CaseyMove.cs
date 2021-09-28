using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseyMove : MonoBehaviour
{
    //== �̵� ==
    [Range(0.0f, 15.0f)]
    public float runSpeed = 6.0f;           //�̵� �ӵ�

    //== ȸ�� ==
    public float rotSpeed = 200.0f;         //ȸ�� �ӵ�
    [Range(0.1f, 2.0f)]
    public float MouseSensitivity = 1.0f;   //���콺 ����

    float rotX = 0;  //X�� ȸ��
    float rotY = 0;  //Y�� ȸ��

    //== �߷� ==
    public float defaultGravity = -10.0f;   //�⺻ �߷�
    public float keepAirGravity = -0.1f;    //������������ �߷�
    public float CaseyGravity;              //���̽� �߷� ����
    bool isGrounded = true;                 //������ üũ
    //bool usingGravity = false;            //�߷� ��������� üũ

    //== ���� ==
    public int MaxGroundJumpCount = 1;  //������ Ƚ��
    public int MaxAirJumpCount = 4;     //�������� Ƚ��
    public int GroundJumpCount = 0;     //������ ī��Ʈ
    public int AirJumpCount = 0;        //���� ���� ī��Ʈ

    public float GroundJumpPower = 7.0f; //���� �Ŀ�
    public float AirJumpPower = 7.0f;      //���� �Ŀ�
    public float yVelocity = 0;             //���� �ӷ�
    bool isJumping = false;
    bool startJump = false;

    //== �߷����� ==
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
        Grounded();             //������ üũ
        Move();                 // Ű���� �Է¿� ���� �̵�
        CameraRotation();       // ���콺�� ���Ʒ�(Y) �����ӿ� ���� ī�޶� X �� ȸ�� 
        CharacterRotation();    // ���콺 �¿�(X) �����ӿ� ���� ĳ���� Y �� ȸ�� 
        Jump();
        KeepAir();
    }

    void Grounded()//������ üũ
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

    void Gravity()//�߷� ����
    {
        Debug.Log("UsingGravity");
        yVelocity += CaseyGravity * Time.deltaTime;
    }


    void KeepAir()//��������
    {
        if(Input.GetKey(KeyCode.Space)&&yVelocity<1)//�����̽� ������ �ְ� �ϰ���
        {
            SpaceTime += Time.deltaTime;
            if(SpaceTime>limitSpaceTime)//0.25�� ���� �����̽� �Է� Ȯ��
            {
                CaseyGravity = keepAirGravity;//�������� �߷�
                Debug.Log("KeepAir");
            }
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            SpaceTime = 0;
            CaseyGravity = defaultGravity;//���� �߷�����
            Debug.Log("StdAir");
        }
    }
    
    void Jump()//����
    {
        //space �Է½� ����
        if (Input.GetKeyDown(KeyCode.Space))//�����̽�Ű ������
        {

            if(MaxAirJumpCount > AirJumpCount)//���� ����
            {
                startJump = true;
                animator.SetBool("startJump", startJump);
                isJumping = true;
                animator.SetBool("Jump", isJumping);

                if (isGrounded)              //�� ����
                {
                    yVelocity = GroundJumpPower;  //�� ���� �Ŀ�
                    Debug.Log("�� ����");
                    ++GroundJumpCount;
                }
                else                        //���� ����
                {
                    yVelocity = AirJumpPower; //�������� �Ŀ�
                    Debug.Log("���� ����");
                    ++AirJumpCount;

                }
                direction.y = yVelocity * Time.deltaTime;
                Invoke("startJumpReset", 0.5f);//0.5���Ŀ� ����
                characterController.Move(direction * Time.deltaTime);
            }
        }
        else 
        {
            if (isJumping && isGrounded)//�ٴڿ� ����
            {
                isJumping = false;
                animator.SetBool("Jump", isJumping);
                GroundJumpCount = 0;//���� ī��Ʈ �ʱ�ȭ
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
        //Ű���� �Է� ��
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
        direction = transform.TransformDirection(direction);    //�������� ���� ��ȯ
        direction.Normalize();//����ȭ
        animator.SetFloat("Speed_Y", direction.y);
        direction.y = yVelocity;
        characterController.Move(direction * runSpeed * Time.deltaTime);

        //�������� Ȯ��
        if ((characterController.collisionFlags & CollisionFlags.Below) != 0)
            print("Touching ground!");
        else
            print("~Touching ground!");
    }

    void CameraRotation()
    {
        float limit_rotX_min = -60.0f;  //X�� ȸ�� ���� �ּڰ�
        float limit_rotX_max = 80.0f;   //X�� ȸ�� ���� �ִ�

        // ���콺 Y �̵� ��
        float mouseY = MouseSensitivity * Input.GetAxis("Mouse Y");

        rotX += mouseY * rotSpeed * Time.deltaTime;
        
        rotX = Mathf.Clamp(rotX, limit_rotX_min, limit_rotX_max);
        Camera.main.transform.localEulerAngles = new Vector3(-rotX, 0, 0);
    }

    void CharacterRotation()
    {
        // ���콺 X �̵� ��
        float mouseX = MouseSensitivity * Input.GetAxis("Mouse X");

        rotY += mouseX * rotSpeed * Time.deltaTime;
        transform.localEulerAngles = new Vector3(0, rotY, 0);
    }
}
