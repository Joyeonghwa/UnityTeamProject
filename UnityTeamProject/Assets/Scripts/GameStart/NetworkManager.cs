using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using HashTable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager netManager;

    public static NetworkManager Inst
    {
        get
        {
            return netManager;
        }
    }

    public List<RoomInfo> roomInfos;
    public LobbyUI lobbyUI;
    public RoomUI roomUI;

    bool bConnect = false;
    string myNickname;

    // Start is called before the first frame update
    void Awake()
    {
        InitManager();
    }

    private void InitManager()
    {
        netManager = this;
        roomInfos = new List<RoomInfo>();
        DontDestroyOnLoad(gameObject);
    }

    public void Register(string email, string nickname, string password, TextMeshProUGUI txtAlert)
    {
        // ȸ�������� �õ��Ѵ�.
        // ȸ�������ϴ� ���� �ٸ� UI�� Ŭ������ ���ϰ� �������� Ȱ��ȭ�Ѵ�.
        StartUI_Manager.Inst.SetCover(true);

        // ȸ������ ����� ���� �ӽ� �̺�Ʈ �Լ����� �����.
        Action<LoginResult> OnLoginSuccess = delegate (LoginResult result)
        {
            if (result.InfoResultPayload.PlayerProfile != null)
                myNickname = result.InfoResultPayload.PlayerProfile.DisplayName;
            else
                myNickname = result.PlayFabId;
            NetworkManager.Inst.Connect();
        };

        Action<PlayFabError> OnLoginFailed = delegate (PlayFabError error)
        {
            StartUI_Manager.Inst.ChangeUI(UI_Type.LOGIN);
            print("! ������ �����ϴµ� �����߽��ϴ�.");
            StartUI_Manager.Inst.SetCover(false);
        };

        Action<RegisterPlayFabUserResult> OnRegisterSuccess = delegate (RegisterPlayFabUserResult result)
        {
            // ȸ������ ������ �ٷ� �α����� �õ��Ѵ�.
            var request = new LoginWithEmailAddressRequest
            {
                Email = email,
                Password = password,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                {
                    GetPlayerProfile = true
                }
            };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailed);
            NetworkManager.Inst.Connect();
        };

        Action<PlayFabError> OnRegisterFailed = delegate (PlayFabError error)
        {
            txtAlert.text = "! ȸ�����Կ� �����߽��ϴ�.";
            print("Register Failed: " + error.GenerateErrorReport());
            StartUI_Manager.Inst.SetCover(false);
        };

        // ȸ�������� �Ѵ�.(=userData�� userDB�� �����Ѵ�.)
        print("Title ID: " + PlayFabSettings.TitleId);
        var request = new RegisterPlayFabUserRequest { 
            Email = email, Password = password, Username = nickname, DisplayName = nickname, RequireBothUsernameAndEmail = true };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailed);
    }

    public void Login(string email, string password, TextMeshProUGUI txtAlert)
    {
        // �α����� �õ��Ѵ�.
        // �α����ϴ� ���� �ٸ� UI�� Ŭ������ ���ϰ� �������� Ȱ��ȭ�Ѵ�.
        StartUI_Manager.Inst.SetCover(true);

        // �α��� ����� ���� �ӽ� �̺�Ʈ �Լ����� �����.
        Action<LoginResult> OnLoginSuccess = delegate (LoginResult result)
        {
            if(result.InfoResultPayload.PlayerProfile != null)
                myNickname = result.InfoResultPayload.PlayerProfile.DisplayName;
            else
                myNickname = result.PlayFabId;

            NetworkManager.Inst.Connect();
        };

        Action<PlayFabError> OnLoginFailed = delegate (PlayFabError error)
        {
            txtAlert.text = "! ��ġ�ϴ� ���� ������ �����ϴ�.";
            print("Login Failed: " + error.GenerateErrorReport());
            StartUI_Manager.Inst.SetCover(false);
        };

        // �α����� �Ѵ�.(=userData�� userDB�� �����Ѵ�.)
        var request = new LoginWithEmailAddressRequest { 
            Email = email, Password = password, 
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams() {
                GetPlayerProfile = true
            } 
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailed);
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        if(!bConnect)
        {
            print("���� ���� �Ϸ�");
            PhotonNetwork.LocalPlayer.NickName = myNickname;
            StartUI_Manager.Inst.SetCover(false);
            StartUI_Manager.Inst.ChangeUI(UI_Type.TITLE);

            bConnect = true;
        }
        else
        {
            JoinLobby();
        }
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("���� ���� ����");
    }

    public void JoinLobby() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        print("�κ� ����");
        StartUI_Manager.Inst.ChangeUI(UI_Type.LOBBY);
    }

    public void LeaveLobby() => PhotonNetwork.LeaveLobby();

    public override void OnLeftLobby()
    {
        print("�κ� ����");
        StartUI_Manager.Inst.ChangeUI(UI_Type.TITLE);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomInfos.Count > 0 && roomList[i].RemovedFromList)
            {
                // �ƹ��� ���� ���� ���, �� ��Ͽ��� �����Ѵ�.
                roomInfos.Remove(roomInfos.Find(info => info.Name == roomList[i].Name));
            }
            else if(!roomList[i].RemovedFromList)
            {
                // ����� �ִ� ���� ���, �� ���� ��Ͽ� ���� ���� ���� ������ �˻��ϰ�, ���� ���� ���̶�� ���� �� ��Ͽ� �ִ´�.
                if(!roomInfos.Contains(roomList[i]))    roomInfos.Add(roomList[i]);
            }
        }

        // �κ� UI�� Ȱ��ȭ�� ���¶�� �� ����� ������Ʈ�Ѵ�.
        if(lobbyUI.enabled)
            lobbyUI.UpdateRoomList();
    }

    public void CreateRoom(string title, MaxPlayer maxPlayer, GameMode gameMode, MapName mapName)
    {
        // �� �ɼ��� �����Ѵ�.
        RoomOptions option = new RoomOptions();
        option.IsVisible = true;

        // �ִ� �÷��̾� ���� �����Ѵ�.
        switch(maxPlayer)
        {
            case MaxPlayer.TWO:     option.MaxPlayers = 2; break;
            case MaxPlayer.FOUR:    option.MaxPlayers = 4; break;
            case MaxPlayer.SIX:     option.MaxPlayers = 6; break;
        }

        // ���� ���, ���� �����Ѵ�.
        option.CustomRoomProperties = new HashTable{ { "Title", title }, { "GameMode", gameMode }, { "MapName", mapName } };

        // �κ� �ǳ��� Ŀ���� ������Ƽ �Ӽ����� �ʱ�ȭ�Ѵ�.
        string[] customPropertiesForLobby = new string[3];
        customPropertiesForLobby[0] = "Title";
        customPropertiesForLobby[1] = "GameMode";
        customPropertiesForLobby[2] = "MapName";
        option.CustomRoomPropertiesForLobby = customPropertiesForLobby;

        // �� id�� �������� ���ѵ� �������ش�. �����ϸ� �� ������ �ٽ� ���Ѵ�.
        const int NAME_LEN = 8;
        string roomName = "";
        for(int i = 0; i < NAME_LEN; i++)
        {
            roomName += UnityEngine.Random.Range('A', 'B');
            if(i < NAME_LEN - 1)    continue;

            if(PhotonNetwork.CreateRoom(roomName, option))
                return;

            i = -1;
        }
    }

    public override void OnCreatedRoom()
    {
        print("�� ����");
        StartUI_Manager.Inst.ChangeUI(UI_Type.ROOM);
    }

    public void JoinRoom(string roomID) => PhotonNetwork.JoinRoom(roomID);

    public override void OnJoinedRoom()
    {
        print("�� ����");
        StartUI_Manager.Inst.ChangeUI(UI_Type.ROOM);
    }

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(roomUI.enabled)  roomUI.OnEnterUser(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (roomUI.enabled)
        {
            roomUI.OnLeftUser(otherPlayer);
            roomUI.ShowRoomUI();
        }
    }

    public override void OnLeftRoom()
    {
        print("�� ����");
        // ���� ������ �ѹ� �������� Disconnect�ѵ� �ٽ� Connect�Ѵ�. �� �̰͵�� ���õ� �̺�Ʈ �Լ����� �˴� ȣ��ȴ�.
    }
}
