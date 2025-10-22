using UnityEngine;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    private float _minPosX = -10f;
    private float _maxPosX = 10f;
    private float _minFollowX = -5f;
    private float _maxFollowX = 5f;
    private float _stageWidth = 18f;
    private float _startPosX = 0f;
    [SerializeField] private Stage[] _stages;
    public float MinFollowX => _minFollowX;
    public float MaxFollowX => _maxFollowX;
    public float MinPosX => _minPosX;
    public float MaxPosX => _maxPosX;
    void Start()
    {
        _minPosX = _startPosX - (_stageWidth / 2 - 1);
        _maxPosX = _startPosX + (_stageWidth / 2 - 1) + (_stageWidth * (_stages.Length - 1));
        _minFollowX = _startPosX;
        _maxFollowX = _startPosX + (_stageWidth * (_stages.Length - 1));
    }
}
