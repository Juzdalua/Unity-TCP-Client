using TMPro;
using UnityEngine;
public class Player : MonoBehaviour
{
    private SpriteRenderer _sprite;
    //private Rigidbody2D _rb;
    //private Vector2 _inputVec;
    //private Vector2 _currentPosition;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        //_rb = GetComponent<Rigidbody2D>();
    }

    //private void Start()
    //{
    //    _rb.interpolation = RigidbodyInterpolation2D.None;
    //}

    //private void FixedUpdate()
    //{
    //    if (GetComponent<PlayerController>().GetPlayerId() == PlayerManager.Instance.GetPlayerId())
    //    {
    //        GetInput();
    //        ApplyMovement();
    //    }
    //}

    //private void LateUpdate()
    //{
    //    if (_inputVec.magnitude != 0)
    //    {
    //        transform.position = _rb.position;

    //        // Send To Server
    //        //ClientPacketHandler.Instance.Move(PlayerManager.Instance.GetPlayerId(), _currentPosition.x, _currentPosition.y);
    //    }
    //}

    //private void GetInput()
    //{
    //    _inputVec.x = Input.GetAxisRaw("Horizontal");
    //    _inputVec.y = Input.GetAxisRaw("Vertical");
    //}

    //private void ApplyMovement()
    //{
    //    Vector2 movement = _inputVec.normalized * PlayerManager.Instance.GetSpeed() * Time.fixedDeltaTime;
    //    _rb.MovePosition(_rb.position + movement);
    //    transform.position = _rb.position;
    //    _currentPosition = _rb.position;

    //    if (_inputVec.x != 0)
    //    {
    //        _sprite.flipX = _inputVec.x < 0;
    //    }

    //    if (_inputVec.magnitude != 0)
    //    {
    //        // Send To Server
    //        //ClientPacketHandler.Instance.Move(PlayerManager.Instance.GetPlayerId(), _currentPosition.x, _currentPosition.y);
    //    }
    //}

    //public Vector2 GetCurrentPosition()
    //{
    //    return _currentPosition;
    //}

    public void FlipX(bool left)
    {
        _sprite.flipX = left;
    }
}
