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

        // Player Set
        player.GetComponent<PlayerController>().SetPlayerId(PlayerManager.Instance.GetMyPlayerId());
        player.GetComponent<PlayerController>().SetPlayerName(PlayerManager.Instance.GetMyPlayerName());
        player.GetComponent<Player>().SetPlayerNameUI(
                    PlayerManager.Instance.GetMyPlayerName() == null ? "UNKNOWN" : PlayerManager.Instance.GetMyPlayerName() == "" ? "UNKNOWN" : PlayerManager.Instance.GetMyPlayerName());

        ClientPacketHandler.Instance.EnterGame(playerId);
    }

    // �÷��̾� �߰� �Ǵ� ������Ʈ
    public void AddOrUpdatePlayer(S_ENTER_GAME pkt)
    {
        for (int i = 0; i < pkt.Players.Count; i++)
        {
            Debug.Log($"ID: {pkt.Players[i].Id}");
            if (pkt.Players[i].Id == PlayerManager.Instance.GetMyPlayerId())
                continue;

            if (!_players.ContainsKey(pkt.Players[i].Id))
            {
                GameObject prefab = playerPrefab;

                // ���ο� �÷��̾� ����
                GameObject player = Instantiate(prefab, new Vector2(pkt.Players[i].PosX, pkt.Players[i].PosY), Quaternion.identity);
                player.name = pkt.Players[i].Id.ToString();
                _players[pkt.Players[i].Id] = player;

                // Player Set
                player.GetComponent<PlayerController>().SetPlayerId(pkt.Players[i].Id);
                player.GetComponent<PlayerController>().SetPlayerName(pkt.Players[i].Name);
                player.GetComponent<Player>().SetPlayerNameUI(
                    pkt.Players[i].Name == null ? "UNKNOWN" : pkt.Players[i].Name == "" ? "UNKNOWN" : pkt.Players[i].Name);
            }
            else
            {
                // ���� �÷��̾��� ��ġ ������Ʈ
                if (Vector2.Distance(_players[pkt.Players[i].Id].transform.position, new Vector2(pkt.Players[i].PosX, pkt.Players[i].PosY)) > 0.1f)
                {
                    Transform currentTransform = _players[pkt.Players[i].Id].transform;
                    Vector2 targetPosition = new Vector2(pkt.Players[i].PosX, pkt.Players[i].PosY);

                    StartCoroutine(SmoothMove(currentTransform, targetPosition,_speed));
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
        else if(transform.position.x < targetPosition.x)
        {
            transform.GetComponent<Player>().FlipX(false);
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
                Transform currentTransform = _players[recvPlayer.Id].transform;
                Vector2 targetPosition = new Vector2(recvPlayer.PosX, recvPlayer.PosY);

                StartCoroutine(SmoothMove(currentTransform, targetPosition, _speed));
            }
        }
    }

    // Shot Bullet
    public void ShotUpdate(S_SHOT pkt)
    {
        GameObject player = _players[pkt.PlayerId];
        WeaponController _weaponController = player.GetComponent<WeaponController>();

        GameObject bullet = Instantiate(_weaponController.bulletPrefab, _weaponController.bulletPoolComponent);
        
        bullet.GetComponent<Bullet>().SetShotPlayerId(pkt.PlayerId);

        bullet.transform.position = player.GetComponent<Player>().IsStanceLeft() ? _weaponController. bulletLeftPos.position : _weaponController.bulletRightPos.position;
        if (player.GetComponent<Player>().IsStanceLeft())
            bullet.GetComponent<SpriteRenderer>().flipY = true;

        Vector3 targetPos = player.GetComponent<Player>().IsStanceLeft() ? Vector2.left : Vector2.right;
        targetPos = targetPos.normalized;

        bullet.GetComponent<Bullet>().Init(targetPos, targetPos.x < 0 ? _weaponController.bulletLeftPos.position : _weaponController.bulletRightPos.position);
    }

    // Hit Bullet
    public void HitBulletUpdate(S_HIT pkt)
    {
        GameObject player = _players[pkt.PlayerId];
        StartCoroutine(OnDamage(player, pkt));

        // TODO player HPBar
    }

    IEnumerator OnDamage(GameObject player, S_HIT pkt)
    {
        //player.GetComponent<SpriteRenderer>().color = Color.black;
        player.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.1f);

        if(pkt.State == Google.Protobuf.Protocol.PlayerState.Dead)
        {
            //player.GetComponent<SpriteRenderer>().color = Color.white;
            player.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            // TODO Dead
        } 
        else
        {
            //player.GetComponent<SpriteRenderer>().color = Color.white;
            player.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
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

    public ulong GetMyPlayerId()
    {
        return _playerId;
    }
    public void SetPlayerName(string playerName)
    {
        _playerName = playerName;
    }

    public string GetMyPlayerName()
    {
        return _playerName;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public void SetPlayerCreateInfo(GameObject player)
    {
        player.GetComponent<PlayerController>().SetPlayerId(PlayerManager.Instance.GetMyPlayerId());
        player.GetComponent<PlayerController>().SetPlayerName(PlayerManager.Instance.GetMyPlayerName());
        player.GetComponentInChildren<TextMeshProUGUI>().text =
            PlayerManager.Instance.GetMyPlayerName() ?? "UNKNOWN";
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
