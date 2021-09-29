using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �÷��̾�� ĳ���͵��� ���� Ŭ���� �� �������̽�
public abstract class Playable : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        InputProc();
    }

    // �÷��̾��� �Է��� ������ ó���ϴ� �Լ�.
    private void InputProc()
    {
        // �ƹ�Ű�� �ȴ�������
        if(!Input.anyKey)
        {
            Idle();
        }
        else
        {
            // �̵�Ű�� ���� ���
            if(
                KeyManager.Inst.GetAxisRawHorizontal() != 0 ||
                KeyManager.Inst.GetAxisRawVertical() != 0)
            {
                Move();
            }

            // ����Ű�� ���� ���
            if(Input.GetKeyDown(KeyManager.Inst.Jump))
            {
                Jump();
            }

            // �Ϲ� ����Ű�� ���� ���
            if(Input.GetKey(KeyManager.Inst.Attack))
            {
                Attack();
            }

            // Ư�� ����Ű�� ���� ���
            if(Input.GetKey(KeyManager.Inst.SpecialAttack))
            {
                SpecialAttack();
            }

            // ���� ����Ű�� ���� ���
            if (Input.GetKey(KeyManager.Inst.MeleeAttack))
            {
                MeleeAttack();
            }
            
            // ��ų(E ��ų)�� �� ���
            if(Input.GetKey(KeyManager.Inst.Skill))
            {
                Skill();
            }

            // �ñر�(Q ��ų)�� �� ���
            if (Input.GetKey(KeyManager.Inst.UltimateSkill))
            {
                UltimateSkill();
            }
        }
    }

    // ���� �÷��̾���� �����ؾ��� ���
    public abstract void Idle();
    public abstract void Move();
    public abstract void Jump();
    public abstract void Attack();
    public abstract void SpecialAttack();
    public abstract void MeleeAttack();
    public abstract void Skill();
    public abstract void UltimateSkill();


    // �� ĳ���Ϳ��� ���������� ���̴� �Լ���
    public void IncreaseHP(int value)
    {
        throw new NotImplementedException();
    }

    public void TakeDamage(int value)
    {
        throw new NotImplementedException();
    }
}
