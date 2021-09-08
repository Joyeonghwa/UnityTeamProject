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
        // 회원 가입을 한다.
        txtAlert.text = null;
        if (txtAlert.text == null && id == null)                 txtAlert.text = "! 아이디 입력란이 비었습니다.";
        if (txtAlert.text == null && password == null)           txtAlert.text = "! 비밀번호 입력란이 비었습니다.";
        if (txtAlert.text == null && passwordCheck == null)      txtAlert.text = "! 비밀번호 재확인을 해주세요.";
        if (txtAlert.text == null && password != passwordCheck)  txtAlert.text = "! 비밀번호와 재확인 번호가 서로 다릅니다.";
        txtAlert.color = Color.red;
        
        if(txtAlert.text == null)
            StartCoroutine(UserDataManager.Inst.Register(id, password, txtAlert));
    }

    private void OnClickBackward()
    {
        // 뒤로가기
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
