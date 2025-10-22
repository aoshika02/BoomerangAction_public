using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BoomerangObj : MonoBehaviour
{
    CancellationTokenSource _cts = new CancellationTokenSource();
    private Rigidbody2D _rigidbody2D;
    private float _speed = 10f;
    private Vector2 _startPos;
    private Tween _rotateTween;
    private BoomerangState _boomerangState = BoomerangState.None;
    private float _duration = 0.25f;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    public async UniTask ThrowBoomerang(Vector2 moveVector,Vector2 startPos,float distance)
    {
        if (_boomerangState != BoomerangState.None) return;
        transform.position = startPos;
        Vector3 euler = transform.eulerAngles;
        euler.z = 0;
        transform.eulerAngles = euler;

        _boomerangState = BoomerangState.Thrown;

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        gameObject.SetActive(true);

        _rotateTween?.Kill();
        _rotateTween = null;
        _rotateTween = transform
            .DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
        _rigidbody2D.velocity = moveVector.normalized * _speed;

        try
        {
            await UniTask.WaitUntil(() => Vector2.Distance(startPos, transform.position) >= distance, cancellationToken: _cts.Token);
        }
        catch (System.OperationCanceledException) { }
        finally
        {
            _rigidbody2D.velocity = Vector2.zero;
            _boomerangState = BoomerangState.Returning;
        }
    }
    private void Update()
    {
        
        if (_boomerangState == BoomerangState.Returning)
        {
            Vector2 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
            _rigidbody2D.velocity = direction * _speed;

            if (Vector2.Distance(PlayerController.Instance.transform.position, transform.position) < 0.5f)
            {
                _rigidbody2D.velocity = Vector2.zero;
                ReleaseObj().Forget();
                gameObject.SetActive(false);
            }
        }
    }
   public async UniTask ReleaseObj() 
    {
        await UniTask.WaitForSeconds(_duration);
        _boomerangState = BoomerangState.None;
    }
    private void OnDisable()
    {
        _rotateTween?.Kill();
        _rotateTween = null;
        try
        {
            _cts?.Cancel();
        }
        catch { }
        finally
        {
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(ParamConsts.ENEMY_TAG))
        {
            if (_boomerangState != BoomerangState.Thrown) return;
            _cts?.Cancel();
            _boomerangState = BoomerangState.Returning;
        }
    }
}
public enum BoomerangState
{
    None,
    Thrown,
    Returning
}
