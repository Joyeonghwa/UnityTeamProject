using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseyAttack : MonoBehaviour
{
    CharacterController characterController;
    Animator animator;

    //===== �߻� ======
    public float fireTime = 0;

    //===== MouseL ���� =====
    public GameObject MLBullet;
    public GameObject Muzzle;
    public float limitMLPushTime = 0.7f;//ML ���� ���� �ּ� �Է� �ð�
    public float MLAutoFireRate = 0.3f;//ML ���� ���ӹ߻� �ӵ�
    public float MLPushTime = 0;//ML������ �ִ� �ð�

    //===== F ���� =====
    [Range(0.0f, 3.0f)] public float FAttackTime = 2.0f;//F���� �� �ٽ� IK����Ǵ� �ð�



    IK ik;//ik���� ���



    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        ik = GameObject.Find("Casey").GetComponent<IK>();
    }
    
    void Update()
    {
        ML_Attack();

        if (Input.GetKeyDown(KeyCode.F))//F����
            F_Attack();
    }




    void Fire_Single(GameObject prijectiles)//�ܹ� ����
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
     
    void Fire_Auto(float fireRate, GameObject prijectiles)//���� ����
    {
        fireTime += Time.deltaTime;
        if (fireTime < fireRate) return;
        Fire_Single(prijectiles);
        fireTime = 0;
    }


    void ML_Attack()//ML����
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire_Single(MLBullet);//ML���� �ܹ�
        }

        if (Input.GetMouseButton(0))
        {
            MLPushTime += Time.deltaTime;
            if (limitMLPushTime > MLPushTime) return;
            Fire_Auto(MLAutoFireRate, MLBullet);//ML���� ����
        }

        if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("MLAttack_Casey", false);
            MLPushTime = 0;
        }
    }

    void F_Attack()//F����  -> �̰͵� IK����ϳ�?
    {
        Debug.Log("F");
        ik.CaseyArmIKEnable = false;
        animator.SetTrigger("FAttack_Casey");
        Invoke("SetArmIKEnbleTrue", FAttackTime);
    }

    void SetArmIKEnbleTrue()//IK ����
    {
        ik.CaseyArmIKEnable = true;
    }
}
