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
                Debug.LogError($"Error: {request.error}");
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
                AlertManager.Instance.AlertPopup(request.error);
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


    //public void CreateAccount(string name, string password)
    //{
    //    StartCoroutine(CreateAccountCoroutine(name, password));
    //}

    //private IEnumerator CreateAccountCoroutine(string name, string password)
    //{
    //    string url = $"{baseUrl}/account"; // API ��������Ʈ
    //    AccountDTO accountDto = new AccountDTO { Name = name, Password = password };
    //    string jsonData = JsonUtility.ToJson(accountDto); // DTO�� JSON���� ��ȯ

    //    using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, jsonData))
    //    {
    //        request.SetRequestHeader("Content-Type", "application/json"); // ��� ����
    //        yield return request.SendWebRequest(); // ��û ���� ���

    //        if (request.result != UnityWebRequest.Result.Success)
    //        {
    //            Debug.LogError($"Error: {request.error}");
    //        }
    //        else
    //        {
    //            // ���������� ������ ���� ���
    //            string jsonResponse = request.downloadHandler.text;
    //            Debug.Log($"Response: {jsonResponse}");
    //            // JSON �Ľ� �� ������ ó��
    //        }
    //    }
    //}
}
