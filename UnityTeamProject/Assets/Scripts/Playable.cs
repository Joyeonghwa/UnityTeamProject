using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어블 캐릭터들의 상위 클래스 및 인터페이스
public abstract class Playable : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        InputProc();
    }

    // 플레이어의 입력을 감지해 처리하는 함수.
    private void InputProc()
    {
        // 아무키도 안눌렀을때
        if(!Input.anyKey)
        {
            Idle();
        }
        else
        {
            // 이동키를 누른 경우
            if(
                KeyManager.Inst.GetAxisRawHorizontal() != 0 ||
                KeyManager.Inst.GetAxisRawVertical() != 0)
            {
                Move();
            }

            // 점프키를 누른 경우
            if(Input.GetKeyDown(KeyManager.Inst.Jump))
            {
                Jump();
            }

            // 일반 공격키를 누른 경우
            if(Input.GetKey(KeyManager.Inst.Attack))
            {
                Attack();
            }

            // 특수 공격키를 누른 경우
            if(Input.GetKey(KeyManager.Inst.SpecialAttack))
            {
                SpecialAttack();
            }

            // 근접 공격키를 누른 경우
            if (Input.GetKey(KeyManager.Inst.MeleeAttack))
            {
                MeleeAttack();
            }
            
            // 스킬(E 스킬)을 쓴 경우
            if(Input.GetKey(KeyManager.Inst.Skill))
            {
                Skill();
            }

            // 궁극기(Q 스킬)를 쓴 경우
            if (Input.GetKey(KeyManager.Inst.UltimateSkill))
            {
                UltimateSkill();
            }
        }
    }

    // 하위 플레이어블에서 구현해야할 목록
    public abstract void Idle();
    public abstract void Move();
    public abstract void Jump();
    public abstract void Attack();
    public abstract void SpecialAttack();
    public abstract void MeleeAttack();
    public abstract void Skill();
    public abstract void UltimateSkill();


    // 두 캐릭터에서 공통적으로 쓰이는 함수들
    public void IncreaseHP(int value)
    {
        throw new NotImplementedException();
    }

    public void TakeDamage(int value)
    {
        throw new NotImplementedException();
    }
}
