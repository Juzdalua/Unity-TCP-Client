using Google.Protobuf.Protocol;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ChatType
{
    Normal = 0,
    Party = 1,
    Guild = 2,
    Whisper = 3,
    System = 4,
    Count = 5,
}

public class ChattingManager : Singleton<ChattingManager>
{
    [Header("Input Text")]
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private GameObject textChatPrefab;
    [SerializeField] private Transform parentContent;

    [Header("Text Type")]
    [SerializeField] private Button chatTypeButton;
    [SerializeField] private TextMeshProUGUI textInput;
    private ChatType currentInputType;
    private Color currentTextColor;

    [Header("Toggle")]
    private List<ChatCell> chatList;
    private ChatType currentViewType;

    [Header("Macro")]
    private string lastChatData = "";
    private string lastWhisperId = "";

    [SerializeField] private ScrollRect _scrollRect;

    // TEMP
    private string playerId = "kim";
    private string friend = "ok";

    private void Awake()
    {
        chatList = new List<ChatCell>();

        currentInputType = ChatType.Normal;
        currentTextColor = Color.white;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(_inputField.isFocused == false)
            {
                //_inputField.Select();
                _inputField.ActivateInputField();
            }
            else
            {
                UpdateChat();
            }
        }

        if(Input.GetKeyDown(KeyCode.Tab) && _inputField.isFocused)
        {
            SetCurrentInputType();
        }
    }

    public void OnEndEditEventMethod()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UpdateChat();
        }
    }

    public void UpdateChat()
    {
        if (_inputField.text == "")
        {
            _inputField.DeactivateInputField();
            return;
        }

        UpdateChatWithCommand(_inputField.text);
    }

    public void UpdateChatWithCommand(string chat)
    {
        // 일반 출력
        if (!chat.StartsWith('/'))
        {
            lastChatData = chat;
            //PrintChatData(currentInputType, currentTextColor, lastChatData);
            SendChatToServer(currentInputType, lastChatData);
            return;
        }

        // 마지막 내용 다시 출력
        if (chat.StartsWith("/re"))
        {
            if(lastChatData == "")
            {
                _inputField.text = "";
                return;
            }
            UpdateChatWithCommand(lastChatData);
        }

        // 귓속말
        // [/w][ID][TEXT]
        else if (chat.StartsWith("/w"))
        {
            lastChatData = chat;
            string[] whisper = chat.Split(" ", 3);

            if (whisper[1] == friend)
            {
                lastWhisperId = whisper[1];
                //PrintChatData(ChatType.Whisper, ChatTypeToColor(ChatType.Whisper), $"[to {whisper[1]}] {whisper[2]}");
                SendChatToServer(ChatType.Whisper, $"[to {whisper[1]}] {whisper[2]}");
            }
            else
            {
                //PrintChatData(ChatType.System, ChatTypeToColor(ChatType.System), $"Do not find [{whisper[1]}]");
                SendChatToServer(ChatType.System, $"Do not find [{whisper[1]}]");
            }
        }

        // 마지막 귓말 대상자에게 다시 귓말
        // [/r][TEXT]
        else if (chat.StartsWith("r"))
        {
            if (lastChatData == "")
            {
                _inputField.text = "";
                return;
            }

            string[] whisper = chat.Split(" ", 2);
            //PrintChatData(ChatType.Whisper, ChatTypeToColor(ChatType.Whisper), $"[to {lastWhisperId}] {whisper[1]}");
            SendChatToServer(ChatType.Whisper, $"[to {lastWhisperId}] {whisper[1]}");
        }
    }
    public void SendChatToServer(ChatType type, string text)
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

    public void PrintChatData(ChatType type, Color color, string text)
    {
        GameObject clone = Instantiate(textChatPrefab, parentContent);
        ChatCell cell = clone.GetComponent<ChatCell>();

        cell.Setup(type, color, text);
        _inputField.text = "";
        _inputField.DeactivateInputField();

        chatList.Add(cell);

        Canvas.ForceUpdateCanvases();  // 레이아웃을 강제로 업데이트
        _scrollRect.verticalNormalizedPosition = 0f;
    }

    public Color ChatTypeToColor(ChatType type)
    {
        Color[] colors = new Color[((int)ChatType.Count)]
        {
            Color.white, Color.blue, Color.green, Color.magenta, Color.yellow
        };

        return colors[(int)type];
    }

    public void SetCurrentInputType()
    {
        // TODO
        bool getGuild = false;
        if (getGuild)
            currentInputType = (int)currentInputType < (int)ChatType.Count - 3 ? currentInputType + 1 : 0;
        else
            currentInputType = (int)currentInputType < (int)ChatType.Count - 4 ? currentInputType + 1 : 0;

        // button text change
        chatTypeButton.GetComponentInChildren<TextMeshProUGUI>().text = currentInputType.ToString();

        currentTextColor = ChatTypeToColor(currentInputType);
        textInput.color = currentTextColor == Color.white ? Color.black : currentTextColor;

        Canvas.ForceUpdateCanvases();  // 레이아웃을 강제로 업데이트
        _scrollRect.verticalNormalizedPosition = 0f;
    }

    public void SetCurrentViewType(int newType)
    {
        currentViewType = (ChatType)newType;

        if(currentViewType == ChatType.Normal)
        {
            for(int i=0; i<chatList.Count; i++)
            {
                chatList[i].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < chatList.Count; i++)
            {
                chatList[i].gameObject.SetActive(chatList[i].ChatType == currentViewType);
            }
        }
    }
}