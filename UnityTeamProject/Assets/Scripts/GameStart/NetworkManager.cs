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
        // 회원가입을 시도한다.
        // 회원가입하는 동안 다른 UI를 클릭하지 못하게 가림막을 활성화한다.
        StartUI_Manager.Inst.SetCover(true);

        // 회원가입 결과에 따른 임시 이벤트 함수들을 만든다.
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
            print("! 서버와 접속하는데 실패했습니다.");
            StartUI_Manager.Inst.SetCover(false);
        };

        Action<RegisterPlayFabUserResult> OnRegisterSuccess = delegate (RegisterPlayFabUserResult result)
        {
            // 회원가입 성공시 바로 로그인을 시도한다.
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
            txtAlert.text = "! 회원가입에 실패했습니다.";
            print("Register Failed: " + error.GenerateErrorReport());
            StartUI_Manager.Inst.SetCover(false);
        };

        // 회원가입을 한다.(=userData를 userDB에 저장한다.)
        print("Title ID: " + PlayFabSettings.TitleId);
        var request = new RegisterPlayFabUserRequest { 
            Email = email, Password = password, Username = nickname, DisplayName = nickname, RequireBothUsernameAndEmail = true };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailed);
    }

    public void Login(string email, string password, TextMeshProUGUI txtAlert)
    {
        // 로그인을 시도한다.
        // 로그인하는 동안 다른 UI를 클릭하지 못하게 가림막을 활성화한다.
        StartUI_Manager.Inst.SetCover(true);

        // 로그인 결과에 따른 임시 이벤트 함수들을 만든다.
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
            txtAlert.text = "! 일치하는 유저 정보가 없습니다.";
            print("Login Failed: " + error.GenerateErrorReport());
            StartUI_Manager.Inst.SetCover(false);
        };

        // 로그인을 한다.(=userData를 userDB에 저장한다.)
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
            print("서버 연결 완료");
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
        print("서버 연결 끊김");
    }

    public void JoinLobby() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        print("로비 입장");
        StartUI_Manager.Inst.ChangeUI(UI_Type.LOBBY);
    }

    public void LeaveLobby() => PhotonNetwork.LeaveLobby();

    public override void OnLeftLobby()
    {
        print("로비 떠남");
        StartUI_Manager.Inst.ChangeUI(UI_Type.TITLE);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomInfos.Count > 0 && roomList[i].RemovedFromList)
            {
                // 아무도 없는 방인 경우, 방 목록에서 제거한다.
                roomInfos.Remove(roomInfos.Find(info => info.Name == roomList[i].Name));
            }
            else if(!roomList[i].RemovedFromList)
            {
                // 사람이 있는 방인 경우, 이 방이 목록에 없는 새로 생긴 방인지 검사하고, 새로 생긴 방이라면 기존 방 목록에 넣는다.
                if(!roomInfos.Contains(roomList[i]))    roomInfos.Add(roomList[i]);
            }
        }

        // 로비 UI가 활성화된 상태라면 방 목록을 업데이트한다.
        if(lobbyUI.enabled)
            lobbyUI.UpdateRoomList();
    }

    public void CreateRoom(string title, MaxPlayer maxPlayer, GameMode gameMode, MapName mapName)
    {
        // 방 옵션을 설정한다.
        RoomOptions option = new RoomOptions();
        option.IsVisible = true;

        // 최대 플레이어 수를 설정한다.
        switch(maxPlayer)
        {
            case MaxPlayer.TWO:     option.MaxPlayers = 2; break;
            case MaxPlayer.FOUR:    option.MaxPlayers = 4; break;
            case MaxPlayer.SIX:     option.MaxPlayers = 6; break;
        }

        // 게임 모드, 맵을 설정한다.
        option.CustomRoomProperties = new HashTable{ { "Title", title }, { "GameMode", gameMode }, { "MapName", mapName } };

        // 로비에 건네줄 커스텀 프로퍼티 속성들을 초기화한다.
        string[] customPropertiesForLobby = new string[3];
        customPropertiesForLobby[0] = "Title";
        customPropertiesForLobby[1] = "GameMode";
        customPropertiesForLobby[2] = "MapName";
        option.CustomRoomPropertiesForLobby = customPropertiesForLobby;

        // 방 id을 랜덤으로 정한뒤 생성해준다. 실패하면 방 제목을 다시 정한다.
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
        print("방 생성");
        StartUI_Manager.Inst.ChangeUI(UI_Type.ROOM);
    }

    public void JoinRoom(string roomID) => PhotonNetwork.JoinRoom(roomID);

    public override void OnJoinedRoom()
    {
        print("방 입장");
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
        print("방 떠남");
        // 방을 떠나면 한번 서버에서 Disconnect한뒤 다시 Connect한다. 즉 이것들과 관련된 이벤트 함수들이 죄다 호출된다.
    }
}
