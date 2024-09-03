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

    // �÷��̾� �߰� �Ǵ� ������Ʈ
    public void AddOrUpdatePlayer(S_ENTER_GAME pkt)
    {
        for (int i = 0; i < pkt.Players.Count; i++)
        {
            if (!_players.ContainsKey(pkt.Players[i].Id))
            {
                GameObject prefab = playerPrefab;

                // ���ο� �÷��̾� ����
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
                // ���� �÷��̾��� ��ġ ������Ʈ
                //_players[pkt.Players[i].Id].transform.position = new Vector2(pkt.Players[i].PosX, pkt.Players[i].PosY);
                // ���� �÷��̾��� ��ġ ������Ʈ (�ε巴�� �̵�)
                StartCoroutine(
                    SmoothMove(
                        _players[pkt.Players[i].Id].transform,
                        new Vector2(pkt.Players[i].PosX, pkt.Players[i].PosY),
                        _speed
                    )
                ); // moveSpeed�� �ʴ� �̵� �Ÿ�
            }
        }
    }

    IEnumerator SmoothMove(Transform transform, Vector2 targetPosition, float speed)
    {
        Vector2 startPosition = transform.position;
        float distance = Vector2.Distance(startPosition, targetPosition);
        float duration = distance / speed; // �ʴ� �̵� �Ÿ��� �̵� �ð� ���

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // ���� ��ġ ����
    }

    // �÷��̾� ���� (�ʿ��)
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
