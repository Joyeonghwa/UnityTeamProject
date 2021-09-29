using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    // �̱��� ����
    public static KeyManager keyManager;
    public static KeyManager Inst { get { return keyManager; } }

    void Start()
    {
        keyManager = this;
        DontDestroyOnLoad(this);
    }

    // ==Ű ��============================
    // �̵�
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode moveUp = KeyCode.W;
    public KeyCode moveDown = KeyCode.S;

    // ����
    public KeyCode Jump = KeyCode.Space;

    // ����
    public KeyCode Attack = KeyCode.Mouse0;
    public KeyCode SpecialAttack = KeyCode.Mouse1;
    public KeyCode MeleeAttack = KeyCode.V;
    public KeyCode Skill = KeyCode.E;
    public KeyCode UltimateSkill = KeyCode.Q;

    // ==���� �Լ�======================
    public float GetAxisRawHorizontal()
    {
        // Input.GetAxisRaw("Horizontal") �Լ��� ��ü�ϴ� �Լ�
        if(Input.GetKey(moveLeft))          return -1f;
        else if(Input.GetKey(moveRight))    return 1f;
        else                                return 0;
    }

    public float GetAxisRawVertical()
    {
        if (Input.GetKey(moveDown))         return -1f;
        else if (Input.GetKey(moveUp))      return 1f;
        else                                return 0;
    }
}
