using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour
{
    [Range(0, 1)] public float posWeight = 1;
    [Range(0, 1)] public float rotWeight = 1;
    [Range(0, 359)] public float xRot = 0.0f;
    [Range(0, 359)] public float yRot = 0.0f;
    [Range(0, 359)] public float zRot = 0.0f;

    public Transform target;// �ٶ� Ÿ��
    private Animator animator; // �ִϸ��̼� 
    private int selecteWeight = 1;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        SetPositionWeight();
    }

    private void SetPositionWeight()//position weight��ŭ �̵�
    {
        //��
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, posWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0.0f);

        animator.SetIKPosition(AvatarIKGoal.RightHand, target.position);
        Quaternion handRotation = Quaternion.LookRotation(target.position - transform.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);

        //��
        animator.SetLookAtWeight(1);
        animator.SetLookAtPosition(target.position);
    }
}
