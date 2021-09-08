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

    // ���� ���������Ʈ userDB�� �α��� & ȸ�����Կ� ��ũ��Ʈ �ּ�.
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
        // ȸ�������� �Ѵ�.(=userData�� userDB�� �����Ѵ�.)
        // ȸ�������� �ϴ� ���� �ٸ� UI�� Ŭ������ ���ϰ� �������� Ȱ��ȭ�Ѵ�.
        StartUI_Manager.Inst.SetCover(true);

        // ��û�� ������ ȸ���������� ������ �����͸� form�� �ۼ��ϰ� �̸� ���� Post�Ѵ�.
        WWWForm form = new WWWForm();
        form.AddField("order", "Register");
        form.AddField("id", id);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.isDone) print("Post Completed");
            else            print("���� ������ �����ϴ�.");

            // ����� �޾� ���� ���ο� ���� �޸� ó���Ѵ�.
            string strResult = www.downloadHandler.text;
            print(strResult);
            WebRequestResult result = JsonUtility.FromJson<WebRequestResult>(strResult);
            if (result.bSuccess)
            {
                // ������ �����ϰ� Ÿ��Ʋ ȭ������ �Ѿ��.
                NetworkManager.Inst.Connect();
            }
            else
            {
                // ��� �޽����� ������Ʈ�Ѵ�.
                txtAlert.text = result.msg;
                StartUI_Manager.Inst.SetCover(false);
            }
        }
    }

    public IEnumerator Login(string id, string password, TextMeshProUGUI txtAlert)
    {
        // �α����� �õ��Ѵ�.
        // �α����ϴ� ���� �ٸ� UI�� Ŭ������ ���ϰ� �������� Ȱ��ȭ�Ѵ�.
        StartUI_Manager.Inst.SetCover(true);

        // ��û�� ������ �α��ο� �ʿ��� �����͸� form�� �ۼ��ϰ� �̸� ���� Post�Ѵ�.
        WWWForm form = new WWWForm();
        form.AddField("order", "Login");
        form.AddField("id", id);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.isDone) print("Post Completed");
            else            print("���� ������ �����ϴ�.");

            // ����� �޾� ���� ���ο� ���� �޸� ó���Ѵ�.
            string strResult = www.downloadHandler.text;
            print(strResult);
            WebRequestResult result = JsonUtility.FromJson<WebRequestResult>(strResult);
            if (result.bSuccess)
            {
                // ������ �����ϰ� Ÿ��Ʋ ȭ������ �Ѿ��.
                NetworkManager.Inst.Connect();
            }
            else
            {
                // ��� �޽����� ������Ʈ�Ѵ�.
                txtAlert.text = result.msg;
                StartUI_Manager.Inst.SetCover(false);
            }
        }
    }
}
