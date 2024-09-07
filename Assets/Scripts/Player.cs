using Google.Protobuf.Protocol;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI playerNameTMP;
    [SerializeField] private TextMeshProUGUI currentHPTMP;
    [SerializeField] private TextMeshProUGUI maxHPTMP;

    [Header("Body")]
    private SpriteRenderer _sprite;

    [Header("Hand")]
    [SerializeField] private SpriteRenderer leftHand;
    [SerializeField] private SpriteRenderer rightHand;
    Vector3 rightPos = new Vector3(0.35f, -0.15f, 0);
    Vector3 rightPosReverse = new Vector3(-0.15f, -0.15f, 0);
    Quaternion leftRot = Quaternion.Euler(0, 0, -35);
    Quaternion leftRotReverse = Quaternion.Euler(0, 0, -135);

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    public void FlipX(bool left)
    {
        _sprite.flipX = left;

        leftHand.transform.localRotation = left ? leftRotReverse : leftRot;
        leftHand.flipY = left;
        leftHand.sortingOrder = left ? 4 : 6;

        rightHand.transform.localPosition = left ? rightPosReverse : rightPos;
        rightHand.flipX = left;
        rightHand.sortingOrder = left ? 6 : 4;
    }

    public bool IsStanceLeft()
    {
        return _sprite.flipX;
    }
    public void SetPlayerNameUI(string name)
    {
        playerNameTMP.text = name;
    }

    public void SetPlayerCurrentHPUI(ulong currentHP)
    {
        currentHPTMP.text = currentHP.ToString();
    }

    public void SetPlayerMaxHPUI(ulong maxHP)
    {
        maxHPTMP.text = maxHP.ToString();
    }

    public void HitBulletPlayer()
    {
        StartCoroutine(OnDamage());
    }

    IEnumerator OnDamage()
    {
        _sprite.color = Color.black;
        //_sprite.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.1f);

        _sprite.color = Color.white;

        //if (pkt.State == Google.Protobuf.Protocol.PlayerState.Dead)
        //{
        //    //player.GetComponent<SpriteRenderer>().color = Color.white;
        //    player.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        //    // TODO Dead
        //}
        //else
        //{
        //    //player.GetComponent<SpriteRenderer>().color = Color.white;
        //    player.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        //}
    }
}
