using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[System.Serializable]
public class WebRequestResult
{
    public string result, msg;
    public bool bSuccess;
}

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager userDataManager;

    public static UserDataManager Inst
    {
        get
        {
            return userDataManager;
        }
    }

    // 구글 스프레드시트 userDB의 로그인 & 회원가입용 스크립트 주소.
    string URL = "https://script.google.com/macros/s/AKfycbw5GktpPTSlVdKIlJ5T_-rRW3GSjw45JZtUjeJiMWIeCovNijIqqiQGx-crOWXzFTP5/exec";

    public UserData userData;

    // Start is called before the first frame update
    void Awake()
    {
        InitManager();
    }

    private void InitManager()
    {
        userDataManager = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator Register(string id, string password, TextMeshProUGUI txtAlert)
    {
        // 회원가입을 한다.(=userData를 userDB에 저장한다.)
        // 회원가입을 하는 동안 다른 UI를 클릭하지 못하게 가림막을 활성화한다.
        StartUI_Manager.Inst.SetCover(true);

        // 요청의 종류와 회원가입으로 저장할 데이터를 form에 작성하고 이를 웹에 Post한다.
        WWWForm form = new WWWForm();
        form.AddField("order", "Register");
        form.AddField("id", id);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.isDone) print("Post Completed");
            else            print("웹의 응답이 없습니다.");

            // 결과를 받아 성공 여부에 따라 달리 처리한다.
            string strResult = www.downloadHandler.text;
            print(strResult);
            WebRequestResult result = JsonUtility.FromJson<WebRequestResult>(strResult);
            if (result.bSuccess)
            {
                // 서버에 연결하고 타이틀 화면으로 넘어간다.
                NetworkManager.Inst.Connect();
            }
            else
            {
                // 경고 메시지를 업데이트한다.
                txtAlert.text = result.msg;
                StartUI_Manager.Inst.SetCover(false);
            }
        }
    }

    public IEnumerator Login(string id, string password, TextMeshProUGUI txtAlert)
    {
        // 로그인을 시도한다.
        // 로그인하는 동안 다른 UI를 클릭하지 못하게 가림막을 활성화한다.
        StartUI_Manager.Inst.SetCover(true);

        // 요청의 종류와 로그인에 필요한 데이터를 form에 작성하고 이를 웹에 Post한다.
        WWWForm form = new WWWForm();
        form.AddField("order", "Login");
        form.AddField("id", id);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.isDone) print("Post Completed");
            else            print("웹의 응답이 없습니다.");

            // 결과를 받아 성공 여부에 따라 달리 처리한다.
            string strResult = www.downloadHandler.text;
            print(strResult);
            WebRequestResult result = JsonUtility.FromJson<WebRequestResult>(strResult);
            if (result.bSuccess)
            {
                // 서버에 연결하고 타이틀 화면으로 넘어간다.
                NetworkManager.Inst.Connect();
            }
            else
            {
                // 경고 메시지를 업데이트한다.
                txtAlert.text = result.msg;
                StartUI_Manager.Inst.SetCover(false);
            }
        }
    }
}
