using TMPro;
using UnityEngine;
public class Player : MonoBehaviour
{
    private SpriteRenderer _sprite;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    public void FlipX(bool left)
    {
        _sprite.flipX = left;
    }
}
