using Google.Protobuf;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : Singleton<GameManager>
{
    private bool isStart = false;
    public static S_LOGIN loginPkt;

    private void Update()
    {    
        if(SceneManager.GetActiveScene().name == "01.MainScene" && !isStart && loginPkt != null)
        {
            ProcessLoginToServer();
        }
    }

    public void ProcessLoginToServer()
    {

        isStart = true;
        PlayerManager.Instance.CreatePlayer(loginPkt);
    }
    
    public void ProcessChatToServer(ChatType type, string text)
    {
        Google.Protobuf.Protocol.ChatType _type;
        switch (type)
        {
            default:
            case ChatType.Normal:
                _type = Google.Protobuf.Protocol.ChatType.Normal;
                break;

            case ChatType.Party:
                _type = Google.Protobuf.Protocol.ChatType.Party;
                break;

            case ChatType.Guild:
                _type = Google.Protobuf.Protocol.ChatType.Guild;
                break;

            case ChatType.Whisper:
                _type = Google.Protobuf.Protocol.ChatType.Whisper;
                break;

            case ChatType.System:
                _type = Google.Protobuf.Protocol.ChatType.System;
                break;
        }

        ClientPacketHandler.Instance.Chat(_type, text);
    }

    public void ProcessChatFromServer(S_CHAT chatPkt)
    {
        Debug.Log($"{chatPkt.Type} / {chatPkt.PlayerName} / {chatPkt.Msg}");
        switch (chatPkt.Type)
        {
            default:
            case Google.Protobuf.Protocol.ChatType.Normal:
                ChattingManager.Instance.PrintChatData(
                    ChatType.Normal, 
                    ChattingManager.Instance.ChatTypeToColor(ChatType.Normal), 
                    $"{chatPkt.PlayerName}: {chatPkt.Msg}"
                );
                break;

            case Google.Protobuf.Protocol.ChatType.Party:
                ChattingManager.Instance.PrintChatData(
                    ChatType.Party,
                    ChattingManager.Instance.ChatTypeToColor(ChatType.Party),
                    $"{chatPkt.PlayerName}: {chatPkt.Msg}"
                );
                break;

            case Google.Protobuf.Protocol.ChatType.Guild:
                ChattingManager.Instance.PrintChatData(
                    ChatType.Guild,
                    ChattingManager.Instance.ChatTypeToColor(ChatType.Guild),
                    $"{chatPkt.PlayerName}: {chatPkt.Msg}"
                );
                break;

            case Google.Protobuf.Protocol.ChatType.Whisper:
                ChattingManager.Instance.PrintChatData(
                    ChatType.Whisper,
                    ChattingManager.Instance.ChatTypeToColor(ChatType.Whisper),
                    $"{chatPkt.PlayerName}: {chatPkt.Msg}"
                );
                break;

            case Google.Protobuf.Protocol.ChatType.System:
                ChattingManager.Instance.PrintChatData(
                    ChatType.System,
                    ChattingManager.Instance.ChatTypeToColor(ChatType.System),
                    $"{chatPkt.Msg}"
                );
                break;
        }
    }
}
