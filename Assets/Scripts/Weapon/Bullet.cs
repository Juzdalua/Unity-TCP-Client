using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D _rb;
    public float damage = 1;
    public float speed = 5;
    Vector2 currentPos;
    private ulong shotPlayerId;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, currentPos) >= 1f)
        {
            DestroyBullet();
        }
    }

    public void Init(Vector3 dir, Vector2 spawnPos)
    {
        currentPos = spawnPos;
        _rb.velocity = dir * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DestroyBullet();
            ulong hitPlayerId = collision.gameObject.GetComponent<PlayerController>().GetPlayerId();
            if (shotPlayerId != hitPlayerId)
            {
                collision.gameObject.GetComponent<PlayerController>().HitBullet(hitPlayerId, damage);
            }
        }
    }

    void DestroyBullet()
    {
        _rb.velocity = Vector2.zero;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void SetShotPlayerId(ulong playerId)
    {
        shotPlayerId = playerId;
    }

    public ulong GetShotPlayerId()
    {
        return shotPlayerId;
    }
}
