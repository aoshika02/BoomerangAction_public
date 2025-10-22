using TMPro;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class StatusView : SingletonMonoBehaviour<StatusView>
{
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    async void Start()
    {
        await UniTask.WaitUntil(() =>
           TimerManager.Instance != null &&
           CoinCountManager.Instance != null &&
           PlayerController.Instance != null);
        TimerManager.Instance.Time.Subscribe(x =>
        {
            x = Mathf.Clamp(x, 0, 5999);
            int minutes = x / 60;
            int seconds = x % 60;
            _timeText.text = $"{minutes:00}:{seconds:00}";
        }).AddTo(this);
        CoinCountManager.Instance.CoinCount.Subscribe(x =>
        {
            x = Mathf.Clamp(x, 0, 99999);
            _coinText.text = x.ToString();
        }).AddTo(this);
        ScoreManager.Instance.Score.Subscribe(x =>
        {
            x = Mathf.Clamp(x, 0, 99999);
            _scoreText.text = x.ToString();
        });
    }
}
