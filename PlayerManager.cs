using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    // �پ��� ĳ���� ������
    public List<GameObject> playerPrefabs; // ����Ƽ �����Ϳ��� �Ҵ� (������ ���)

    private Dictionary<string, GameObject> _players = new Dictionary<string, GameObject>();

    // �÷��̾� �߰� �Ǵ� ������Ʈ
    public void AddOrUpdatePlayer(string playerId, Vector2 position)
    {
        if (!_players.ContainsKey(playerId))
        {
            // ������ ���� (���÷� �÷��̾� ID�� �ؽ� ���� ���)
            GameObject prefab = SelectPrefabByPlayerId(playerId);

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
    private GameObject SelectPrefabByPlayerId(string playerId)
    {
        // ��: �÷��̾� ID�� �ؽ� ���� ����Ͽ� �ε����� ����
        int index = Mathf.Abs(playerId.GetHashCode()) % playerPrefabs.Count;

        // ��ȿ�� ������ �ε��� ��ȯ
        return playerPrefabs[index];
    }
}
