using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

public enum SceneType
{
    Menu,
    Game
}

public enum PacketId
{
    PKT_C_TEST = 0001,
    PKT_S_TEST = 0002,
    PKT_C_LOGIN = 1000,
    PKT_S_LOGIN = 1001,
    PKT_C_ENTER_GAME = 1002,
    PKT_S_ENTER_GAME = 1003,
    PKT_C_CHAT = 1004,
    PKT_S_CHAT = 1005,
};

public class PacketMessage
{
    public ushort Id { get; set; }
    public IMessage Message { get; set; }
}


public class ClientManager : MonoBehaviour
{
    [Header("Server Info")]
    private Socket _clientSocket;
    private bool _isConnected = false;
    public string serverIP = "127.0.0.1"; // 서버 IP 주소
    public int serverPort = 7777;        // 서버 포트 번호
    private float lastSendTime = 0f;
    private float lastRecvTime = 0;

    [Header("Scene Type")]
    [SerializeField] SceneType _sceneType;
    private MainMenuManager _menuManager;
    private PlayerManager _playerManager;

    void Start()
    {
        if (_sceneType == SceneType.Menu)
            _menuManager = GetComponent<MainMenuManager>();
        else if (_sceneType == SceneType.Game)
            _playerManager = GetComponent<PlayerManager>();
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
                // 서버 연결 후 플레이어 캐릭터 생성
                CreatePlayer();
            }

        }
        catch (Exception e)
        {
            Debug.Log("Failed to connect: " + e.Message);
            Task.Delay(5000).ContinueWith(_ => ConnectToServer(ipAddress, port, sceneType)); // 연결 실패 시 5초 후 재시도
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
                Debug.Log("Heartbeat failed: " + e.Message);
                HandleDisconnect();
            }
        }
    }

    public void ReceiveData()
    {
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            if (_isConnected && _clientSocket.Poll(0, SelectMode.SelectRead))
            {
                bytesRead = _clientSocket.Receive(buffer);
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
            Debug.Log("Error receiving data: " + e.Message);
            HandleDisconnect();
        }
    }

    public void ReceivePacket()
    {
        try
        {
            while (_clientSocket.Available > 0)
            {
                byte[] buffer = new byte[4096]; // 적절한 버퍼 크기로 설정합니다.
                int bytesRead = _clientSocket.Receive(buffer);

                if (bytesRead > 0)
                {
                    // 수신된 데이터의 길이를 바이트 배열로 변환합니다.
                    ArraySegment<byte> segment = new ArraySegment<byte>(buffer, 0, bytesRead);

                    // 데이터가 충분한지 확인합니다 (헤더 4바이트 + 데이터)
                    if (segment.Count >= 4)
                    {
                        // 1. 헤더 분석
                        ushort size = BitConverter.ToUInt16(segment.Array, segment.Offset); // 리틀 엔디안
                        ushort id = BitConverter.ToUInt16(segment.Array, segment.Offset + 2); // 리틀 엔디안

                        // 2. 데이터 추출
                        int dataSize = size - 4; // 데이터 크기 (헤더 크기 4바이트를 제외한 크기)
                        if (segment.Count >= size)
                        {
                            byte[] data = new byte[dataSize];
                            Buffer.BlockCopy(segment.Array, segment.Offset + 4, data, 0, dataSize);
                            Debug.Log($"Received Packet ID: {id}, Size: {size}");

                            // 데이터 처리
                            ProcessReceivedPacket((PacketId)id, data);
                        }
                        else
                        {
                            Debug.LogWarning("Received incomplete packet.");
                            // 처리되지 않은 데이터가 있는 경우 적절한 처리를 추가합니다.
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Received data is too short to contain a header.");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to receive data: " + e.Message);
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

            // 다른 패킷 타입도 여기에 추가할 수 있습니다.
            // case PacketId.PKT_S_LOGIN:
            //     S_LOGIN login = S_LOGIN.Parser.ParseFrom(data);
            //     Debug.Log($"Received login response: ...");
            //     break;

            default:
                Debug.Log($"Unknown packet id: {id}");
                break;
        }
    }

    private void ProcessMessage(string message)
    {
        // 메시지 형식: CREATE_PLAYER:playerId:x:y
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
        // 예시로 고유 ID 및 초기 위치 설정
        string playerId = Guid.NewGuid().ToString(); // 고유한 플레이어 ID 생성
        Vector2 initialPosition = Vector2.zero; // 초기 위치 설정

        _playerManager.AddOrUpdatePlayer(playerId, initialPosition);

        // 서버에 플레이어 생성 정보 전송 (옵션)
        string message = $"PLAYER_CREATED:{playerId}:{initialPosition.x}:{initialPosition.y}";
        SendData(message);
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

    public void SendData(string message)
    {
        if (_isConnected && _clientSocket.Poll(0, SelectMode.SelectWrite))
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            try
            {
                _clientSocket.Send(data);
            }
            catch (Exception e)
            {
                Debug.Log("Failed to send data: " + e.Message);
                HandleDisconnect();
            }
        }
    }
    public void SendPacket(PacketId packetId, byte[] protobufData)
    {
        Debug.Log("START SendPacket");

        if (_isConnected && _clientSocket.Poll(0, SelectMode.SelectWrite))
        {
            try
            {
                // 1. 헤더 생성 (리틀 엔디안)
                ushort id = (ushort)packetId;
                ushort size = (ushort)(protobufData.Length + 4); // 데이터의 길이 + 헤더 길이

                byte[] header = new byte[4];

                // size (2바이트) - 리틀 엔디안 방식으로 변환
                header[0] = (byte)(size & 0xFF); // 하위 바이트
                header[1] = (byte)(size >> 8);   // 상위 바이트

                // id (2바이트) - 리틀 엔디안 방식으로 변환
                header[2] = (byte)(id & 0xFF);   // 하위 바이트
                header[3] = (byte)(id >> 8);     // 상위 바이트

                // 2. 헤더와 데이터 결합
                byte[] packet = new byte[header.Length + protobufData.Length];
                Buffer.BlockCopy(header, 0, packet, 0, header.Length);
                Buffer.BlockCopy(protobufData, 0, packet, header.Length, protobufData.Length);

                // 디버깅 로그 출력
                Debug.Log("Sending Packet Length: " + packet.Length);

                _clientSocket.Send(packet);
                Debug.Log("Success SendPacket");
            }
            catch (Exception e)
            {
                Debug.Log("Failed to send data: " + e.Message);
                HandleDisconnect();
            }
        }
    }


    private void OnApplicationQuit()
    {
        // 애플리케이션 종료 시 연결 종료 처리
        HandleDisconnect();
    }

    public bool IsConnected()
    {
        return _isConnected;
    }
}
