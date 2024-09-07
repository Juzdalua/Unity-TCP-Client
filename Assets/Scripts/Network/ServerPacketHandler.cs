using Google.Protobuf.Protocol;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerPacketHandler : Singleton<ServerPacketHandler>
{
    public void ProcessReceivedPacket(PacketId id, byte[] data)
    {
        switch (id)
        {
            default:
                AlertManager.Instance.AlertPopup("잘못된 정보");
                Debug.Log($"Unknown packet id: {id}");
                break;

            case PacketId.PKT_S_SERVER_CHAT:
                S_SERVER_CHAT serverChatPkt = S_SERVER_CHAT.Parser.ParseFrom(data);

                string message = serverChatPkt.Msg.ToString();
                //string message1 = serverChatPkt.Msg.ToStringUtf8();
                //string message2 = Encoding.UTF8.GetString(serverChatPkt.Msg.ToByteArray());
                
                Debug.Log(message);
                //Debug.Log(message1);
                //Debug.Log(message2);

                S_CHAT toChatPkt = new S_CHAT()
                {
                    Type = serverChatPkt.Type,
                    Msg = message
                };

                ChattingManager.Instance.ProcessChatFromServer(toChatPkt);
                //if (movePkt.Player != null)
                //{
                //    PlayerManager.Instance.MoveUpdatePlayer(movePkt.Player);
                //}
                //else
                //{
                //    //AlertManager.Instance.AlertPopup(chatPkt.Error.ErrorMsg);
                //    Debug.Log($"Error Code: CHAT");
                //}
                break;

            case PacketId.PKT_S_CREATE_ROOM:
                S_CREATE_ROOM room = S_CREATE_ROOM.Parser.ParseFrom(data);
                if(room.Item != null)
                {
                    RoomManager.Instance.CreateRoom(room);
                }
                else
                {
                    //AlertManager.Instance.AlertPopup(signup.Error.ErrorMsg);
                    Debug.Log($"Error Code: Create Room");
                }
                break;

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
                    PlayerManager.Instance.SetPlayerName(login.Player.Name);

                    //Scene 전환
                    ClientManager.Instance.SetSceneType(SceneType.Game);
                    MainMenuManager.Instance.ActiveMenu();
                    SceneManager.LoadScene("01.MainScene");
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
                    ChattingManager.Instance.ProcessChatFromServer(chatPkt);
                }
                else
                {
                    //AlertManager.Instance.AlertPopup(chatPkt.Error.ErrorMsg);
                    Debug.Log($"Error Code: CHAT");
                }
                break;

            case PacketId.PKT_S_MOVE:
                S_MOVE movePkt = S_MOVE.Parser.ParseFrom(data);
                //TODO
                if (movePkt.Player != null)
                {
                    PlayerManager.Instance.MoveUpdatePlayer(movePkt.Player);
                }
                else
                {
                    //AlertManager.Instance.AlertPopup(chatPkt.Error.ErrorMsg);
                    Debug.Log($"Error Code: CHAT");
                }
                break;

            case PacketId.PKT_S_SHOT:
                S_SHOT shotPkt = S_SHOT.Parser.ParseFrom(data);
                //TODO
                if (shotPkt.PlayerId != 0)
                {
                    PlayerManager.Instance.ShotUpdate(shotPkt);
                }
                else
                {
                    //AlertManager.Instance.AlertPopup(chatPkt.Error.ErrorMsg);
                    Debug.Log($"Error Code: SHOT");
                }
                break;

            case PacketId.PKT_S_HIT:
                S_HIT hitPkt = S_HIT.Parser.ParseFrom(data);
                //TODO
                if (hitPkt.PlayerId != 0)
                {
                    PlayerManager.Instance.HitBulletUpdate(hitPkt);
                }
                else
                {
                    //AlertManager.Instance.AlertPopup(chatPkt.Error.ErrorMsg);
                    Debug.Log($"Error Code: SHOT");
                }
                break;

            case PacketId.PKT_S_EAT_ROOM_ITEM:
                S_EAT_ROOM_ITEM useItemPkt = S_EAT_ROOM_ITEM.Parser.ParseFrom(data);
                //TODO
                if (useItemPkt.Player != null)
                {
                    PlayerManager.Instance.EatHealPackUpdate(useItemPkt);
                }
                else
                {
                    //AlertManager.Instance.AlertPopup(chatPkt.Error.ErrorMsg);
                    Debug.Log($"Error Code: SHOT");
                }
                break;
        }
    }
}
