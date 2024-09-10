using Google.Protobuf.Protocol;
using UnityEngine;
public enum MoveDir
{
    UP = 0,
    DOWN = 1,
    LEFT = 2,
    RIGHT = 3,
}

enum PlayerState
{
    IDLE = 0,
    MOVE = 1,
    SKILL = 2,
    DEAD = 3,
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ulong playerId;
    [SerializeField] private string playerName;

    private Rigidbody2D _rb;
    public float speed = 5.0f;
    private Vector2 _targetPosition;

    MoveDir dir;
    PlayerState _state;
    bool _moveKeyPressed = false;
    bool _updated = false;

    int posX;
    int posY;
    Vector3Int cellPos;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _targetPosition = _rb.position;
        _state = PlayerState.IDLE;
        cellPos = Vector3Int.FloorToInt(new Vector3(transform.position.x, transform.position.y, 0));
    }

    void Update()
    {
        if (playerId == PlayerManager.Instance.GetMyPlayerId())
        {
            GetDirInput();
            UpdateMove();
        }
    }

    void GetDirInput()
    {
        _moveKeyPressed = true;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            dir = MoveDir.UP;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            dir = MoveDir.DOWN;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            dir = MoveDir.LEFT;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            dir = MoveDir.RIGHT;
        }
        else
        {
            _moveKeyPressed = false;
        }
    }

    void UpdateMove()
    {
        Vector3 destPos = cellPos + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            if (dir == MoveDir.LEFT)
                GetComponent<Player>().FlipX(true);
            else if (dir == MoveDir.RIGHT)
                GetComponent<Player>().FlipX(false);

            if (CanGo(destPos))
            {
                transform.position += moveDir.normalized * speed * Time.deltaTime;
                _state = PlayerState.MOVE;
                _updated = true;
            }
        }
    }

    void MoveToNextPos()
    {
        if (_moveKeyPressed == false)
        {
            _state = PlayerState.IDLE;
            //CheckUpdatedFlag();
            return;
        }

        Vector3Int destPos = cellPos;

        switch (dir)
        {
            case MoveDir.UP:
                destPos += Vector3Int.up;
                break;
            case MoveDir.DOWN:
                destPos += Vector3Int.down;
                break;
            case MoveDir.LEFT:
                destPos += Vector3Int.left;
                break;
            case MoveDir.RIGHT:
                destPos += Vector3Int.right;
                break;
        }
        
        if (CanGo(destPos))
        {
            cellPos = destPos;
            posX = destPos.x;
            posY = destPos.y;
            _updated = true;
        }

        CheckUpdatedFlag();
    }

    // 목표지점으로 이동하는데 장애물이 있는지 확인
    private bool CanGo(Vector3 destPos)
    {
        if (MapManager.Instance.CanGo(destPos))
        {
            if (PlayerManager.Instance.CanGo(destPos + new Vector3(0.5f, 0.5f, 0)))
            {
                return true;
            }
        }
        return false;
    }

    void CheckUpdatedFlag()
    {
        if (_updated)
        {
            ClientPacketHandler.Instance.Move(PlayerManager.Instance.GetMyPlayerId(), posX + 0.5f, posY + 0.5f, dir);
            _updated = false;
        }
    }

    // SET, GET ID & NAME
    public void SetPlayerId(ulong _playerId)
    {
        playerId = _playerId;
    }

    public void SetPlayerName(string _playerName)
    {
        playerName = _playerName;
    }

    public ulong GetPlayerId()
    {
        return playerId;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    // Hit Bullet
    public void HitBullet(ulong hitPlayerId, ulong damage)
    {
        if (PlayerManager.Instance.GetMyPlayerId() == hitPlayerId)
        {
            ClientPacketHandler.Instance.HitBullet(PlayerManager.Instance.GetMyPlayerId(), damage);
        }
    }
    
    // EatHealPack
    public void EatHealPack(ulong eatPlayerId, RoomItem healPackInfo)
    {
        if (PlayerManager.Instance.GetMyPlayerId() == eatPlayerId)
        {
            ClientPacketHandler.Instance.EatHealPack(eatPlayerId, healPackInfo);
        }
    }
}
