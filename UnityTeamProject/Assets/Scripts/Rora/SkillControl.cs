using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SkillControl : MonoBehaviour
{
    // == 무기 관련 

    public bool IsControlWeaponEnable = true;

    [Range(0, 1)] public float posWeight = 1;
    [Range(0, 1)] public float rotWeight = 1;
    [Range(0, 359)] public float xRot = 0.0f;
    [Range(0, 359)] public float yRot = 0.0f;
    [Range(0, 359)] public float zRot = 0.0f;


    private Animator animator;
    public Transform cameraObjTransform;
    public GameObject wand;

    public bool IsFixedWand = true;

    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if(IsControlWeaponEnable) ControlWand();
    }
   

    void ControlWand()
    {
        //SetEachWeight
        if (!IsFixedWand)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, posWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rotWeight);

            animator.SetIKPosition(AvatarIKGoal.RightHand, cameraObjTransform.position);

            Quaternion handRotation = Quaternion.LookRotation(cameraObjTransform.position - transform.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);
        }

        //LookAtObj
        {
           animator.SetLookAtWeight(1.0f);
           animator.SetLookAtPosition(cameraObjTransform.position);
        }

        //지팡이
        //wand.GetComponent<Transform>().LookAt(cameraObjTransform.position - new Vector3(0.0f, 90.0f, 0.0f));
    }


    public void Death()
    {
        Debug.Log("DEATH");
    }
    
}




// 미완성
abstract class Skill : MonoBehaviour
{
    public float damage;
    public float Damage { get { return damage; } }

    public Skill(int damage_)
    {
        damage = damage_;
    }
}

class MainAttack : Skill //주공격(레이저빔쏘기)
{
    public MainAttack(int damage_) : base(damage_){ }
}

class SubAttack : Skill //보조공격(산탄총 공격)
{
    public SubAttack(int damage_) : base(damage_) { }
}

class VSkill : Skill // 근접공격(타격)
{
    public VSkill(int damage_) : base(damage_) { }
}

class QSkill : Skill // 포탑생성
{
    public QSkill(int damage_) : base(damage_) { }
}

class ESkill : Skill // 블랙홀
{
    public ESkill(int damage_) : base(damage_) { }
}

class Teleportl : Skill //텔레포트
{
    public Teleportl(int damage_) : base(damage_) { }
}