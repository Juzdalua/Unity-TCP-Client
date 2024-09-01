using UnityEngine;
public class GameManager : MonoBehaviour
{
    private ClientManager _networkManager;

    void Start()
    {
        _networkManager = GetComponent<ClientManager>();

        if (_networkManager != null)
        {
            _networkManager.ConnectToServer(_networkManager.serverIP, _networkManager.serverPort, SceneType.Game);
        }
        else
        {
            Debug.Log("ClientManager not found in the scene.");
        }
    }

    private void Update()
    {
        if(_networkManager != null)
        {
            _networkManager.CheckSocket(SceneType.Game);
        }
    }
}
