using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SignUpUI : BaseUI
{
    public TMP_InputField txtID;
    public TMP_InputField txtPassword;
    public TMP_InputField txtPasswordCheck;
    public TextMeshProUGUI txtAlert;

    string id;
    string password;
    string passwordCheck;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnClickComplete()
    {
        // ȸ�� ������ �Ѵ�.
        txtAlert.text = null;
        if (txtAlert.text == null && id == null)                 txtAlert.text = "! ���̵� �Է¶��� ������ϴ�.";
        if (txtAlert.text == null && password == null)           txtAlert.text = "! ��й�ȣ �Է¶��� ������ϴ�.";
        if (txtAlert.text == null && passwordCheck == null)      txtAlert.text = "! ��й�ȣ ��Ȯ���� ���ּ���.";
        if (txtAlert.text == null && password != passwordCheck)  txtAlert.text = "! ��й�ȣ�� ��Ȯ�� ��ȣ�� ���� �ٸ��ϴ�.";
        txtAlert.color = Color.red;
        
        if(txtAlert.text == null)
            StartCoroutine(UserDataManager.Inst.Register(id, password, txtAlert));
    }

    private void OnClickBackward()
    {
        // �ڷΰ���
        UI_Manager.Inst.ChangeUI(UI_Type.LOGIN);
    }

    public void OnValueChangedID(TMP_InputField txtID)
    {
        id = txtID.text;
    }

    public void OnValueChangedPassword(TMP_InputField txtPassword)
    {
        password = txtPassword.text;
    }

    public void OnValueChangedPasswordCheck(TMP_InputField txtPasswordCheck)
    {
        passwordCheck = txtPasswordCheck.text;
    }

    public override void Activate()
    {
        base.Activate();

        txtID.text = "";
        txtPassword.text = "";
        txtPasswordCheck.text = "";
        txtAlert.text = "";
    }
}
