using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour
{
    [Range(0, 1)] public float posWeight = 1;
    //[Range(0, 1)] public float rotWeight = 1;
    //[Range(0, 359)] public float xRot = 0.0f;
    //[Range(0, 359)] public float yRot = 0.0f;
    //[Range(0, 359)] public float zRot = 0.0f;
    public bool CaseyArmIKEnable = true; //팔 IK
    public bool CaseyHeadIKEnable = true; //얼굴 IK

    public Transform target;// 바라볼 타겟
    protected Animator animator; // 애니메이션 
    private int selecteWeight = 1;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if(CaseyArmIKEnable)
            SetPositionWeightArm();

        if(CaseyHeadIKEnable)
            SetPositionWeightHead();
    }

    private void SetPositionWeightArm()//position weight만큼 팔 이동
    {
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, posWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0.0f);

        animator.SetIKPosition(AvatarIKGoal.RightHand, target.position);
        Quaternion handRotation = Quaternion.LookRotation(target.position - transform.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);
    }

        private void SetPositionWeightHead()//position weight만큼 고개 이동
    {
        animator.SetLookAtWeight(1);
        animator.SetLookAtPosition(target.position);
    }
}
