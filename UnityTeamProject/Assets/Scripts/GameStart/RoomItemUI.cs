using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System;

public class RoomItemUI : MonoBehaviour
{
    public TextMeshProUGUI txtNickName;
    public Image imgReadyState;

    [Header("���� ���ο� ����� �̹���")]
    public Sprite spriteReady;
    public Sprite spriteHost;

    bool bIsReady = false;

    public string Nickname
    {
        get
        {
            return txtNickName.text;
        }
    }

    public bool IsReady
    {
        get
        {
            return bIsReady;
        }
    }

    public void Initialize(Player player)
    {
        Image background = GetComponent<Image>();

        if (player != null)
        {
            // �������� �г��Ӱ� ���� ���ο� ���� ���� �̹����� �����Ѵ�.
            txtNickName.text = player.NickName;
            if (player.IsMasterClient) imgReadyState.sprite = spriteHost;
            else imgReadyState.sprite = spriteReady;

            background.color = new Color(background.color.r, background.color.g, background.color.b, 0.2f);
            imgReadyState.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else
        {
            // �� ����Ʈ�� �ʱ�ȭ��Ų��.
            txtNickName.text = "";
            imgReadyState.sprite = spriteReady;

            background.color = new Color(background.color.r, background.color.g, background.color.b, 0.1f);
            imgReadyState.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
        }
    }

    public void Initialize(RoomItemUI item)
    {
        Image background = GetComponent<Image>();

        txtNickName.text = item.txtNickName.text;
        if(txtNickName.text == "")
        {
            // �� �������� ���
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0.1f);
            imgReadyState.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
        }
        else
        {
            // �������� �ִ� ���
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0.2f);
            imgReadyState.color = item.imgReadyState.color;
        }
    }

    public void SetReady()
    {
        // ������ ��� ���� ���θ� ���� �ʿ䰡 �����Ƿ� �Ѿ��.
        if(imgReadyState.sprite == spriteHost)  return;

        // ���� ���ο� ���� ���� �̹����� �����Ѵ�.
        bIsReady = !bIsReady;
        if(bIsReady)    imgReadyState.color = Color.yellow;
        else            imgReadyState.color = Color.white;
    }

    public void SetNewMasterClient()
    {
        imgReadyState.sprite = spriteHost;
    }
}
