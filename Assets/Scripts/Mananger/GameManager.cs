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
            ProcessLogin();
        }
    }

    public void ProcessLogin()
    {

        isStart = true;
        CreatePlayer();
    }
    private void CreatePlayer()
    {
        ulong playerId = loginPkt.Player.Id;
        Vector2 initialPosition = new Vector2(loginPkt.Player.PosX, loginPkt.Player.PosY);

        //TODO Player HP Set

        Debug.Log(SceneManager.GetActiveScene().name);
        PlayerManager.Instance.AddOrUpdatePlayer(playerId.ToString(), initialPosition);

        // 서버에 플레이어 생성 정보 전송 (옵션)
        string message = $"PLAYER_CREATED:{playerId}:{initialPosition.x}:{initialPosition.y}";
        EnterGame(playerId);
    }

    void EnterGame(ulong playerId)
    {
        C_ENTER_GAME enterPkt = new C_ENTER_GAME()
        {
            PlayerId = playerId
        };

        byte[] data = enterPkt.ToByteArray();
        ClientManager.Instance.SendPacket(PacketId.PKT_C_ENTER_GAME, data);
    }
}
