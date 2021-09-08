using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UI_Type
{
    LOGIN,
    SIGN_UP,
    TITLE,
    LOBBY,
    ROOM
}

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager uiManager;

    public static UI_Manager Inst
    {
        get
        {
            return uiManager;
        }
    }

    private BaseUI curUI;

    public LoginUI logInUI;
    public SignUpUI signUpUI;
    public TitleUI titleUI;
    public GameObject lobbyUI;
    public GameObject roomUI;

    public GameObject cover;

    // Start is called before the first frame update
    void Start()
    {
        InitManager();
    }

    private void InitManager()
    {
        uiManager = this;
        curUI = logInUI;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeUI(UI_Type type)
    {
        curUI.Deactivate();

        switch(type)
        {
            case UI_Type.LOGIN:
                curUI = logInUI;
                break;
            case UI_Type.SIGN_UP:
                curUI = signUpUI;
                break;
            case UI_Type.TITLE:
                curUI = titleUI;
                break;
            case UI_Type.LOBBY:
                break;
            case UI_Type.ROOM:
                break;
        }

        curUI.Activate();
    }

    public void SetCover(bool bCoverOn)
    {
        // UI를 클릭하지 못하게하는 커버를 활성화 or 비활성화 한다.
        cover.SetActive(bCoverOn);
    }
}
