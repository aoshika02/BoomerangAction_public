using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using UnityEngine;

public class TimerManager : SingletonMonoBehaviour<TimerManager>
{
    private bool _isCounting = false;
    public IReadOnlyReactiveProperty<int> Time => _time;
    private ReactiveProperty<int> _time = new ReactiveProperty<int>(0);
    private CancellationTokenSource _tokenSource;

    public async UniTask Timer()
    {
        if (_isCounting) return;
        try
        {
            _isCounting = true;
            _tokenSource = new CancellationTokenSource();
            _time.Value = 0;
            while (true)
            {
                await UniTask.WaitForSeconds(1, cancellationToken: _tokenSource.Token);
                _time.Value++;
            }
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Timer Stopped");
        }
        finally
        {
            _isCounting = false;
        }
    }
    public void StopTimer()
    {
        _isCounting = false;
        _tokenSource.Cancel();
    }
}
