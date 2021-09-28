using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using HashTable = ExitGames.Client.Photon.Hashtable;
using System;

public class RoomUI : BaseUI
{
    const int MAX_PLAYER_COUNT = 12;

    public TextMeshProUGUI txtRoomName;
    public TextMeshProUGUI txtUsers;
    public RoomItemUI[] leftProfiles;
    public RoomItemUI[] rightProfiles;
    public Image imgMap;
    public TextMeshProUGUI txtBtnReadyOrStart;

    PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        ShowRoomUI();
    }

    private void OnClickReadyOrStart()
    {
        // �غ� Ȥ�� ���� ����
        if(PhotonNetwork.IsMasterClient)
        {
            // ���� ����
        }
        else
        {
            // �غ�
            // ����Ʈ���� ���� �÷��̾��� �г��Ӱ� ��ġ�ϴ� ������ ã�� ���� ���¸� �����Ѵ�.
            string myName = PhotonNetwork.LocalPlayer.NickName;
            for (int i = 0; i < leftProfiles.Length; i++)
            {
                if(leftProfiles[i].Nickname == myName)
                    pv.RPC("RPC_SetReady", RpcTarget.AllBuffered, true, i);
            }

            for (int i = 0; i < rightProfiles.Length; i++)
            {
                if (rightProfiles[i].Nickname == myName)
                    pv.RPC("RPC_SetReady", RpcTarget.AllBuffered, false, i);
            }
        }
    }

    [PunRPC]
    private void RPC_SetReady(bool bLeft, int i)
    {
        if(bLeft)   leftProfiles[i].SetReady();
        else        rightProfiles[i].SetReady();
    }

    private void OnClickRoomExit()
    {
        // ���� ������ ������ ��� �ٸ� ������ �������� �����Ѵ�.
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            SetNewMaster();

        // �ڷΰ���
        NetworkManager.Inst.LeaveRoom();
    }

    private void SetNewMaster()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            if (PhotonNetwork.LocalPlayer != PhotonNetwork.PlayerList[0])   PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[0]);
            else                                                            PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[1]);
        }
        else
            PhotonNetwork.CurrentRoom.IsOpen = true;
    }

    private void InitPlayerList()
    {
        for (int i = 0; i < MAX_PLAYER_COUNT; i++)
        {
            // �ε����� ���� ���� �÷��̾� ������ ������ ū���� ���� ����Ʈ�� ����Ѱ��� ä����� �����Ѵ�.
            if (i < PhotonNetwork.CurrentRoom.PlayerCount)
            {
                // �÷��̾��� ������ ����Ʈ�� ä���д�.
                if (i % 2 == 0) leftProfiles[i / 2].Initialize(PhotonNetwork.PlayerList[i]);
                else            rightProfiles[i / 2].Initialize(PhotonNetwork.PlayerList[i]);
            }
            else
            {
                // �� ����Ʈ�� �ʱ�ȭ�Ѵ�.
                if (i % 2 == 0) leftProfiles[i / 2].Initialize(player: null);
                else            rightProfiles[i / 2].Initialize(player: null);
            }
        }
    }

    public void ShowRoomUI()
    {
        HashTable ht = PhotonNetwork.CurrentRoom.CustomProperties;
        
        // �� ���� �����ֱ�
        txtRoomName.text = (string)ht["Title"];

        // �ο��� �����ֱ�
        txtUsers.text = "�ο��� " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        // �÷��̾� ������ ����Ʈ �ʱ�ȭ
        InitPlayerList();

        // ���� & ���� ��ư �ؽ�Ʈ ����
        if (PhotonNetwork.IsMasterClient)   txtBtnReadyOrStart.text = "�����ϱ�";
        else                                txtBtnReadyOrStart.text = "����";
    }

    public void OnEnterUser(Player player)
    {
        // �ο��� ������Ʈ.
        txtUsers.text = "�ο��� " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        // ������ ����Ʈ ������Ʈ
        int userCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (userCount % 2 != 0) leftProfiles[userCount / 2].Initialize(player);
        else                    rightProfiles[(userCount / 2) - 1].Initialize(player);
    }

    public void OnLeftUser(Player player)
    {
        // �ο��� ������Ʈ.
        txtUsers.text = "�ο��� " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        // ������ ����Ʈ ������Ʈ
        Debug.Log("Remain Player: " + PhotonNetwork.CurrentRoom.PlayerCount);
        for(int i = 0; i < leftProfiles.Length; i++)
        {
            // ���� �ڸ��� ���� ���� ������ �ִ� �ڸ���� �� ���� �׸� �ִ� ������ ������.
            if (leftProfiles[i].Nickname == player.NickName)
            {
                int j = i;
                for (; j < leftProfiles.Length - 1; j++)
                {
                    leftProfiles[j].Initialize(leftProfiles[j + 1]);
                }
                leftProfiles[j].Initialize(player: null);
                return;
            }
        }

        for (int i = 0; i < rightProfiles.Length; i++)
        {
            if (rightProfiles[i].Nickname == player.NickName)
            {
                int j = i;
                for (; j < rightProfiles.Length - 1; j++)
                {
                    rightProfiles[j].Initialize(rightProfiles[j + 1]);
                }
                rightProfiles[j].Initialize(player: null);
                return;
            }
        }
    }
}