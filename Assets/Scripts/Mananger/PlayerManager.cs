using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using System.Collections;

public class PlayerManager : Singleton<PlayerManager>
{
    private float _speed = 5f;
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

        ClientPacketHandler.Instance.EnterGame(playerId);
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
                StartCoroutine(
                    SmoothMove(
                        _players[pkt.Players[i].Id].transform,
                        new Vector2(pkt.Players[i].PosX, pkt.Players[i].PosY),
                        _speed
                    )
                );
            }
        }
    }

    IEnumerator SmoothMove(Transform transform, Vector2 targetPosition, float speed)
    {
        if(transform.position.x > targetPosition.x)
        {
            transform.GetComponent<Player>().FlipX(true);
        }

        Vector2 startPosition = transform.position;
        float distance = Vector2.Distance(startPosition, targetPosition);
        float duration = distance / speed; // 초당 이동 거리로 이동 시간 계산

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // 최종 위치 설정
    }

    public void MoveUpdatePlayer(Google.Protobuf.Protocol.Player recvPlayer)
    {
        if (!_players.ContainsKey(recvPlayer.Id))
        {
            GameObject prefab = playerPrefab;

            // 새로운 플레이어 생성
            GameObject player = Instantiate(prefab, new Vector2(recvPlayer.PosX, recvPlayer.PosY), Quaternion.identity);
            player.name = recvPlayer.Id.ToString();
            _players[recvPlayer.Id] = player;
        }
        else
        {
            // 기존 플레이어의 위치 업데이트
            StartCoroutine(
                SmoothMove(
                    _players[recvPlayer.Id].transform,
                    new Vector2(recvPlayer.PosX, recvPlayer.PosY),
                    _speed
                )
            ); // moveSpeed는 초당 이동 거리
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

    public float GetSpeed()
    {
        return _speed;
    }
}
