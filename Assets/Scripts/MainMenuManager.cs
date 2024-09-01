using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;

using Google.Protobuf.Protocol;
using System;
using Google.Protobuf;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField idInput;
    [SerializeField] private TMP_InputField pwdInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signupButton;
    [SerializeField] private TextMeshProUGUI connectedTMP;
    [SerializeField] private List<Image> loadImages;

    private ClientManager _networkManager;

    private void Start()
    {
        _networkManager = GetComponent<ClientManager>();
    }

    private void Update()
    {
        TabInput();
        EnterInput();
        UpdateConnectedTMPUI();

        CheckSocket();
    }

    void TabInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (idInput.isFocused)
                pwdInput.Select();
            else
                idInput.Select();
        }
    }

    void EnterInput()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            OnClickButton("login");
    }
    public void OnClickButton(string type)
    {
        string id = idInput.text;
        string pwd = pwdInput.text;

        if (id.Trim() == "" || pwd.Trim() == "")
            return;

        if (!_networkManager.IsConnected())
            return;

        if (type == "login")
            Login(id, pwd);
        else if (type == "signup")
            Signup(id, pwd);
    }

    void Login(string id, string pwd)
    {
        Debug.Log("START Login");

        Account account = new Account()
        {
            Id = ulong.Parse(id),
            Password = CreatedHashPwd(pwd),
        };

        C_LOGIN loginPkt = new C_LOGIN()
        {
            Type = LoginType.Login,
            Account = account,
        };
        byte[] data = loginPkt.ToByteArray();
        _networkManager.SendPacket(PacketId.PKT_C_LOGIN, data);
    }

    void Signup(string id, string pwd)
    {

    }

    void CheckSocket()
    {
        if (_networkManager != null)
        {
            _networkManager.CheckSocket(SceneType.Menu);
        }
    }

    string CreatedHashPwd(string pwd)
    {
        // SHA256 �ؽ� ��ü ����
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ���ڿ��� ����Ʈ �迭�� ��ȯ
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(pwd));

            // ����Ʈ �迭�� 16���� ���ڿ��� ��ȯ
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
    void UpdateConnectedTMPUI()
    {
        if (_networkManager.IsConnected())
        {
            connectedTMP.text = "������ ����Ǿ����ϴ�.";
            loadImages[0].gameObject.SetActive(false);
            loadImages[1].gameObject.SetActive(true);
        }
        else
        {
            connectedTMP.text = "������ �������Դϴ�.";
            loadImages[0].gameObject.SetActive(true);
            loadImages[1].gameObject.SetActive(false);
        }
    }

    byte[] protobufTestCreateAndSerialize()
    {
        S_CHAT chat = new S_CHAT
        {
            PlayerId = 1,
            Msg = "Hi",
        };

        byte[] data = chat.ToByteArray();
        Debug.Log($"Serialized Player Data: {BitConverter.ToString(data)}");
        return data;
    }

    void protobufTestDeserializeAndUse(byte[] data)
    {
        S_CHAT chat = S_CHAT.Parser.ParseFrom(data);

        Debug.Log($"Player ID: {chat.PlayerId}");
        Debug.Log($"Message: {chat.Msg})");
    }

}
