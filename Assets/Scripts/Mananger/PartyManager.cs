using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyManager : Singleton<PartyManager>
{
    private bool isOpen = false;
    [SerializeField] private GameObject PartyObject;

    [Header("Tab Button")]
    [SerializeField] private Button myPartyButton;
    [SerializeField] private Button findPartyButton;

    [Header("My Party Contents")]
    [SerializeField] private GameObject myPartyContents;
    [SerializeField] private GameObject emptyPartyPrefab;
    [SerializeField] private GameObject addPartyNamePrefab;
    private Dictionary<ulong, GameObject> myPartyDic = new Dictionary<ulong, GameObject>();
    private ulong myPartyId = 0;

    [Header("Find Party Contents")]
    [SerializeField] private GameObject findPartyContents;

    [Header("Create Delete Button")]
    [SerializeField] private Button createPartyButton;
    [SerializeField] private Button withdrawPartyButton;

    private void Awake()
    {
        ClosePartyCanvas();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isOpen)
            {
                OpenPartyCanvas();
            }
            else
            {
                ClosePartyCanvas();
            }
        }
    }

    public void OpenPartyCanvas()
    {
        isOpen = true;
        PartyObject.SetActive(true);
        ShowParty("my");
    }

    public void ClosePartyCanvas()
    {
        isOpen = false;
        PartyObject.SetActive(false);
    }

    public void ShowParty(string type)
    {
        if (type == "my")
        {
            ClientPacketHandler.Instance.GetMyParty(PlayerManager.Instance.GetMyPlayerId());
        }
        else if (type == "find")
        {

        }
    }

    public void ShowMyPartyProcess(S_MY_PARTY pkt)
    {
        Debug.Log(pkt);
        myPartyId = pkt.PartyId;
        if (myPartyId == 0)
        {
            emptyPartyPrefab.SetActive(true);
            createPartyButton.gameObject.SetActive(true);
            withdrawPartyButton.gameObject.SetActive(false);
            myPartyContents.SetActive(true);
            findPartyContents.SetActive(false);
            return;
        }

        if (myPartyDic.Count == 0)
        {
            for (int i = 0; i < pkt.Players.Count; i++)
            {
                GameObject partyNamePrefab = Instantiate(addPartyNamePrefab, myPartyContents.transform);
                partyNamePrefab.GetComponentInChildren<TextMeshProUGUI>().text = pkt.Players[i].Name;
                myPartyDic[pkt.Players[i].Id] = partyNamePrefab;
            }
        }

        else
        {
            // 딕셔너리에 없고 패킷에 있는 경우 -> 추가
            for (int i = 0; i < pkt.Players.Count; i++)
            {
                if (myPartyDic.ContainsKey(pkt.Players[i].Id))
                    continue;

                GameObject partyNamePrefab = Instantiate(addPartyNamePrefab, myPartyContents.transform);
                partyNamePrefab.GetComponentInChildren<TextMeshProUGUI>().text = pkt.Players[i].Name;
                myPartyDic[pkt.Players[i].Id] = partyNamePrefab;

            }

            // 딕셔너리에 있고 패킷에 없는 경우 -> 삭제
            if (myPartyDic.Count != pkt.Players.Count)
            {
                foreach (var partyObj in myPartyDic)
                {
                    for (int i = 0; i < pkt.Players.Count; i++)
                    {
                        if (partyObj.Key == pkt.Players[i].Id)
                            continue;

                        Destroy(partyObj.Value);
                        myPartyDic.Remove(partyObj.Key);
                    }
                }
            }
        }

        if (myPartyDic.Count != 0)
        {
            emptyPartyPrefab.SetActive(false);
            createPartyButton.gameObject.SetActive(false);
            withdrawPartyButton.gameObject.SetActive(true);
        }
        else
        {
            emptyPartyPrefab.SetActive(true);
            createPartyButton.gameObject.SetActive(true);
            withdrawPartyButton.gameObject.SetActive(false);
        }

        myPartyContents.SetActive(true);
        findPartyContents.SetActive(false);
    }

    public void ShowFindPartyProcess()
    {
        myPartyContents.SetActive(false);
        findPartyContents.SetActive(true);
    }

    // My Party Contents
    public void CreateParty()
    {
        if (myPartyId != 0)
        {
            Debug.Log("Already join party");
            return;
        }

        ClientPacketHandler.Instance.CreateParty(PlayerManager.Instance.GetMyPlayerId());
    }

    public void CreatePartyProcess(S_CREATE_PARTY pkt)
    {
        if (myPartyId != 0)
        {
            Debug.Log("Already join party");
            return;
        }

        emptyPartyPrefab.SetActive(false);

        GameObject partyNamePrefab = Instantiate(addPartyNamePrefab, myPartyContents.transform);
        partyNamePrefab.GetComponentInChildren<TextMeshProUGUI>().text = $"{PlayerManager.Instance.GetMyPlayerName()}의 파티";

        // Party Info
        myPartyDic[pkt.PartyId] = partyNamePrefab;
        myPartyId = pkt.PartyId;

        createPartyButton.gameObject.SetActive(false);
        withdrawPartyButton.gameObject.SetActive(true);
    }

    public void JoinParty()
    {

    }

    public void JoinPartyProcess()
    {
        emptyPartyPrefab.SetActive(false);
        for (int i = 0; i < 1; i++)
        {
            GameObject partyNamePrefab = Instantiate(addPartyNamePrefab, myPartyContents.transform);
            partyNamePrefab.GetComponentInChildren<TextMeshProUGUI>().text = "new player";
            //parties[pkt.PartyId] = partyNamePrefab;
        }

        createPartyButton.gameObject.SetActive(false);
        withdrawPartyButton.gameObject.SetActive(true);
    }

    public void WithdrawParty()
    {
        if (myPartyId == 0)
        {
            Debug.Log("Not Party Join");
            return;
        }
        ClientPacketHandler.Instance.WithdrawParty(PlayerManager.Instance.GetMyPlayerId(), myPartyId);
    }

    public void WithdrawPartyProcess(S_WITHDRAW_PARTY pkt)
    {
        if (myPartyId == 0)
        {
            Debug.Log("Not Party Join");
            return;
        }

        else if (myPartyId == pkt.PartyId)
        {
            // 내가 탈퇴
            if (pkt.Players.Count == 0 || pkt.WithdrawPlayerId == PlayerManager.Instance.GetMyPlayerId())
            {
                myPartyId = 0;

                foreach(var party in myPartyDic.Values)
                {
                    Destroy(party);
                }
                myPartyDic.Clear();

                emptyPartyPrefab.SetActive(true);
                createPartyButton.gameObject.SetActive(true);
                withdrawPartyButton.gameObject.SetActive(false);
                return;
            }

            // 다른 사람이 탈퇴
            Destroy(myPartyDic[pkt.PartyId].gameObject);
            myPartyDic.Remove(pkt.PartyId);
            
            emptyPartyPrefab.SetActive(false);
            createPartyButton.gameObject.SetActive(false);
            withdrawPartyButton.gameObject.SetActive(true);
        }
    }


    // Find Party Contents
}
