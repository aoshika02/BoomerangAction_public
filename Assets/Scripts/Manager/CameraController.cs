using UnityEngine;

public class CameraController : SingletonMonoBehaviour<CameraController>
{
    [SerializeField] private float _followSmoothTime = 0.2f; // 滑らかさ
    [SerializeField] private float _offsetX = 2f;            // プレイヤー前方オフセット

    private PlayerController _playerController;
    private StageManager _stageManager;
    private bool _isFollowing = true;

    private Vector3 _velocity = Vector3.zero;
    private Camera _camera;

    void Start()
    {
        _playerController = PlayerController.Instance;
        _stageManager = StageManager.Instance;
        _camera= GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        if (_isFollowing == false || _playerController == null || _stageManager == null)
            return;

        // プレイヤー基準の目標位置
        Vector3 targetPos = transform.position;
        targetPos.x = _playerController.transform.position.x + _offsetX;

        // 範囲制限
        targetPos.x = Mathf.Clamp(targetPos.x, _stageManager.MinFollowX, _stageManager.MaxFollowX);

        // 滑らかに追従
        transform.position = Vector3.SmoothDamp(
            transform.position,
            new Vector3(targetPos.x, transform.position.y, transform.position.z),
            ref _velocity,
            _followSmoothTime
        );
    }

    public void SetFollow(bool isFollow)
    {
        _isFollowing = isFollow;
    }
    public Camera GetCamera() { return _camera; }
}
