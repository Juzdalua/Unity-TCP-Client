using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] public Transform bulletLeftPos;
    [SerializeField] public Transform bulletRightPos;
    [SerializeField] public Transform bulletPoolComponent;
    [SerializeField] public GameObject bulletPrefab;

    private float lastShotTime = 0;
    private float shotTerm = 1f;

    private void Update()
    {
        Shot();
    }

    void Shot()
    {
        if(PlayerManager.Instance.GetMyPlayerId() == GetComponent<PlayerController>().GetPlayerId())
        {
            if (lastShotTime != 0 && Time.time - lastShotTime < shotTerm)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 targetPos = GetComponent<Player>().IsStanceLeft() ? Vector2.left : Vector2.right;
                targetPos = targetPos.normalized;
                
                lastShotTime = Time.time;

                ClientPacketHandler.Instance.Shot(PlayerManager.Instance.GetMyPlayerId(), GetComponent<Player>().IsStanceLeft() ? bulletLeftPos.position : bulletRightPos.position.normalized, targetPos);
            }

            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    GameObject bullet = Instantiate(bulletPrefab, bulletPoolComponent);
            //    bullet.transform.position = GetComponent<Player>().IsStanceLeft() ? bulletLeftPos.position : bulletRightPos.position;
            //    if (GetComponent<Player>().IsStanceLeft())
            //        bullet.GetComponent<SpriteRenderer>().flipY = true;

            //    Vector3 targetPos = GetComponent<Player>().IsStanceLeft() ? Vector2.left : Vector2.right;
            //    targetPos = targetPos.normalized;

            //    bullet.GetComponent<Bullet>().Init(targetPos, targetPos.x < 0 ? bulletLeftPos.position : bulletRightPos.position);
            //    lastShotTime = Time.time;

            //    ClientPacketHandler.Instance.Shot(PlayerManager.Instance.GetMyPlayerId(), GetComponent<Player>().IsStanceLeft() ? bulletLeftPos.position : bulletRightPos.position.normalized, targetPos);
            //}
        }
    }
}
