using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : Singleton<RoomManager>
{
    [Header("HealPack")]
    [SerializeField] private List<HealPackController> _healPackControllerList;
    private Dictionary<ulong, HealPackController> _healPackControllers = new Dictionary<ulong, HealPackController>();

    private void Awake()
    {
        for (int i = 0; i < _healPackControllerList.Count; i++)
        {
            _healPackControllerList[i].gameObject.SetActive(false);
            _healPackControllers[_healPackControllerList[i].GetRoomItemId()] = _healPackControllerList[i];
        }
    }

    public void CreateRoom(S_CREATE_ROOM pkt)
    {
        for (int i = 0; i < pkt.Item.Count; i++)
        {
            RoomItem roomItem = pkt.Item[i];
            if (roomItem != null)
            {
                _healPackControllers[roomItem.RoomItemId].SetRoomItem(roomItem);
                if (roomItem.State == RoomItemState.Available)
                {
                    _healPackControllers[roomItem.RoomItemId].gameObject.SetActive(true);
                    _healPackControllers[roomItem.RoomItemId].GetComponent<HealPackController>().CreateHealPack();
                }
            }

        }
    }
}
