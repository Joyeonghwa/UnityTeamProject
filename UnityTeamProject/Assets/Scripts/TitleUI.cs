using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUI : BaseUI
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        NetworkManager.Inst.DisConnect();
        Application.Quit();
    }
}
