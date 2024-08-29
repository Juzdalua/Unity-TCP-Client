using UnityEngine;
public class GameManager : MonoBehaviour
{
    public string serverIP = "127.0.0.1"; // ���� IP �ּ�
    public int serverPort = 7777;        // ���� ��Ʈ ��ȣ
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
