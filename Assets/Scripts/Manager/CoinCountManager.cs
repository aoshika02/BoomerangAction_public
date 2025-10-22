using UniRx;

public class CoinCountManager : SingletonMonoBehaviour<CoinCountManager>
{
    public IReadOnlyReactiveProperty<int> CoinCount => _coinCount;
    private ReactiveProperty<int> _coinCount = new ReactiveProperty<int>(0);
    public void Init() 
    {
        _coinCount.Value = 0;
    }
    public void AddCoin(int count) => _coinCount.Value += count;
    public int GetCoin() => _coinCount.Value;
}
