using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    private TcpClient _client;
    private NetworkStream _stream;
    private bool _isConnected = false;
    private PlayerManager _playerManager;
    private float lastRecvTime = 0;

    public string serverIP = "127.0.0.1"; // ���� IP �ּ�
    public int serverPort = 7777;        // ���� ��Ʈ ��ȣ

    void Start()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
    }

    private void Update()
    {
        
    }

    public void CheckSocket()
    {
        if (_isConnected)
            Heartbeat();
        else
            ConnectToServer(serverIP, serverPort);
    }

    public async void ConnectToServer(string ipAddress, int port)
    {
        while (!_isConnected)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ipAddress, port);
                _stream = _client.GetStream();
                _isConnected = true;
                Debug.Log("Connected to server");

                // ���� ���� �� �÷��̾� ĳ���� ����
                CreatePlayer();

                ReceiveData(); // �����κ��� ������ ���� ����
            }
            catch (Exception e)
            {
                await Task.Delay(5000); // ���� ���� �� 5�� �� ��õ�
            }
        }
    }

    private async void Heartbeat()
    {
        float term = 1f;
        float lastSendTime = 0f;
        string message = "ping";

        while (_isConnected && _stream != null && (lastSendTime == 0f || lastRecvTime - lastSendTime > term))
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            try
            {
                await _stream.WriteAsync(data, 0, data.Length);
                lastSendTime = Time.time;
            }
            catch (Exception e)
            {
                _isConnected = false;
            }
        }
    }

    private async void ReceiveData()
    {
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            while (_isConnected)
            {
                bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    lastRecvTime = Time.time;
                    ProcessMessage(receivedMessage);
                }
                else
                {
                    HandleDisconnect();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving data: " + e.Message);
            HandleDisconnect();
        }
    }

    private void ProcessMessage(string message)
    {
        // �޽��� ����: CREATE_PLAYER:playerId:x:y
        string[] parts = message.Split(':');
        if (parts.Length == 4 && parts[0] == "CREATE_PLAYER")
        {
            string playerId = parts[1];
            float x = float.Parse(parts[2]);
            float y = float.Parse(parts[3]);

            _playerManager.AddOrUpdatePlayer(playerId, new Vector2(x, y));
        }
    }

    private void CreatePlayer()
    {
        // ���÷� ���� ID �� �ʱ� ��ġ ����
        string playerId = Guid.NewGuid().ToString(); // ������ �÷��̾� ID ����
        Vector2 initialPosition = Vector2.zero; // �ʱ� ��ġ ����

        _playerManager.AddOrUpdatePlayer(playerId, initialPosition);

        // ������ �÷��̾� ���� ���� ���� (�ɼ�)
        string message = $"PLAYER_CREATED:{playerId}:{initialPosition.x}:{initialPosition.y}";
        SendData(message);
    }

    private void HandleDisconnect()
    {
        if (_isConnected)
        {
            _isConnected = false;
            _client.Close();
            Debug.Log("Disconnected from server");
        }
    }

    public async void SendData(string message)
    {
        if (_isConnected && _stream != null)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            try
            {
                await _stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to send data: " + e.Message);
                HandleDisconnect();
            }
        }
    }

    private void OnApplicationQuit()
    {
        // ���ø����̼� ���� �� ���� ���� ó��
        HandleDisconnect();
    }

    public bool IsConnected()
    {
        return _isConnected;
    }
}
