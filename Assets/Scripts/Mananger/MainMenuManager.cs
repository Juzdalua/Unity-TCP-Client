using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        {
            string hashPwd = UtilManager.Instance.CreatedHashPwd(pwd);
            //ClientPacketHandler.Instance.Login(id.Trim(), hashPwd.Trim());
            APIManager.Instance.Signin(id.Trim(), hashPwd.Trim());
        }
        else if (type == "signup")
        {
            string hashPwd = UtilManager.Instance.CreatedHashPwd(pwd);
            //ClientPacketHandler.Instance.Signup(id.Trim(), hashPwd.Trim());
            APIManager.Instance.Signup(id.Trim(), hashPwd.Trim());
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
