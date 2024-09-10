using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public enum SceneType
{
    Menu,
    Game
}

public enum PacketId
{
    PKT_S_SERVER_CHAT = 9999,
    PKT_S_CREATE_ROOM = 9998,

    PKT_S_ENV = 9000,

    PKT_S_DISCONNECT = 999,

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

    PKT_C_SHOT = 1012,
    PKT_S_SHOT = 1013,

    PKT_C_HIT = 1014,
    PKT_S_HIT = 1015,

    PKT_C_EAT_ROOM_ITEM = 1016,
    PKT_S_EAT_ROOM_ITEM = 1017,

    PKT_C_USE_ITEM = 1018,
    PKT_S_USE_ITEM = 1019,

    PKT_C_CREATE_PARTY = 1020,
    PKT_S_CREATE_PARTY = 1021,

    PKT_C_JOIN_PARTY = 1022,
    PKT_S_JOIN_PARTY = 1023,

    PKT_C_WITHDRAW_PARTY = 1024,
    PKT_S_WITHDRAW_PARTY = 1025,

    PKT_C_MY_PARTY = 1026,
    PKT_S_MY_PARTY = 1027,

    PKT_C_ALL_PARTY = 1028,
    PKT_S_ALL_PARTY = 1029,
};

public class ClientManager : Singleton<ClientManager>
{
    private Socket _clientSocket;
    private bool _isConnected = false;
    public string serverIP = "127.0.0.1"; // 서버 IP 주소
    public int serverPort = 7777;        // 서버 포트 번호
    private float lastSendTime = 0f;
    private float lastRecvTime = 0;

    [Header("Scene Type")]
    [SerializeField] private SceneType _sceneType;
    [SerializeField] private MainMenuManager _menuManager;
    [SerializeField] private PlayerManager _playerManager;

    private Task _receiveTask;

    async void Start()
    {
        _menuManager = MainMenuManager.Instance.GetComponent<MainMenuManager>();
        _playerManager = PlayerManager.Instance.GetComponent<PlayerManager>();

        await ConnectToServerAsync(serverIP, serverPort);

        // 비동기 데이터 수신 루프 시작
        _receiveTask = Task.Run(() => ReceivePacketLoopAsync());
    }

    private void Update()
    {
        // 클라이언트 상태 확인 및 핸드쉐이크 처리
        CheckSocket();
    }

    private async Task ReceivePacketLoopAsync()
    {
        while (_isConnected)
        {
            try
            {
                await ReceivePacketAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception in ReceivePacketLoopAsync: {e}");
                HandleDisconnect();
            }
            await Task.Delay(10); // 짧은 대기 후 재시도
        }
    }

    public async void CheckSocket()
    {
        await HeartbeatAsync();
    }

    public async Task ConnectToServerAsync(string ipAddress, int port)
    {
        try
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await Task.Factory.FromAsync(_clientSocket.BeginConnect, _clientSocket.EndConnect, ipAddress, port, null);
            _isConnected = true;
            Debug.Log("Connected to server");
        }
        catch (Exception e)
        {
            AlertManager.Instance.AlertPopup("서버 연결에 실패했습니다.");
            await Task.Delay(5000); // 연결 실패 시 5초 후 재시도
            await ConnectToServerAsync(ipAddress, port);
        }
    }

    public async Task HeartbeatAsync()
    {
        float term = 1f;

        if (_isConnected && _clientSocket.Poll(0, SelectMode.SelectWrite) && (lastSendTime == 0f || lastRecvTime - lastSendTime > term))
        {
            C_CHAT pkt = new C_CHAT()
            {
                Msg = "Ping"
            };

            try
            {
                await SendPacket(PacketId.PKT_C_TEST, pkt.ToByteArray());
                lastSendTime = Time.time;
            }
            catch (Exception e)
            {
                AlertManager.Instance.AlertPopup("서버와 연결이 끊어졌습니다.");
                HandleDisconnect();
            }
        }
    }

    //public async Task ReceivePacketAsync()
    //{
    //    try
    //    {
    //        byte[] headerBuffer = new byte[4];
    //        int headerBytesRead = await Task.Factory.FromAsync(
    //            (callback, state) => _clientSocket.BeginReceive(headerBuffer, 0, headerBuffer.Length, SocketFlags.None, callback, state),
    //            ar => _clientSocket.EndReceive(ar),
    //            null
    //        );

    //        if (headerBytesRead != headerBuffer.Length)
    //        {
    //            Debug.LogWarning("Header incomplete.");
    //            return;
    //        }

    //        ushort size = BitConverter.ToUInt16(headerBuffer, 0);
    //        ushort id = BitConverter.ToUInt16(headerBuffer, 2);

    //        byte[] dataBuffer = new byte[size - 4];
    //        int dataBytesRead = await Task.Factory.FromAsync(
    //            (callback, state) => _clientSocket.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, callback, state),
    //            ar => _clientSocket.EndReceive(ar),
    //            null
    //        );

    //        if (dataBytesRead == dataBuffer.Length)
    //        {
    //            Debug.Log($"Received Packet ID: {id}, Size: {size}");
    //            ServerPacketHandler.Instance.ProcessReceivedPacket((PacketId)id, dataBuffer);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Data incomplete.");
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError($"Exception in ReceivePacketAsync: {e}");
    //        HandleDisconnect();
    //    }
    //}


    public async Task ReceivePacketAsync()
    {
        try
        {
            byte[] headerBuffer = new byte[4];
            int headerBytesRead = await Task.Factory.FromAsync(
                (callback, state) => _clientSocket.BeginReceive(headerBuffer, 0, headerBuffer.Length, SocketFlags.None, callback, state),
                ar => _clientSocket.EndReceive(ar),
                null
            );

            if (headerBytesRead != headerBuffer.Length)
            {
                Debug.LogWarning("Header incomplete.");
                return;
            }

            ushort size = BitConverter.ToUInt16(headerBuffer, 0);
            ushort id = BitConverter.ToUInt16(headerBuffer, 2);

            byte[] dataBuffer = new byte[size - 4];
            int dataBytesRead = await Task.Factory.FromAsync(
                (callback, state) => _clientSocket.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, callback, state),
                ar => _clientSocket.EndReceive(ar),
                null
            );

            if (dataBytesRead == dataBuffer.Length)
            {
                MainThreadDispatcher.ExecuteOnMainThread(() =>
                {
                    Debug.Log($"Received Packet ID: {id}, Size: {size}");
                    ServerPacketHandler.Instance.ProcessReceivedPacket((PacketId)id, dataBuffer);
                });
            }
            else
            {
                Debug.LogWarning("Data incomplete.");
            }
        }
        catch (Exception e)
        {
            MainThreadDispatcher.ExecuteOnMainThread(() =>
            {
                Debug.LogError($"Exception in ReceivePacketAsync: {e}");
                HandleDisconnect();
            });
        }
    }



    public async Task SendPacket(PacketId packetId, byte[] protobufData)
    {
        if (_isConnected)
        {
            try
            {
                ushort id = (ushort)packetId;
                ushort size = (ushort)(protobufData.Length + 4);

                byte[] header = new byte[4];
                header[0] = (byte)(size & 0xFF);
                header[1] = (byte)(size >> 8);
                header[2] = (byte)(id & 0xFF);
                header[3] = (byte)(id >> 8);

                byte[] packet = new byte[header.Length + protobufData.Length];
                Buffer.BlockCopy(header, 0, packet, 0, header.Length);
                Buffer.BlockCopy(protobufData, 0, packet, header.Length, protobufData.Length);

                await Task.Factory.FromAsync(
                    (callback, state) => _clientSocket.BeginSend(packet, 0, packet.Length, SocketFlags.None, callback, state),
                    ar => _clientSocket.EndSend(ar),
                    null
                );
            }
            catch (Exception e)
            {
                AlertManager.Instance.AlertPopup("데이터 송신 실패");
                HandleDisconnect();
            }
        }
    }

    private void OnApplicationQuit()
    {
        HandleDisconnect();
    }

    private void HandleDisconnect()
    {
        if (_isConnected)
        {
            _isConnected = false;
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
            Debug.Log("Disconnected from server");
        }
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
