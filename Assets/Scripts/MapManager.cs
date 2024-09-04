using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : Singleton<MapManager>
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap tilemap;

    [Header("Tilemap Boundary")]
    [SerializeField] private int maxX = 8;
    [SerializeField] private int minX = -9;
    [SerializeField] private int maxY = 4;
    [SerializeField] private int minY = -5;

    public bool CanGo(Vector3 destPos)
    {
        if (destPos.x > maxX || destPos.x < minX || destPos.y > maxY || destPos.y < minY)
            return false;
        return true;
    }
}
