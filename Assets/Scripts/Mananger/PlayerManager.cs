using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : Singleton<PlayerManager>
{
    // �پ��� ĳ���� ������
    //public List<GameObject> playerPrefabs; 
    public GameObject playerPrefab; 

    private Dictionary<string, GameObject> _players = new Dictionary<string, GameObject>();

    [SerializeField] int _playerId = 0;

    // �÷��̾� �߰� �Ǵ� ������Ʈ
    public void AddOrUpdatePlayer(string playerId, Vector2 position)
    {
        if (!_players.ContainsKey(playerId))
        {
            // ������ ���� (���÷� �÷��̾� ID�� �ؽ� ���� ���)
            //GameObject prefab = SelectPrefabByPlayerId(playerId);
            GameObject prefab = playerPrefab;

            // ���ο� �÷��̾� ����
            GameObject player = Instantiate(prefab, position, Quaternion.identity);
            player.name = playerId;
            _players[playerId] = player;
        }
        else
        {
            // ���� �÷��̾��� ��ġ ������Ʈ
            _players[playerId].transform.position = position;
        }
    }

    // �÷��̾� ���� (�ʿ��)
    public void RemovePlayer(string playerId)
    {
        if (_players.ContainsKey(playerId))
        {
            Destroy(_players[playerId]);
            _players.Remove(playerId);
        }
    }

    // �÷��̾� ID�� ���� ������ �������� ����
    //private GameObject SelectPrefabByPlayerId(string playerId)
    //{
    //    // ��: �÷��̾� ID�� �ؽ� ���� ����Ͽ� �ε����� ����
    //    int index = Mathf.Abs(playerId.GetHashCode()) % playerPrefabs.Count;

    //    // ��ȿ�� ������ �ε��� ��ȯ
    //    return playerPrefabs[index];
    //}

    void SetPlayerId(int playerId)
    {
        _playerId = playerId;
    }

    int GetPlayerId()
    {
        return _playerId;
    }
}