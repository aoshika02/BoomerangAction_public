using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    private bool _isGoal;
    private bool _isDead;
    private TimerManager _timerManager;
    private CancellationTokenSource _tokenSource;

    async void Start()
    {
        _tokenSource = new CancellationTokenSource();
        await FadeManager.IrisIn(centerPos: GetPlayerPos());
        _timerManager = TimerManager.Instance;
        _timerManager.Timer().Forget();
        GoalAsync().Forget();
        DeadAsync().Forget();
    }
    private async UniTask GoalAsync()
    {
        _isGoal = false;
        try
        {
            await UniTask.WaitUntil(() => _isGoal,cancellationToken: _tokenSource.Token);
            _timerManager.StopTimer();
            PlayerController.Instance.SetMove(false);
            await ResultView.Instance.ViewResult();
            await UniTask.WaitForSeconds(10);
            await FadeManager.IrisOut(centerPos: GetPlayerPos());
            await SceneManager.LoadSceneAsync(0);
        }
        catch { }
    }
    private async UniTask DeadAsync()
    {
        _isDead = false;
        try
        {
            await UniTask.WaitUntil(() => _isDead, cancellationToken: _tokenSource.Token);
            PlayerController.Instance.SetMove(false);
            _timerManager.StopTimer();
            await FadeManager.IrisOut(centerPos: GetPlayerPos());
            await SceneManager.LoadSceneAsync(0);
        }
        catch { }
        }
    private Vector2 GetPlayerPos() 
    {
        var pos = PlayerController.Instance.transform.position;
        return CameraController.Instance.GetCamera().WorldToViewportPoint(pos);
    }
    public void Goal()
    {
        _isGoal = true;
    }
    public void Dead()
    {
        _isDead = true;
    }
}
