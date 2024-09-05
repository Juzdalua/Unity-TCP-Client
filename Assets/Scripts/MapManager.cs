using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : Singleton<MapManager>
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap tilemap;

    [Header("Tilemap Boundary")]
    // 1920*1080
    //private int maxX = 8;
    //private int minX = -9;

    // 800*600
    private int maxX = 5;
    private int minX = -6;

    private int maxY = 4;
    private int minY = -5;

    public void GetTilemapCurrentCellPos(Vector3 destPos)
    {
        Debug.Log(tilemap.WorldToCell(destPos));
    }

    public bool CanGo(Vector3 destPos)
    {
        if (destPos.x > maxX || destPos.x < minX || destPos.y > maxY || destPos.y < minY)
            return false;
        return true;
    }
}
