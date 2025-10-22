using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : SingletonMonoBehaviour<PlayerController>
{
    [SerializeField] private float _defaultMoveSpeed = 5f;
    [SerializeField] private float _maxMoveSpeed = 10f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _maxSpeedStack = 20f;
    [SerializeField] private float _minSpeedStack = 0f;
    [SerializeField] private float _speedUpDuration = 10f;
    [SerializeField] private float _gravity = 7.5f;
    [SerializeField] private BoomerangObj _boomerangObj;
    [SerializeField] private float _throwDistance = 2.5f;

    private int _direction = 1;
    private float _currentMoveSpeed;
    private Rigidbody2D _rigidbody2D;
    private bool _isJumping = false;
    private bool _ignoreGroundCheck = false;
    private bool _isMove = false;
    private Vector2 _contextVector;
    private Vector2 _throwVector;
    private InputManager _inputManager;
    private PlayerAnimController _playerAnimController;
    private StageManager _stageManager;
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimController = PlayerAnimController.Instance;
        _stageManager = StageManager.Instance;
        _currentMoveSpeed = _defaultMoveSpeed;
        _isMove = true;

        _inputManager = InputManager.Instance;
        _inputManager.Jump.Skip(1).Subscribe(_ => Jump()).AddTo(this);
        _inputManager.Attack.Skip(1).Subscribe(_ =>
        {
            if (_boomerangObj != null)
            {
                if (_throwVector == Vector2.zero)
                {
                    _boomerangObj.ThrowBoomerang(new Vector2(_direction, 0), transform.position, _throwDistance).Forget();
                }
                else
                {
                    _boomerangObj.ThrowBoomerang(_throwVector, transform.position, _throwDistance).Forget();
                }
            }
        }).AddTo(this);
        _inputManager.SetMovePerformedAction(PlayerMoveStart);
        _inputManager.SetMoveCanceledAction(PlayerMoveCancel);

        _inputManager.SetThrowPerformedAction(PlayerThrowStart);
        _inputManager.SetThrowCanceledAction(PlayerThrowCancel);
    }
    private void Update()
    {
        if (_isMove == false) return;
        var vecX = _contextVector.x;
        _rigidbody2D.velocity = new Vector2(vecX * _currentMoveSpeed, _rigidbody2D.velocity.y);
        _playerAnimController.SetIsMove(Mathf.Abs(_rigidbody2D.velocity.x) > 0.01f);

    }
    private void FixedUpdate()
    {
        if (_isMove == false) return;
        bool grounded = !_ignoreGroundCheck && IsGrounded();

        if (_isJumping && grounded)
        {
            _playerAnimController.JumpEnd();
            _isJumping = false;
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
        }

        if (_isJumping && !grounded)
        {
            _rigidbody2D.AddForce(Vector2.down * _gravity * (_contextVector.y < 0f ? -_contextVector.y : 1), ForceMode2D.Force);
        }

        if (!IsGrounded())
        {
            // 空中にいるとき常に重力を加える
            _rigidbody2D.AddForce(Vector2.down * _gravity, ForceMode2D.Force);
        }
        else
        {
            // 地面にいるとき垂直速度をリセット
            if (!_isJumping)
            {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
            }
        }

    }
    private void LateUpdate()
    {
        var pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, _stageManager.MinPosX, _stageManager.MaxPosX);
        transform.position = pos;
    }
    public async void Jump()
    {
        if (_isMove == false)
        {
            return;
        }
        if (_isJumping) return;
        if (Mathf.Abs(_rigidbody2D.velocity.y) < 0.01f)
        {
            _ignoreGroundCheck = true;
            _playerAnimController.JumpStart();
            _rigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _isJumping = true;
        }

        await UniTask.WaitForSeconds(0.02f);
        _ignoreGroundCheck = false;
    }

    public void SetMove(bool isMove)
    {
        _isMove = isMove;
        if (isMove == false)
        {
            _rigidbody2D.velocity = Vector2.zero;
        }
    }
    private void PlayerMoveStart(InputAction.CallbackContext context)
    {
        _contextVector = context.ReadValue<Vector2>();
        if(_contextVector.x != 0) 
        {
            _direction = _contextVector.x > 0 ? 1 : -1;
            transform.localScale = new Vector3(_direction, 1, 1);
        }
    }
    private void PlayerMoveCancel(InputAction.CallbackContext context)
    {
        _contextVector = Vector2.zero;
    }
    private void PlayerThrowStart(InputAction.CallbackContext context)
    {
        _throwVector = context.ReadValue<Vector2>();
    }
    private void PlayerThrowCancel(InputAction.CallbackContext context)
    {
        _throwVector = Vector2.zero;
    }
    private bool IsGrounded()
    {
        var col = GetComponent<Collider2D>();
        var bounds = col.bounds;
        Debug.Log(bounds);
        float extraHeight = 0.1f;
        bool grounded = Physics2D.BoxCast(bounds.center, bounds.size, 0f, Vector2.down, extraHeight, LayerMask.GetMask(ParamConsts.GROUND));

        Debug.DrawRay(bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, 0), Vector2.down * extraHeight, Color.green);
        Debug.DrawRay(bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, 0), Vector2.down * extraHeight, Color.green);

        return grounded;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(ParamConsts.ENEMY_TAG)) 
        {
            GameManager.Instance.Dead();
        }
    }
    private void OnDestroy()
    {
        if (_inputManager != null)
        {
            _inputManager.RemoveMoveStartedAction(PlayerMoveStart);
            _inputManager.RemoveMovePerformedAction(PlayerMoveStart);
            _inputManager.RemoveMoveCanceledAction(PlayerMoveCancel);

            _inputManager.RemoveThrowStartedAction(PlayerThrowStart);
            _inputManager.RemoveThrowPerformedAction(PlayerThrowStart);
            _inputManager.RemoveThrowCanceledAction(PlayerThrowCancel);
        }
    }
}
