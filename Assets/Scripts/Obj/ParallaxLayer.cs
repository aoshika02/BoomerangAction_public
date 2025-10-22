using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float _scrollSpeed = 0.5f;
    [SerializeField] private float _width; // 背景1枚の横幅
    private Vector3 _startPos;
    private CameraController _cameraController;

    private void Start()
    {
        _startPos = transform.position;
        _cameraController = CameraController.Instance;
    }

    private void Update()
    {
        float distance = _cameraController.transform.position.x * _scrollSpeed;
        transform.position = new Vector3(_startPos.x + distance, _startPos.y, _startPos.z);

        // 背景が一定距離を超えたら位置をリセットしてループ
        float _cameraParallaxX = _cameraController.transform.position.x * (1 - _scrollSpeed);
        if (_cameraParallaxX > _startPos.x + _width)
        {
            _startPos.x += _width;
        }
        else if (_cameraParallaxX < _startPos.x - _width)
        {
            _startPos.x -= _width;
        }
    }
}
