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
    [Header("User Info")]
    [SerializeField] private TMP_InputField idInput;
    [SerializeField] private TMP_InputField pwdInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signupButton;

    [Header("Socket Info")]
    [SerializeField] private TextMeshProUGUI connectedTMP;
    [SerializeField] private List<Image> loadImages;

    [Header("UI")]
    [SerializeField] private GameObject alertPopup;
    private float fadeDuration = 2.0f; // ���̵�ƿ� ���� �ð�
    private bool isFadingOut = false;
    private float fadeOutTimer = 0f;

    private ClientManager _networkManager;

    private void Start()
    {
        SetAlertPopup();
        _networkManager = GetComponent<ClientManager>();
    }

    private void Update()
    {
        TabInput();
        EnterInput();
        UpdateConnectedTMPUI();
        FadeoutAlert();

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
    public void AlertPopup(string msg)
    {
        alertPopup.SetActive(true);
        alertPopup.GetComponent<CanvasGroup>().alpha = 1;
        alertPopup.GetComponentInChildren<TextMeshProUGUI>().text = msg;

        // ���̵�ƿ� ����
        isFadingOut = true;
        fadeOutTimer = 0f;
    }

    void FadeoutAlert()
    {
        if (isFadingOut)
        {
            // ��� �ð� ���
            fadeOutTimer += Time.deltaTime;

            // alpha �� ��� (0���� fadeDuration���� �ð��� ������ alpha�� 0���� ����)
            alertPopup.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1, 0, fadeOutTimer / fadeDuration);

            // ���̵�ƿ� �Ϸ� ��
            if (fadeOutTimer >= fadeDuration)
            {
                isFadingOut = false;
                alertPopup.SetActive(false); // �˾��� ��Ȱ��ȭ
            }
        }
    }

    void SetAlertPopup()
    {
        alertPopup.SetActive(false);
        alertPopup.GetComponent<CanvasGroup>().alpha = 0;
        alertPopup.GetComponentInChildren<TextMeshProUGUI>().text = "";
    }
}
