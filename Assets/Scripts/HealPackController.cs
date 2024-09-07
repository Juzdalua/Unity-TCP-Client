using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPackController : MonoBehaviour
{
    [SerializeField] ulong itemId;
    [SerializeField] GameObject healPack;
    private float term = 5f;
    private float lastSpawnTime = 5f;
    private bool isExist = false;
    private RoomItem roomItem;

    private void Awake()
    {
        healPack.SetActive(false);
        lastSpawnTime = 0f;
        isExist = false;
    }

    public void CreateHealPack()
    {
        healPack.SetActive(true);
        isExist = true;

        //if (isExist)
        //    return;

        //if(Time.time - lastSpawnTime > term)
        //{
        //    healPack.SetActive(true);
        //    lastSpawnTime = Time.time;
        //    isExist = true;
        //}
    }

    void EatHealPack()
    {
        healPack.SetActive(false);
        lastSpawnTime = Time.time;
        isExist = false;
    }

    public void SetRoomItem(RoomItem healPack)
    {
        roomItem = healPack;
    }

    public ulong GetRoomItemId()
    {
        return itemId;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isExist)
        {
            EatHealPack();
            collision.gameObject.GetComponent<PlayerController>().EatHealPack(collision.gameObject.GetComponent<PlayerController>().GetPlayerId(), roomItem);
        }
    }
}
