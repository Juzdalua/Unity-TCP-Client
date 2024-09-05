using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI playerNameTMP;

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
    public void SetPlayerNameUI(string name)
    {
        playerNameTMP.text = name;
    }

    public bool IsStanceLeft()
    {
        return _sprite.flipX;
    }
}
