using System.Collections;
using System.Collections.Generic;
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
    private List<GameObject> myPartyList = new List<GameObject>();

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
    }

    public void ClosePartyCanvas()
    {
        isOpen = false;
        PartyObject.SetActive(false);
    }

    public void ShowParty(string type)
    {
        if(type == "my")
        {
            myPartyContents.SetActive(true);
            findPartyContents.SetActive(false);
        }
        else if(type == "find")
        {
            myPartyContents.SetActive(false);
            findPartyContents.SetActive(true);
        }
    }

    // My Party Contents
    public void CreateParty()
    {
        emptyPartyPrefab.SetActive(false);
        
        GameObject partyNamePrefab = Instantiate(addPartyNamePrefab, myPartyContents.transform);
        partyNamePrefab.GetComponentInChildren<TextMeshProUGUI>().text = PlayerManager.Instance.GetMyPlayerName();
        myPartyList.Add(partyNamePrefab);

        createPartyButton.gameObject.SetActive(false);
        withdrawPartyButton.gameObject.SetActive(true);
    }

    public void JoinParty()
    {
        emptyPartyPrefab.SetActive(false);
        for(int i=0; i<1; i++)
        {
            GameObject partyNamePrefab = Instantiate(addPartyNamePrefab, myPartyContents.transform);
            partyNamePrefab.GetComponentInChildren<TextMeshProUGUI>().text = "new player";
            myPartyList.Add(partyNamePrefab);
        }

        createPartyButton.gameObject.SetActive(false);
        withdrawPartyButton.gameObject.SetActive(true);
    }

    public void WithdrawParty()
    {
        emptyPartyPrefab.SetActive(true);
        
        for(int i=0; i<myPartyList.Count; i++)
        {
            Destroy(myPartyList[i].gameObject);
        }
        myPartyList.Clear();

        createPartyButton.gameObject.SetActive(true);
        withdrawPartyButton.gameObject.SetActive(false);
    }


    // Find Party Contents
}
