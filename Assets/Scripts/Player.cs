using UnityEngine;
public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SpriteRenderer _sprite;
    private Vector2 _inputVec;
    private Vector2 _currentPosition;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        if (GetComponent<PlayerController>().GetPlayerId() == PlayerManager.Instance.GetPlayerId())
            ApplyMovement();

        //if (_inputVec.magnitude == 0)
        //    _rb.velocity = Vector2.zero;
    }

    private void GetInput()
    {
        _inputVec.x = Input.GetAxisRaw("Horizontal");
        _inputVec.y = Input.GetAxisRaw("Vertical");
    }

    private void ApplyMovement()
    {
        Vector2 movement = _inputVec.normalized * PlayerManager.Instance.GetSpeed() * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + movement);
        _currentPosition = _rb.position;

        if (_inputVec.x != 0)
        {
            _sprite.flipX = _inputVec.x < 0;
        }

        if (_inputVec.magnitude != 0)
        {
            // Send To Server
            ClientPacketHandler.Instance.Move(PlayerManager.Instance.GetPlayerId(), _currentPosition.x, _currentPosition.y);
        }
    }

    public Vector2 GetCurrentPosition()
    {
        return _currentPosition;
    }

    public void FlipX(bool left)
    {
        _sprite.flipX = left;
    }
}
