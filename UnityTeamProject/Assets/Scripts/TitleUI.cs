using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class TitleUI : BaseUI
{
    public TextMeshProUGUI txtWelcome;

    // Start is called before the first frame update
    void Start()
    {
        txtWelcome.text = "환영합니다 " + PhotonNetwork.LocalPlayer.NickName + "씨.";
    }

    private void OnClickGameStart()
    {
        NetworkManager.Inst.JoinLobby();
    }

    private void OnClickOption()
    {
        
    }

    private void OnClickExit()
    {
        Debug.Log("Application Quit");
        NetworkManager.Inst.Disconnect();
        Application.Quit();
    }
}
