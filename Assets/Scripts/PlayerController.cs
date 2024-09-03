using UnityEngine;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private ulong playerId;
    [SerializeField] private string playerName;

    private ClientManager _networkManager;
    private Player _player;
    private PlayerManager _playerManager;
    private Vector2 _lastPosition;
    private float _timeSinceLastSend = 0f;
    private float _sendInterval = 0.25f;

    void Start()
    {
        _networkManager = FindObjectOfType<ClientManager>();
        _player = GetComponent<Player>();
        _playerManager = FindObjectOfType<PlayerManager>();
        _lastPosition = _player.GetCurrentPosition();
    }

    void Update()
    {
        _timeSinceLastSend += Time.deltaTime;

        Vector2 currentPosition = _player.GetCurrentPosition();
        if (Vector2.Distance(_lastPosition, currentPosition) > 0.1f && _timeSinceLastSend >= _sendInterval)
        {
            string message = $"PlayerID:{currentPosition.x}:{currentPosition.y}";
            //_networkManager.SendData(message);
            _lastPosition = currentPosition;
            _timeSinceLastSend = 0f;
        }
    }

    public void ProcessPositionUpdate(string message)
    {
        string[] parts = message.Split(':');
        if (parts.Length == 4)
        {
            string playerId = parts[0];
            float x = float.Parse(parts[1]);
            float y = float.Parse(parts[2]);

            //_playerManager.AddOrUpdatePlayer(playerId, new Vector2(x, y));
        }
    }

    public void SetPlayerId(ulong _playerId)
    {
        playerId = _playerId;
    }

    public void SetPlayerName(string _playerName)
    {
        playerName = _playerName;
    }

    public ulong GetPlayerId()
    {
        return playerId;
    }

    public string GetPlayerName()
    {
        return playerName;
    }
}
