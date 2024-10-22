using Google.Protobuf.Protocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Login")]
    private bool isStart = false;
    //public static S_LOGIN loginPkt;
    public static PlayerDTO playerData;
    

    private void Update()
    {    
        //if(SceneManager.GetActiveScene().name == "01.MainScene" && !isStart && loginPkt != null)
        if(SceneManager.GetActiveScene().name == "01.MainScene" && !isStart && playerData != null)
        {
            ProcessLoginToServer();
        }
    }

    public void ProcessLoginToServer()
    {

        isStart = true;
        //PlayerManager.Instance.CreatePlayer(loginPkt);
        PlayerManager.Instance.CreatePlayer(playerData);
    }
}
