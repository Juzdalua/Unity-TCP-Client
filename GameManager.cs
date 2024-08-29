using UnityEngine;
public class GameManager : MonoBehaviour
{
    public string serverIP = "127.0.0.1"; // 서버 IP 주소
    public int serverPort = 7777;        // 서버 포트 번호
    private ClientManager _networkManager;

    void Start()
    {
        _networkManager = FindObjectOfType<ClientManager>();

        if (_networkManager != null)
        {
            _networkManager.ConnectToServer(serverIP, serverPort);
        }
        else
        {
            Debug.LogError("ClientManager not found in the scene.");
        }
    }
}
