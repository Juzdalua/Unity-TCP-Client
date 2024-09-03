using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;

using Google.Protobuf.Protocol;
using System;
using Google.Protobuf;
using Unity.VisualScripting;

public class MainMenuManager : Singleton<MainMenuManager>
{
    [Header("User Info")]
    [SerializeField] private TMP_InputField idInput;
    [SerializeField] private TMP_InputField pwdInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signupButton;

    [Header("Socket Info")]
    [SerializeField] private ClientManager _networkManager;
    [SerializeField] private TextMeshProUGUI connectedTMP;
    [SerializeField] private List<Image> loadImages;

    private void Start()
    {
        AlertManager.Instance.SetAlertPopup();
        _networkManager = ClientManager.Instance.GetComponent<ClientManager>();
    }

    private void Update()
    {
        TabInput();
        EnterInput();
        UpdateConnectedTMPUI();
        AlertManager.Instance.FadeoutAlert();
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
        if (Input.GetKeyDown(KeyCode.Return))
            OnClickButton("login");
    }
    public void OnClickButton(string type)
    {
        string id = idInput.text;
        string pwd = pwdInput.text;

        if (id.Trim() == "" || pwd.Trim() == "")
            return;

        if (id.Split(" ").Length > 1 || pwd.Split(" ").Length > 1)
            return;

        if (!_networkManager.IsConnected())
            return;

        if (type == "login")
            Login(id.Trim(), pwd.Trim());
        else if (type == "signup")
            Signup(id.Trim(), pwd.Trim());
    }

    void Login(string id, string pwd)
    {
        Account account = new Account()
        {
            Id = 0,
            Name = id,
            //Password = pwd,
            Password = CreatedHashPwd(pwd),
        };

        C_LOGIN loginPkt = new C_LOGIN()
        {
            Account = account,
        };
        PlayerManager.Instance.SetPlayerName(id);
        byte[] data = loginPkt.ToByteArray();
        _networkManager.SendPacket(PacketId.PKT_C_LOGIN, data);
    }

    void Signup(string id, string pwd)
    {
        Account account = new Account()
        {
            Id = 0,
            Name = id,
            Password = CreatedHashPwd(pwd),
        };

        C_SIGNUP signupPkt = new C_SIGNUP()
        {
            Account = account,
        };
        byte[] data = signupPkt.ToByteArray();
        _networkManager.SendPacket(PacketId.PKT_C_SIGNUP, data);
    }

    string CreatedHashPwd(string pwd)
    {
        // SHA256 해시 객체 생성
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // 문자열을 바이트 배열로 변환
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(pwd));

            // 바이트 배열을 16진수 문자열로 변환
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
            connectedTMP.text = "서버에 연결되었습니다.";
            loadImages[0].gameObject.SetActive(false);
            loadImages[1].gameObject.SetActive(true);
        }
        else
        {
            connectedTMP.text = "서버에 접속중입니다.";
            loadImages[0].gameObject.SetActive(true);
            loadImages[1].gameObject.SetActive(false);
        }
    }

    public void ActiveMenu()
    {
        if(ClientManager.Instance.GetSceneType() == SceneType.Menu)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
