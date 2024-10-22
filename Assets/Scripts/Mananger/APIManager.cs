using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class ErrorDTO
{
    public int errorCode;
    public string errorMsg;
}
public class AccountDTO
{
    public int accountId;
    public string name;
}
public class PlayerDTO
{
    public int accountId;
    public int playerId;
    public string name;
    public float posX;
    public float posY;
    public int maxHP;
    public int currentHP;
}

public class PartyDTO
{
    public int partyId;
}

public class APIManager : Singleton<APIManager>
{
    private readonly string baseUrl = "http://localhost:4000";

    public void GetAccountById(int accountId)
    {
        StartCoroutine(GetAccountCoroutine(accountId));
    }

    private IEnumerator GetAccountCoroutine(int accountId)
    {
        string url = $"{baseUrl}/account/{accountId}"; 
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest(); 

            if (request.result != UnityWebRequest.Result.Success)
            {
                ErrorDTO errorDto = JsonUtility.FromJson<ErrorDTO>(request.downloadHandler.text);
                AlertManager.Instance.AlertPopup(errorDto.errorMsg);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"Response: {jsonResponse}");
            }
        }
    }

    // Signup
    public void Signup(string name, string password)
    {
        StartCoroutine(SignupCoroutine(name, password));
    }

    private IEnumerator SignupCoroutine(string name, string password)
    {
        string url = $"{baseUrl}/account/signup";
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("password", password);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                ErrorDTO errorDto = JsonUtility.FromJson<ErrorDTO>(request.downloadHandler.text);
                AlertManager.Instance.AlertPopup(errorDto.errorMsg);
            }
            else
            {
                AlertManager.Instance.AlertPopup("Signup Success");
            }
        }
    }

    // Signin
    public void Signin(string name, string password)
    {
        StartCoroutine(SigninCoroutine(name, password));
    }

    private IEnumerator SigninCoroutine(string name, string password)
    {
        string url = $"{baseUrl}/account/signin";
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("password", password);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                ErrorDTO errorDto = JsonUtility.FromJson<ErrorDTO>(request.downloadHandler.text);
                AlertManager.Instance.AlertPopup(errorDto.errorMsg);
            }
            else
            {
                PlayerDTO accountNPlayer = JsonUtility.FromJson<PlayerDTO>(request.downloadHandler.text);
                PlayerManager.Instance.SetPlayerId((ulong)accountNPlayer.playerId);
                PlayerManager.Instance.SetPlayerName(accountNPlayer.name);

                //Scene 전환
                ClientManager.Instance.SetSceneType(SceneType.Game);
                MainMenuManager.Instance.ActiveMenu();
                SceneManager.LoadScene("01.MainScene");
                GameManager.playerData = accountNPlayer;
            }
        }
    }

    // Move
    public void UpdateMove(int playerId, float posX, float posY)
    {
        StartCoroutine(UpdateMoveCoroutine(playerId, posX, posY));
    }

    private IEnumerator UpdateMoveCoroutine(int playerId, float posX, float posY)
    {
        string url = $"{baseUrl}/player/move";
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerId);
        form.AddField("posX", posX.ToString());
        form.AddField("posY", posY.ToString());

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                ErrorDTO errorDto = JsonUtility.FromJson<ErrorDTO>(request.downloadHandler.text);
                AlertManager.Instance.AlertPopup(errorDto.errorMsg);
            }
        }
    }

    // Create Party
    public void CreateParty(int playerId)
    {
        StartCoroutine(CreatePartyCoroutine(playerId));
    }

    private IEnumerator CreatePartyCoroutine(int playerId)
    {
        string url = $"{baseUrl}/player/party";
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerId);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                ErrorDTO errorDto = JsonUtility.FromJson<ErrorDTO>(request.downloadHandler.text);
                AlertManager.Instance.AlertPopup(errorDto.errorMsg);
            }
            else
            {
                PartyDTO partyData = JsonUtility.FromJson<PartyDTO>(request.downloadHandler.text);
                ClientPacketHandler.Instance.CreateParty((ulong)playerId, (ulong)partyData.partyId);
            }
        }
    }

}
