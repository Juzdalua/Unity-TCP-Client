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
    private bool isMyTap = true;

    [Header("Tab Button")]
    [SerializeField] private Button myPartyButton;
    [SerializeField] private Button findPartyButton;

    [Header("My Party Contents")]
    [SerializeField] private GameObject myPartyContents;
    [SerializeField] private GameObject emptyMyPartyPrefab;
    [SerializeField] private GameObject addMyPartyNamePrefab;
    private Dictionary<ulong, GameObject> myPartyDic = new Dictionary<ulong, GameObject>();
    private ulong myPartyId = 0;

    [Header("Find Party Contents")]
    [SerializeField] private GameObject findPartyContents;
    [SerializeField] private GameObject emptyFindPartyPrefab;
    [SerializeField] private GameObject addFindPartyPrefab;
    [SerializeField] private GameObject partyDetailContents;
    [SerializeField] private GameObject partyDetailPrefab;
    private Dictionary<ulong, GameObject> allPartyDic = new Dictionary<ulong, GameObject>();
    private Dictionary<ulong, List<GameObject>> partyDetailDic = new Dictionary<ulong, List<GameObject>>();

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
        myPartyContents.SetActive(true);
        findPartyContents.SetActive(false);
        partyDetailContents.SetActive(false);
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
            ClientPacketHandler.Instance.GetAllParty(PlayerManager.Instance.GetMyPlayerId());
        }
    }

    public void ShowMyPartyProcess(S_MY_PARTY pkt)
    {
        isMyTap = true;
        myPartyId = pkt.PartyId;
        if (myPartyId == 0)
        {
            emptyMyPartyPrefab.SetActive(true);
            createPartyButton.gameObject.SetActive(true);
            withdrawPartyButton.gameObject.SetActive(false);
            myPartyContents.SetActive(true);
            findPartyContents.SetActive(false);
            partyDetailContents.SetActive(false);
            return;
        }

        if (myPartyDic.Count == 0)
        {
            for (int i = 0; i < pkt.Players.Count; i++)
            {
                GameObject partyNamePrefab = Instantiate(addMyPartyNamePrefab, myPartyContents.transform);
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

                GameObject partyNamePrefab = Instantiate(addMyPartyNamePrefab, myPartyContents.transform);
                partyNamePrefab.GetComponentInChildren<TextMeshProUGUI>().text = pkt.Players[i].Name;
                myPartyDic[pkt.Players[i].Id] = partyNamePrefab;

            }

            // 딕셔너리에 있고 패킷에 없는 경우 -> 삭제
            if (myPartyDic.Count != pkt.Players.Count)
            {
                List<ulong> keysToRemove = new List<ulong>();

                foreach (var partyObj in myPartyDic)
                {
                    bool found = false;
                    for (int i = 0; i < pkt.Players.Count; i++)
                    {
                        if (partyObj.Key == pkt.Players[i].Id)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        keysToRemove.Add(partyObj.Key);
                    }
                }

                // 임시 리스트를 사용하여 항목을 삭제
                foreach (var key in keysToRemove)
                {
                    Destroy(myPartyDic[key]);
                    myPartyDic.Remove(key);
                }
            }
        }

        if (myPartyDic.Count != 0)
        {
            emptyMyPartyPrefab.SetActive(false);
            createPartyButton.gameObject.SetActive(false);
            withdrawPartyButton.gameObject.SetActive(true);
        }
        else
        {
            emptyMyPartyPrefab.SetActive(true);
            createPartyButton.gameObject.SetActive(true);
            withdrawPartyButton.gameObject.SetActive(false);
        }

        foreach (var partyDic in allPartyDic)
        {
            Destroy(partyDic.Value.gameObject);
        }
        allPartyDic.Clear();

        partyDetailContents.SetActive(false);
        myPartyContents.SetActive(true);
        findPartyContents.SetActive(false);
    }

    public void ShowFindPartyProcess(S_ALL_PARTY pkt)
    {
        if (!isMyTap)
        {
            return;
        }

        foreach (var partyDetail in partyDetailDic)
        {
            for (int i = 0; i < partyDetail.Value.Count; i++)
            {
                Destroy(partyDetail.Value[i].gameObject);
            }
        }
        partyDetailDic.Clear();

        foreach (var allParty in allPartyDic)
        {
            Destroy(allParty.Value.gameObject);
        }
        allPartyDic.Clear();

        isMyTap = false;
        for (int i = 0; i < pkt.Parties.Count; i++)
        {
            var party = pkt.Parties[i];
            if (party.PartyStatus == PartyStatus.Unavailable)
                continue;

            GameObject partyList = Instantiate(addFindPartyPrefab, findPartyContents.transform);
            partyList.GetComponentInChildren<TextMeshProUGUI>().text = $"{party.PartyId}번 파티";
            partyList.GetComponentInChildren<Button>().onClick.AddListener(() => OnClickFindPartyDetail(party.PartyId));

            for (int j = 0; j < party.PartyPlayers.Count; j++)
            {
                GameObject partyDetailList = Instantiate(partyDetailPrefab, partyDetailContents.transform);
                partyDetailList.GetComponentInChildren<TextMeshProUGUI>().text = PlayerManager.Instance.GetPlayerNameByPlayerId(party.PartyPlayers[j].PlayerId) ?? "UNKNOWN";
                partyDetailList.SetActive(false);

                if (j == 0)
                {
                    partyDetailDic[party.PartyId] = new List<GameObject>();
                }
                partyDetailDic[party.PartyId].Add(partyDetailList);
            }

            allPartyDic[party.PartyId] = partyList;
        }

        if (allPartyDic.Count == 0)
        {
            emptyFindPartyPrefab.SetActive(true);
        }
        else
        {
            emptyFindPartyPrefab.SetActive(false);
        }

        partyDetailContents.SetActive(true);
        myPartyContents.SetActive(false);
        findPartyContents.SetActive(true);
    }

    public void OnClickFindPartyDetail(ulong partyId)
    {
        foreach (var partyDetail in partyDetailDic)
        {
            if (partyDetail.Key == partyId)
            {
                for (int i = 0; i < partyDetail.Value.Count; i++)
                {
                    partyDetail.Value[i].gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < partyDetail.Value.Count; i++)
                {
                    partyDetail.Value[i].gameObject.SetActive(false);
                }
            }
        }
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

        emptyMyPartyPrefab.SetActive(false);

        GameObject partyNamePrefab = Instantiate(addMyPartyNamePrefab, myPartyContents.transform);
        partyNamePrefab.GetComponentInChildren<TextMeshProUGUI>().text = PlayerManager.Instance.GetMyPlayerName();

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
        emptyMyPartyPrefab.SetActive(false);
        for (int i = 0; i < 1; i++)
        {
            GameObject partyNamePrefab = Instantiate(addMyPartyNamePrefab, myPartyContents.transform);
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

                foreach (var party in myPartyDic.Values)
                {
                    Destroy(party);
                }
                myPartyDic.Clear();

                emptyMyPartyPrefab.SetActive(true);
                createPartyButton.gameObject.SetActive(true);
                withdrawPartyButton.gameObject.SetActive(false);
                return;
            }

            // 다른 사람이 탈퇴
            Destroy(myPartyDic[pkt.PartyId].gameObject);
            myPartyDic.Remove(pkt.PartyId);

            emptyMyPartyPrefab.SetActive(false);
            createPartyButton.gameObject.SetActive(false);
            withdrawPartyButton.gameObject.SetActive(true);
        }
    }


    // Find Party Contents
}
