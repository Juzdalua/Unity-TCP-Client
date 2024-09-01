using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    // 다양한 캐릭터 프리팹
    public List<GameObject> playerPrefabs; // 유니티 에디터에서 할당 (프리팹 목록)

    private Dictionary<string, GameObject> _players = new Dictionary<string, GameObject>();

    // 플레이어 추가 또는 업데이트
    public void AddOrUpdatePlayer(string playerId, Vector2 position)
    {
        if (!_players.ContainsKey(playerId))
        {
            // 프리팹 선택 (예시로 플레이어 ID의 해시 값을 사용)
            GameObject prefab = SelectPrefabByPlayerId(playerId);

            // 새로운 플레이어 생성
            GameObject player = Instantiate(prefab, position, Quaternion.identity);
            player.name = playerId;
            _players[playerId] = player;
        }
        else
        {
            // 기존 플레이어의 위치 업데이트
            _players[playerId].transform.position = position;
        }
    }

    // 플레이어 제거 (필요시)
    public void RemovePlayer(string playerId)
    {
        if (_players.ContainsKey(playerId))
        {
            Destroy(_players[playerId]);
            _players.Remove(playerId);
        }
    }

    // 플레이어 ID에 따라 적절한 프리팹을 선택
    private GameObject SelectPrefabByPlayerId(string playerId)
    {
        // 예: 플레이어 ID의 해시 값을 사용하여 인덱스를 생성
        int index = Mathf.Abs(playerId.GetHashCode()) % playerPrefabs.Count;

        // 유효한 프리팹 인덱스 반환
        return playerPrefabs[index];
    }
}
