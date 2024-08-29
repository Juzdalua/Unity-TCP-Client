using UnityEngine;
public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
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
        ApplyMovement();
    }

    private void GetInput()
    {
        _inputVec.x = Input.GetAxisRaw("Horizontal");
        _inputVec.y = Input.GetAxisRaw("Vertical");
    }

    private void ApplyMovement()
    {
        Vector2 movement = _inputVec.normalized * speed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + movement);
        _currentPosition = _rb.position;

        if(_inputVec.x != 0)
        {
            _sprite.flipX = _inputVec.x < 0;
        }
    }

    public Vector2 GetCurrentPosition()
    {
        return _currentPosition;
    }
}
