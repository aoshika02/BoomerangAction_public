using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ResultView : SingletonMonoBehaviour<ResultView>
{
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private CanvasGroup _canvasGroup;
    private int _time = 0;
    private int _coin = 0;
    private int _score = 0;
    private void Start()
    {
        _canvasGroup.alpha = 0;
        _time = 0;
        _coin = 0;
        _score = 0;
        _timeText.text = "00:00";
        _coinText.text = "0";
        _scoreText.text = "0";
    }
    public async UniTask ViewResult()
    {
        _canvasGroup.alpha = 1;
        var target = TimerManager.Instance.Time.Value;
        while (target > 0)
        {
            _time++;
            target--;
            int minutes = _time / 60;
            int seconds = _time % 60;
            _timeText.text = $"{minutes:00}:{seconds:00}";
            await UniTask.Yield();
        }
        target = CoinCountManager.Instance.CoinCount.Value;

        while (target > 0)
        {
            _coin++;
            target--;
            _coinText.text = _coin.ToString();
            await UniTask.Yield();
        }

        target = ScoreManager.Instance.Score.Value;
        await DOTween.To(() => _score, x => {
            _score = x;
            _scoreText.text = _score.ToString(); ;
        }, target, 1f);
    }
}
