using UniRx;
using Cysharp.Threading.Tasks;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    public IReadOnlyReactiveProperty<int> Score => _score;
    private ReactiveProperty<int> _score = new ReactiveProperty<int>();
    private int _stackScore = 0;
    private bool _isCall;
    // 1フレームあたりの最大加算スコア
    private int _maxScoreUnit = 10;

    protected override void Awake()
    {
        if (CheckInstance() == false) return;
        _stackScore = 0;
        _score.Value = 0;
    }
    public void AddScore(int score)
    {
        _stackScore += score;
        AddScoreAsync().Forget();
    }
    private async UniTask AddScoreAsync() 
    {
        if(_isCall) return;
        _isCall = true;
        while (_stackScore > 0)
        {
            int unit =_maxScoreUnit;
            if (_stackScore < _maxScoreUnit) 
            {
                unit = _stackScore;
            }
            _stackScore -= unit;
            _score.Value += unit;
            await UniTask.Yield();
        }
        _isCall = false;
    }
 }
