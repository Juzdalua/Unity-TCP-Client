using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf.Protocol;

public class PlayerManager : Singleton<PlayerManager>
{
    //public List<GameObject> playerPrefabs; 
    public GameObject playerPrefab;

    private Dictionary<ulong, GameObject> _players = new Dictionary<ulong, GameObject>();

    [SerializeField] ulong _playerId = 0;
    [SerializeField] string _playerName = "";

    public void CreatePlayer(S_LOGIN pkt)
    {
        ulong playerId = pkt.Player.Id;

        //TODO Player HP Set

        GameObject prefab = playerPrefab;
        GameObject player = Instantiate(prefab, new Vector2(pkt.Player.PosX, pkt.Player.PosY), Quaternion.identity);
        player.name = pkt.Player.Id.ToString();
        _players[pkt.Player.Id] = player;

        GameManager.Instance.EnterGame(playerId);
    }

    // 플레이어 추가 또는 업데이트
    public void AddOrUpdatePlayer(S_ENTER_GAME pkt)
    {
        for (int i = 0; i < pkt.Players.Count; i++)
        {
            if (!_players.ContainsKey(pkt.Players[i].Id))
            {
                GameObject prefab = playerPrefab;

                // 새로운 플레이어 생성
                GameObject player = Instantiate(prefab, new Vector2(pkt.Players[i].PosX, pkt.Players[i].PosY), Quaternion.identity);
                player.name = pkt.Players[i].Id.ToString();
                _players[pkt.Players[i].Id] = player;

                if (pkt.ToPlayer == ToPlayer.Owner)
                {
                    SetPlayerId(pkt.Players[0].Id);
                    player.GetComponent<PlayerController>().SetPlayerId(pkt.Players[0].Id);
                    player.GetComponent<PlayerController>().SetPlayerName(GetPlayerName());
                }
            }
            else
            {
                // 기존 플레이어의 위치 업데이트
                _players[pkt.Players[i].Id].transform.position = new Vector2(pkt.Players[i].PosX, pkt.Players[i].PosY);
            }
        }
    }

    // 플레이어 제거 (필요시)
    public void RemovePlayer(ulong playerId)
    {
        if (_players.ContainsKey(playerId))
        {
            Destroy(_players[playerId]);
            _players.Remove(playerId);
            // TODO Room Leave
        }
    }

    public void SetPlayerId(ulong playerId)
    {
        _playerId = playerId;
    }

    public ulong GetPlayerId()
    {
        return _playerId;
    }
    public void SetPlayerName(string playerName)
    {
        _playerName = playerName;
    }

    public string GetPlayerName()
    {
        return _playerName;
    }
}
