using UnityEngine;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using System.Collections;
using TMPro;

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

        if (playerId == PlayerManager.Instance.GetPlayerId())
            SetPlayerCreateInfo(player);

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

                if (pkt.Players[i].Id == PlayerManager.Instance.GetPlayerId())
                    SetPlayerCreateInfo(player);
            }
            else
            {
                // ���� �÷��̾��� ��ġ ������Ʈ
                if (Vector2.Distance(_players[pkt.Players[i].Id].transform.position, new Vector2(pkt.Players[i].PosX, pkt.Players[i].PosY)) > 0.1f)
                {
                    //StartCoroutine(
                    //    SmoothMove(
                    //        _players[pkt.Players[i].Id].transform,
                    //        new Vector2(pkt.Players[i].PosX, pkt.Players[i].PosY),
                    //        _speed
                    //    )
                    //);
                }
            }
        }
    }

    IEnumerator SmoothMove(Transform transform, Vector2 targetPosition, float speed)
    {
        if (transform.position.x > targetPosition.x)
        {
            transform.GetComponent<Player>().FlipX(true);
        }

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

    public void MoveUpdatePlayer(Google.Protobuf.Protocol.Player recvPlayer)
    {
        // ���ο� �÷��̾� ����
        if (!_players.ContainsKey(recvPlayer.Id))
        {
            GameObject prefab = playerPrefab;

            GameObject player = Instantiate(prefab, new Vector2(recvPlayer.PosX, recvPlayer.PosY), Quaternion.identity);
            player.name = recvPlayer.Id.ToString();
            _players[recvPlayer.Id] = player;
        }
        // ���� �÷��̾��� ��ġ ������Ʈ
        else
        {
            if (Vector2.Distance(_players[recvPlayer.Id].transform.position, new Vector2(recvPlayer.PosX, recvPlayer.PosY)) > 0.1f)
            {
                //StartCoroutine(
                //    SmoothMove(
                //        _players[recvPlayer.Id].transform,
                //        new Vector2(recvPlayer.PosX, recvPlayer.PosY),
                //        _speed
                //    )
                //);
            }
        }
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

    public void SetPlayerCreateInfo(GameObject player)
    {
        player.GetComponent<PlayerController>().SetPlayerId(PlayerManager.Instance.GetPlayerId());
        player.GetComponent<PlayerController>().SetPlayerName(PlayerManager.Instance.GetPlayerName());
        player.GetComponentInChildren<TextMeshProUGUI>().text =
            PlayerManager.Instance.GetPlayerName() ?? "UNKNOWN";
    }

    public bool CanGo(Vector3 destPos)
    {
        foreach (GameObject player in _players.Values)
        {
            if (player.transform.position == destPos)
                return false;
        }
        return true;
    }
}
