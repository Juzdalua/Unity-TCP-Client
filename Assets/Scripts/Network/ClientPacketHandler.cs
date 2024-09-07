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
        PacketId packetId = PacketId.PKT_C_SIGNUP;
        byte[] data = signupPkt.ToByteArray();
        await ClientManager.Instance.SendPacket(packetId, data);
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
        PacketId packetId = PacketId.PKT_C_LOGIN;
        byte[] data = loginPkt.ToByteArray();
        await ClientManager.Instance.SendPacket(packetId, data);
    }

    public async void EnterGame(ulong playerId)
    {
        C_ENTER_GAME enterPkt = new C_ENTER_GAME()
        {
            PlayerId = playerId
        };
        PacketId packetId = PacketId.PKT_C_ENTER_GAME;
        byte[] data = enterPkt.ToByteArray();
        await ClientManager.Instance.SendPacket(packetId, data);
    }

    public async void Chat(Google.Protobuf.Protocol.ChatType _type, string text)
    {
        C_CHAT chatPkt = new C_CHAT()
        {
            Type = _type,
            PlayerId = PlayerManager.Instance.GetMyPlayerId(),
            PlayerName = PlayerManager.Instance.GetMyPlayerName(),
            Msg = text,
        };
        PacketId packetId = PacketId.PKT_C_CHAT;
        byte[] data = chatPkt.ToByteArray();
        await ClientManager.Instance.SendPacket(packetId, data);
    }

    public async void Move(ulong playerId, float posX, float posY, MoveDir dir)
    {
        C_MOVE movePkt = new C_MOVE()
        {
            PlayerId = playerId,
            PosX = posX,
            PosY = posY
        };
        PacketId packetId = PacketId.PKT_C_MOVE;
        byte[] data = movePkt.ToByteArray();
        await ClientManager.Instance.SendPacket(packetId, data);
    }

    public async void Shot(ulong playerId, Vector2 spawnPos, Vector3 target)
    {
        C_SHOT shotPkt = new C_SHOT()
        {
            PlayerId = playerId,
            SpawnPosX = spawnPos.x,
            SpawnPosY = spawnPos.y,
            TargetPosX = target.x,
            TargetPosY = target.y
        };
        PacketId packetId = PacketId.PKT_C_SHOT;
        byte[] data = shotPkt.ToByteArray();
        await ClientManager.Instance.SendPacket(packetId, data);
    }

    public async void HitBullet(ulong playerId, ulong damage)
    {
        C_HIT hitPkt = new C_HIT()
        {
            PlayerId = playerId,
            Damage = damage
        };
        PacketId packetId = PacketId.PKT_C_HIT;
        byte[] data = hitPkt.ToByteArray();
        await ClientManager.Instance.SendPacket(packetId, data);
    }

    public async void EatHealPack(ulong playerId, RoomItem healPackInfo)
    {
        C_EAT_ROOM_ITEM earRoomItemPkt = new C_EAT_ROOM_ITEM()
        {
            PlayerId = playerId,
            Item = healPackInfo
        };
        PacketId packetId = PacketId.PKT_C_EAT_ROOM_ITEM;
        byte[] data = earRoomItemPkt.ToByteArray();
        await ClientManager.Instance.SendPacket(packetId, data);
    }
}
