using Google.Protobuf;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ClientPacketHandler : Singleton<ClientPacketHandler>
{
    public async void Signup(string id, string pwd)
    {
        Account account = new Account()
        {
            Id = 0,
            Name = id,
            Password = UtilManager.Instance.CreatedHashPwd(pwd),
        };

        C_SIGNUP signupPkt = new C_SIGNUP()
        {
            Account = account,
        };
        byte[] data = signupPkt.ToByteArray();
        await ClientManager.Instance.SendPacket(PacketId.PKT_C_SIGNUP, data);
    }

    public async void Login(string id, string pwd)
    {
        Account account = new Account()
        {
            Id = 0,
            Name = id,
            //Password = pwd,
            Password = UtilManager.Instance.CreatedHashPwd(pwd),
        };

        C_LOGIN loginPkt = new C_LOGIN()
        {
            Account = account,
        };
        byte[] data = loginPkt.ToByteArray();
        await ClientManager.Instance.SendPacket(PacketId.PKT_C_LOGIN, data);
    }

    public async void EnterGame(ulong playerId)
    {
        C_ENTER_GAME enterPkt = new C_ENTER_GAME()
        {
            PlayerId = playerId
        };

        byte[] data = enterPkt.ToByteArray();
        await ClientManager.Instance.SendPacket(PacketId.PKT_C_ENTER_GAME, data);
    }

    public async void Chat(Google.Protobuf.Protocol.ChatType _type, string text)
    {
        C_CHAT chatPkt = new C_CHAT()
        {
            Type = _type,
            PlayerId = PlayerManager.Instance.GetPlayerId(),
            PlayerName = PlayerManager.Instance.GetPlayerName(),
            Msg = text,
        };

        byte[] data = chatPkt.ToByteArray();
        await ClientManager.Instance.SendPacket(PacketId.PKT_C_CHAT, data);
    }

    public async void Move(ulong playerId, float posX, float posY, MoveDir dir)
    {
        C_MOVE movePkt = new C_MOVE()
        {
            PlayerId = playerId,
            PosX = posX,
            PosY = posY
        };

        byte[] data = movePkt.ToByteArray();
        await ClientManager.Instance.SendPacket(PacketId.PKT_C_MOVE, data);
    }
}
