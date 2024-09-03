using Google.Protobuf.Protocol;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private bool isStart = false;
    public static S_LOGIN loginPkt;

    private void Update()
    {    
        if(SceneManager.GetActiveScene().name == "01.MainScene" && !isStart && loginPkt != null)
        {
            ProcessLoginToServer();
        }
    }

    public void ProcessLoginToServer()
    {

        isStart = true;
        PlayerManager.Instance.CreatePlayer(loginPkt);
    }
}
