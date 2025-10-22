using UnityEngine;

public class FieldObjPool : SingletonMonoBehaviour<FieldObjPool>
{
    private GenericObjectPool<CoinObj> _coinPool;

    [SerializeField] private GameObject _coinObj;

    [SerializeField] private Transform _coinPoolParent;
    protected override void Awake()
    {
        if (CheckInstance() == false)
        {
            return;
        }
        _coinPool = new GenericObjectPool<CoinObj>(_coinObj, _coinPoolParent);
    }
    public CoinObj GetCoinObj()
    {
        return _coinPool.Get();
    }
    public void ReleaseObj(CoinObj obj)
    {
        _coinPool.Release(obj);
    }
}