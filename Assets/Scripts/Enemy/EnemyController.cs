using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyAnimController _enemyAnimController;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private int _health = 3;
    [SerializeField] private int _moveDirection = 1;
    [SerializeField] private int _scoreValue = 100;
    [SerializeField] private Transform _startPos;
    [SerializeField] private Transform _endPos;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;
    private ScoreManager _scoreManager;
    private bool _isCall = false;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    void Start()
    {
        _scoreManager = ScoreManager.Instance;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        MoveFlow(_cancellationTokenSource.Token).Forget();
    }
    private async UniTask MoveFlow(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                SetVelocity(_endPos.position);
                await UniTask.WaitUntil(() => Vector2.Distance(transform.position, _endPos.position) < 0.1f, cancellationToken: token);
                SetVelocity(_startPos.position);
                await UniTask.WaitUntil(() => Vector2.Distance(transform.position, _startPos.position) < 0.1f, cancellationToken: token);
            }
        }
        catch 
        {
        
        }
    }
    private void SetVelocity(Vector2 targetPos)
    {
        var dir = (targetPos - (Vector2)transform.position).normalized;
        _rigidbody2D.velocity = dir * _moveSpeed;
        SetDirection(dir);
    }
    private void SetDirection(Vector2 direction)
    {
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    private void TakeDamage(int damage)
    {
        if(_isCall) return;
        _health -= damage;
        if (_health <= 0)
        {
            Die();
        }
        else
        {
            DamageAsync(3, _cancellationTokenSource.Token).Forget();
        }
    }
    private async UniTask DamageAsync(int blinkCount, CancellationToken token)
    {
        if (_isCall) return;
        _isCall = true;
        var count = 0;
        try
        {
            while (count < blinkCount)
            {
                await _spriteRenderer.DOColor(Color.red, 0.1f).ToUniTask(cancellationToken: token);
                await _spriteRenderer.DOColor(Color.white, 0.1f).ToUniTask(cancellationToken: token);
                count++;
            }
        }
        catch 
        {

        }
        _isCall = false;
    }

    private void Die()
    {
        _cancellationTokenSource.Cancel();
        _rigidbody2D.velocity = Vector2.zero;
        _enemyAnimController.SetIsDead(true);
        _scoreManager.AddScore(_scoreValue);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
        _rigidbody2D.velocity = Vector2.zero;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(ParamConsts.BOOMERANG_TAG))
        {
            TakeDamage(1);
        }
    }
}
