using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoginUI : BaseUI
{
    public TMP_InputField txtID;
    public TMP_InputField txtPassword;
    public TextMeshProUGUI txtAlert;

    string id;
    string password;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnClickLogIn()
    {
        txtAlert.text = null;
        if (txtAlert.text == null && string.IsNullOrEmpty(id))        txtAlert.text = "! 아이디 입력란이 비었습니다.";
        if (txtAlert.text == null && string.IsNullOrEmpty(password))  txtAlert.text = "! 비밀번호 입력란이 비었습니다.";
        txtAlert.color = Color.red;

        if (txtAlert.text == null)
            StartCoroutine(UserDataManager.Inst.Login(id, password, txtAlert));
    }

    private void OnClickSignUp()
    {
        UI_Manager.Inst.ChangeUI(UI_Type.SIGN_UP);
    }

    private void OnClickExit()
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }

    public void OnValueChangedID(TMP_InputField txtID)
    {
        id = txtID.text;
    }

    public void OnValueChangedPassword(TMP_InputField txtPassword)
    {
        password = txtPassword.text;
    }

    public override void Activate()
    {
        Debug.Log("Login Activated");
        base.Activate();

        txtID.text = "";
        txtPassword.text = "";
        txtAlert.text = "";
    }
}
