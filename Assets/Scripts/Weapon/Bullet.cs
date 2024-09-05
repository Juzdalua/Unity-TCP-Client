using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D _rb;
    public float damage = 1;
    public float speed = 5;
    Vector2 currentPos;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(Vector2.Distance(transform.position, currentPos) >= 1f)
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
        }
    }

    void DestroyBullet()
    {
        _rb.velocity = Vector2.zero;
        gameObject.SetActive(false);
        GameObject.Destroy(this);
    }
}
