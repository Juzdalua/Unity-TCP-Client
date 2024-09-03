using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerPacketHandler : Singleton<ServerPacketHandler>
{
    public void ProcessReceivedPacket(PacketId id, byte[] data)
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
                    PlayerManager.Instance.SetPlayerId(login.Player.Id);

                    //Scene 전환
                    ClientManager.Instance.SetSceneType(SceneType.Game);
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
                    PlayerManager.Instance.AddOrUpdatePlayer(enter);
                }
                else
                {
                    //AlertManager.Instance.AlertPopup(enter.Error.ErrorMsg);
                    Debug.Log($"Error Code: EnterGame");
                }
                break;

            case PacketId.PKT_S_CHAT:
                S_CHAT chatPkt = S_CHAT.Parser.ParseFrom(data);
                if (chatPkt.Msg != null)
                {
                    GameManager.Instance.ProcessChatFromServer(chatPkt);
                }
                else
                {
                    //AlertManager.Instance.AlertPopup(chatPkt.Error.ErrorMsg);
                    Debug.Log($"Error Code: CHAT");
                }
                break;

            default:
                AlertManager.Instance.AlertPopup("잘못된 정보");
                Debug.Log($"Unknown packet id: {id}");
                break;
        }
    }
}
