using Google.Protobuf;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPacketHandler : Singleton<ClientPacketHandler>
{
    public void Signup(string id, string pwd)
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
        ClientManager.Instance.SendPacket(PacketId.PKT_C_SIGNUP, data);
    }

    public void Login(string id, string pwd)
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
        PlayerManager.Instance.SetPlayerName(id);
        byte[] data = loginPkt.ToByteArray();
        ClientManager.Instance.SendPacket(PacketId.PKT_C_LOGIN, data);
    }

    public void EnterGame(ulong playerId)
    {
        C_ENTER_GAME enterPkt = new C_ENTER_GAME()
        {
            PlayerId = playerId
        };

        byte[] data = enterPkt.ToByteArray();
        ClientManager.Instance.SendPacket(PacketId.PKT_C_ENTER_GAME, data);
    }

    public void Chat(Google.Protobuf.Protocol.ChatType _type, string text)
    {
        C_CHAT chatPkt = new C_CHAT()
        {
            Type = _type,
            PlayerId = PlayerManager.Instance.GetPlayerId(),
            PlayerName = PlayerManager.Instance.GetPlayerName(),
            Msg = text,
        };

        byte[] data = chatPkt.ToByteArray();
        ClientManager.Instance.SendPacket(PacketId.PKT_C_CHAT, data);
    }
}