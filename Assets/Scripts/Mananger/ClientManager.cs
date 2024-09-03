using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public enum SceneType
{
    Menu,
    Game
}

public enum PacketId
{
    PKT_C_TEST = 1000,
    PKT_S_TEST = 1001,

    PKT_C_SIGNUP = 1002,
    PKT_S_SIGNUP = 1003,

    PKT_C_LOGIN = 1004,
    PKT_S_LOGIN = 1005,

    PKT_C_ENTER_GAME = 1006,
    PKT_S_ENTER_GAME = 1007,

    PKT_C_CHAT = 1008,
    PKT_S_CHAT = 1009,

    PKT_C_MOVE = 1010,
    PKT_S_MOVE = 1011,
};

public class PacketMessage
{
    public ushort Id { get; set; }
    public IMessage Message { get; set; }
}


public class ClientManager : Singleton<ClientManager>
{
    [Header("Server Info")]
    private Socket _clientSocket;
    private bool _isConnected = false;
    public string serverIP = "127.0.0.1"; // ���� IP �ּ�
    public int serverPort = 7777;        // ���� ��Ʈ ��ȣ
    private float lastSendTime = 0f;
    private float lastRecvTime = 0;

    [Header("Scene Type")]
    [SerializeField] private SceneType _sceneType;
    [SerializeField] private MainMenuManager _menuManager;
    [SerializeField] private PlayerManager _playerManager;

    void Start()
    {
        _menuManager = MainMenuManager.Instance.GetComponent<MainMenuManager>();
        _playerManager = PlayerManager.Instance.GetComponent<PlayerManager>();


        if (_sceneType == SceneType.Menu)
        {

        }
        else if (_sceneType == SceneType.Game)
        {

        }
    }

    public void CheckSocket(SceneType sceneType)
    {
        if (!_isConnected)
        {
            ConnectToServer(serverIP, serverPort, sceneType);
        }
        else
        {
            Heartbeat();
            ReceivePacket();
        }
    }

    public void ConnectToServer(string ipAddress, int port, SceneType sceneType)
    {
        try
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientSocket.Connect(ipAddress, port);
            _isConnected = true;
            Debug.Log("Connected to server");

            if (sceneType == SceneType.Menu)
            {
            }
            else if (sceneType == SceneType.Game)
            {
            }

        }
        catch (Exception e)
        {
            //Debug.Log("Failed to connect: " + e.Message);
            AlertManager.Instance.AlertPopup("���� ���ῡ �����߽��ϴ�.");
            Task.Delay(5000).ContinueWith(_ => ConnectToServer(ipAddress, port, sceneType)); // ���� ���� �� 5�� �� ��õ�
        }
    }

    private void Heartbeat()
    {
        float term = 1f;

        if (_isConnected && _clientSocket.Poll(0, SelectMode.SelectWrite) && (lastSendTime == 0f || lastRecvTime - lastSendTime > term))
        {
            //byte[] data = Encoding.ASCII.GetBytes(message);
            C_CHAT pkt = new C_CHAT()
            {
                Msg = "Ping"
            };

            try
            {
                //_clientSocket.Send(data);
                SendPacket(PacketId.PKT_C_TEST, pkt.ToByteArray());
                lastSendTime = Time.time;
            }
            catch (Exception e)
            {
                //Debug.Log("Heartbeat failed: " + e.Message);
                AlertManager.Instance.AlertPopup("������ ������ ���������ϴ�.");
                HandleDisconnect();
            }
        }
    }

    public void ReceivePacket()
    {
        try
        {
            while (_clientSocket.Available > 0)
            {
                byte[] buffer = new byte[4096]; // ������ ���� ũ��� �����մϴ�.
                int bytesRead = _clientSocket.Receive(buffer);

                if (bytesRead > 0)
                {
                    // ���ŵ� �������� ���̸� ����Ʈ �迭�� ��ȯ�մϴ�.
                    ArraySegment<byte> segment = new ArraySegment<byte>(buffer, 0, bytesRead);

                    // �����Ͱ� ������� Ȯ���մϴ� (��� 4����Ʈ + ������)
                    if (segment.Count >= 4)
                    {
                        // 1. ��� �м�
                        ushort size = BitConverter.ToUInt16(segment.Array, segment.Offset); // ��Ʋ �����
                        ushort id = BitConverter.ToUInt16(segment.Array, segment.Offset + 2); // ��Ʋ �����

                        // 2. ������ ����
                        int dataSize = size - 4; // ������ ũ�� (��� ũ�� 4����Ʈ�� ������ ũ��)
                        if (segment.Count >= size)
                        {
                            byte[] data = new byte[dataSize];
                            Buffer.BlockCopy(segment.Array, segment.Offset + 4, data, 0, dataSize);
                            Debug.Log($"Received Packet ID: {id}, Size: {size}");

                            // ������ ó��
                            ProcessReceivedPacket((PacketId)id, data);
                        }
                        else
                        {
                            Debug.LogWarning("Received incomplete packet.");
                            // ó������ ���� �����Ͱ� �ִ� ��� ������ ó���� �߰��մϴ�.
                        }
                    }
                    else
                    {
                        AlertManager.Instance.AlertPopup("������ ���� ����.");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(_menuManager);
            AlertManager.Instance.AlertPopup("������ ���� ����.");
            //Debug.LogError("Failed to receive data: " + e.Message);
            HandleDisconnect();
        }
    }

    private void ProcessReceivedPacket(PacketId id, byte[] data)
    {
        switch (id)
        {
            case PacketId.PKT_S_TEST:
                S_CHAT chat = S_CHAT.Parser.ParseFrom(data);
                Debug.Log($"Received chat message: PlayerId={chat.PlayerId}, Msg={chat.Msg}");
                break;

            case PacketId.PKT_S_SIGNUP:
                S_SIGNUP signup = S_SIGNUP.Parser.ParseFrom(data);
                if (signup.Success)
                {
                    Debug.Log($"Signup Success");
                }
                else
                {
                    AlertManager.Instance.AlertPopup(signup.Error.ErrorMsg);
                    Debug.Log($"Error Code: {signup.Error.ErrorCode}");
                }
                break;

            case PacketId.PKT_S_LOGIN:
                S_LOGIN login = S_LOGIN.Parser.ParseFrom(data);
                if (login.Success)
                {
                    Debug.Log($"Login Success: {login.Player.Id}");
                    
                    //Scene ��ȯ
                    SetSceneType(SceneType.Game);
                    MainMenuManager.Instance.ActiveMenu();
                    SceneManager.LoadScene("01.MainScene");
                    //GameManager.Instance.loginPkt = login;
                    GameManager.loginPkt = login;
                }
                else
                {
                    AlertManager.Instance.AlertPopup(login.Error.ErrorMsg);
                    Debug.Log($"Error Code: {login.Error.ErrorCode}");
                }
                break;

            case PacketId.PKT_S_ENTER_GAME:
                S_ENTER_GAME enter = S_ENTER_GAME.Parser.ParseFrom(data);
                if (enter.Success)
                {
                    Debug.Log($"Enter Game Success");
                }
                else
                {
                    //AlertManager.Instance.AlertPopup(enter.Error.ErrorMsg);
                    Debug.Log($"Error Code: EnterGame");
                }
                break;

            case PacketId.PKT_S_CHAT:
                S_CHAT chatPkt = S_CHAT.Parser.ParseFrom(data);
                if (chatPkt.Msg != null || chatPkt.PlayerId != 0)
                {
                    Debug.Log($"CHAT Success");
                }
                else
                {
                    //AlertManager.Instance.AlertPopup(chatPkt.Error.ErrorMsg);
                    Debug.Log($"Error Code: CHAT");
                }
                break;

            default:
                AlertManager.Instance.AlertPopup("�߸��� ����");
                Debug.Log($"Unknown packet id: {id}");
                break;
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

    public void HandleDisconnect()
    {
        if (_isConnected)
        {
            _isConnected = false;
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
            Debug.Log("Disconnected from server");
        }
    }

    public void SendPacket(PacketId packetId, byte[] protobufData)
    {
        if (_isConnected && _clientSocket.Poll(0, SelectMode.SelectWrite))
        {
            try
            {
                // 1. ��� ���� (��Ʋ �����)
                ushort id = (ushort)packetId;
                ushort size = (ushort)(protobufData.Length + 4); // �������� ���� + ��� ����

                byte[] header = new byte[4];

                // size (2����Ʈ) - ��Ʋ ����� ������� ��ȯ
                header[0] = (byte)(size & 0xFF); // ���� ����Ʈ
                header[1] = (byte)(size >> 8);   // ���� ����Ʈ

                // id (2����Ʈ) - ��Ʋ ����� ������� ��ȯ
                header[2] = (byte)(id & 0xFF);   // ���� ����Ʈ
                header[3] = (byte)(id >> 8);     // ���� ����Ʈ

                // 2. ����� ������ ����
                byte[] packet = new byte[header.Length + protobufData.Length];
                Buffer.BlockCopy(header, 0, packet, 0, header.Length);
                Buffer.BlockCopy(protobufData, 0, packet, header.Length, protobufData.Length);

                _clientSocket.Send(packet);
            }
            catch (Exception e)
            {
                AlertManager.Instance.AlertPopup("������ �۽� ����");
                //Debug.Log("Failed to send data: " + e.Message);
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

    public SceneType GetSceneType()
    {
        return _sceneType;
    }

    public void SetSceneType(SceneType type)
    {
        _sceneType = type;
    }
}