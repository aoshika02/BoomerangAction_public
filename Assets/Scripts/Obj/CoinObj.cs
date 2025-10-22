using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class CoinObj : MonoBehaviour,IPool
{
    public bool IsGenericUse { get; set; }
    private CoinCountManager _coinCountManager;
    private ScoreManager _scoreManager;
    private int _coinValue = 1;
    private int _scoreValue = 100;
    private void Start()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        _coinCountManager = CoinCountManager.Instance;
        _scoreManager = ScoreManager.Instance;
    }
    public void Init(int coinValue = 1) 
    {
        _coinValue = coinValue;
    }
    public void OnRelease()
    {
        gameObject.SetActive(false);
    }

    public void OnReuse()
    {
        gameObject.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(ParamConsts.PLAYER_TAG)|| collision.gameObject.CompareTag(ParamConsts.BOOMERANG_TAG))
        {
            _coinCountManager.AddCoin(_coinValue);
            _scoreManager.AddScore(_scoreValue);
            gameObject.SetActive(false);
        }
    }
}
