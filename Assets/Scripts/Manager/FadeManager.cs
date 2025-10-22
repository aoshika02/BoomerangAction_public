using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
    [SerializeField] private Image _fadeImage;
    private static Image _staticFadeImage;
    private static Material _staticFadeMaterial;
    protected override void Awake()
    {
        if (CheckInstance() == false)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        _staticFadeImage = _fadeImage;
        _staticFadeMaterial = _staticFadeImage.material;
    }
    public static async UniTask FadeIn(float duration = 0.5f)
    {
        Color fadeColor = _staticFadeMaterial.GetColor(Shader.PropertyToID(ParamConsts.COLOR));
        await DOVirtual.Float(0, 1, duration, f => 
        {
            Color color = fadeColor;
            color.a = f;
            fadeColor = color;
            _staticFadeMaterial.SetColor(Shader.PropertyToID(ParamConsts.COLOR), fadeColor);
        }).ToUniTask();
    }
    public static async UniTask FadeOut(float duration = 0.5f)
    {
        Color fadeColor = _staticFadeMaterial.GetColor(Shader.PropertyToID(ParamConsts.COLOR));
        await DOVirtual.Float(1, 0, duration, f =>
        {
            Color color = fadeColor;
            color.a = f;
            fadeColor = color;
            _staticFadeMaterial.SetColor(Shader.PropertyToID(ParamConsts.COLOR), fadeColor);
        }).ToUniTask();
    }
    /// <summary>
    /// 画面を円を広げて表示
    /// </summary>
    /// <param name="color">ワイプ時の色</param>
    /// <param name="centerPos">円の中心地</param>
    /// <param name="smoothThickness">アンチエイリアスの厚さ</param>
    /// <param name="duration">かかる時間</param>
    /// <returns></returns>
    public static async UniTask IrisIn(Color color = default, Vector2 centerPos = default, float smoothThickness = 0, float duration = 0.5f)
    {
        await IrisWipe(1.2f, color, centerPos, smoothThickness, duration);
    }
    /// <summary>
    /// 画面を円を縮めて非表示
    /// </summary>
    /// <param name="color">ワイプ時の色</param>
    /// <param name="centerPos">円の中心地</param>
    /// <param name="smoothThickness">アンチエイリアスの厚さ</param>
    /// <param name="duration">かかる時間</param>
    /// <returns></returns>
    public static async UniTask IrisOut(Color color = default, Vector2 centerPos = default, float smoothThickness = 0, float duration = 0.5f)
    {
        await IrisWipe(0, color, centerPos, smoothThickness, duration);
    }
    /// <summary>
    /// アイリスワイプトランジション処理
    /// </summary>
    /// <param name="end">終了値</param>
    /// <param name="color">ワイプ時の色</param>
    /// <param name="centerPos">円の中心地</param>
    /// <param name="smoothThickness">アンチエイリアスの厚さ</param>
    /// <param name="duration">かかる時間</param>
    /// <returns></returns>
    private static async UniTask IrisWipe(float end, Color color = default, Vector2 centerPos = default, float smoothThickness = 0, float duration = 0.5f)
    {
        if (_staticFadeMaterial == null) return;
        if (centerPos == default)
        {
            centerPos = new Vector2(0.5f, 0.5f);
        }
        else
        {
            centerPos = new Vector2(Mathf.Clamp01(centerPos.x), Mathf.Clamp01(centerPos.y));
        }
        if (color == default) color = Color.black;
        //開始値取得
        float start = _staticFadeMaterial.GetFloat(Shader.PropertyToID(ParamConsts.CIRCLE_RADIUS));
        //初期化
        _staticFadeMaterial.SetColor(Shader.PropertyToID(ParamConsts.COLOR), color);
        _staticFadeMaterial.SetFloat(Shader.PropertyToID(ParamConsts.CENTER_X), centerPos.x);
        _staticFadeMaterial.SetFloat(Shader.PropertyToID(ParamConsts.CENTER_Y), centerPos.y);
        _staticFadeMaterial.SetFloat(Shader.PropertyToID(ParamConsts.SMOOTH_THICKNESS), smoothThickness);
        //アイリスワイプ実行
        await DOVirtual.Float(start, end, duration, f =>
        {
            _staticFadeMaterial.SetFloat(Shader.PropertyToID(ParamConsts.CIRCLE_RADIUS), f);
        })
        .OnComplete(() =>
        {
            _staticFadeMaterial.SetFloat(Shader.PropertyToID(ParamConsts.SMOOTH_THICKNESS), 0);
        }).ToUniTask();
    }
}
