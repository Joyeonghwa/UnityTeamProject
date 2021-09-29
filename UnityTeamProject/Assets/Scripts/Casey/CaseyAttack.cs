using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseyAttack : MonoBehaviour
{
    CharacterController characterController;
    Animator animator;

    //===== 발사 ======
    public float fireTime = 0;

    //===== MouseL 공격 =====
    public GameObject MLBullet;
    public GameObject Muzzle;
    public float limitMLPushTime = 0.7f;//ML 연속 공격 최소 입력 시간
    public float MLAutoFireRate = 0.3f;//ML 연속 연속발사 속도
    public float MLPushTime = 0;//ML누르고 있는 시간

    //===== F 공격 =====
    [Range(0.0f, 3.0f)] public float FAttackTime = 2.0f;//F공격 후 다시 IK실행되는 시간



    IK ik;//ik변수 사용



    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        ik = GameObject.Find("Casey").GetComponent<IK>();
    }
    
    void Update()
    {
        ML_Attack();

        if (Input.GetKeyDown(KeyCode.F))//F공격
            F_Attack();
    }




    void Fire_Single(GameObject prijectiles)//단발 공격
    {
        Debug.Log("MouseL");
        animator.SetBool("MLAttack_Casey", true);
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo = new RaycastHit();
        GameObject bullet = Instantiate(prijectiles);
        bullet.transform.position = Muzzle.transform.position;
        bullet.transform.rotation = transform.rotation;
        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.Log(hitInfo.ToString());
        }
    }
     
    void Fire_Auto(float fireRate, GameObject prijectiles)//연발 공격
    {
        fireTime += Time.deltaTime;
        if (fireTime < fireRate) return;
        Fire_Single(prijectiles);
        fireTime = 0;
    }


    void ML_Attack()//ML공격
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire_Single(MLBullet);//ML공격 단발
        }

        if (Input.GetMouseButton(0))
        {
            MLPushTime += Time.deltaTime;
            if (limitMLPushTime > MLPushTime) return;
            Fire_Auto(MLAutoFireRate, MLBullet);//ML공격 연발
        }

        if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("MLAttack_Casey", false);
            MLPushTime = 0;
        }
    }

    void F_Attack()//F공격  -> 이것도 IK써야하나?
    {
        Debug.Log("F");
        ik.CaseyArmIKEnable = false;
        animator.SetTrigger("FAttack_Casey");
        Invoke("SetArmIKEnbleTrue", FAttackTime);
    }

    void SetArmIKEnbleTrue()//IK 설정
    {
        ik.CaseyArmIKEnable = true;
    }
}
