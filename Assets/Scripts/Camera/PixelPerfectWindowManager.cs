using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 管理像素完美窗口缩放与分辨率设置，防止超出视野或裁剪内容
/// </summary>
[RequireComponent(typeof(PixelPerfectCamera))]
public class PixelPerfectWindowManager : MonoBehaviour
{
    [Header("像素完美基础设置")]
    public int referencePPU = 16; // 与素材一致
    public Vector2Int referenceResolution = new Vector2Int(320, 180); // 设计分辨率

    [Header("运行时配置")]
    public bool applyOnStart = true;
    public bool upscaleRT = true; // 启用放大渲染（推荐）
    public bool pixelSnapping = true;

    private PixelPerfectCamera ppCamera;
    private int currentScale = 1;

    private const string PlayerPrefsKey = "PixelScale";

    void Awake()
    {
        SetupPixelPerfectCamera();

        if (applyOnStart)
        {
            LoadScaleFromPrefs();
            ApplyResolution(currentScale);
        }
    }

    void SetupPixelPerfectCamera()
    {
        ppCamera = GetComponent<PixelPerfectCamera>();

        // 正交摄像机设置
        var cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = referenceResolution.y / (2f * referencePPU);
        }

        // Pixel Perfect 设置
        ppCamera.assetsPPU = referencePPU;
        ppCamera.refResolutionX = referenceResolution.x;
        ppCamera.refResolutionY = referenceResolution.y;

        // 裁剪画面内容，防止视野超出（黑边会自动添加）
        ppCamera.cropFrameX = true;
        ppCamera.cropFrameY = true;

        // 使用RT进行放大（不模糊，完整内容显示）
        ppCamera.upscaleRT = upscaleRT;

        // 防止像素抖动
        ppCamera.pixelSnapping = pixelSnapping;
    }

    /// <summary>
    /// 计算最大缩放倍数，防止过大
    /// </summary>
    int CalculateMaxScale()
    {
        int screenHeight = Screen.currentResolution.height;
        int maxScale = Mathf.FloorToInt(screenHeight / (float)referenceResolution.y);
        return Mathf.Clamp(maxScale, 1, 8);
    }

    /// <summary>
    /// 应用分辨率（窗口模式）
    /// </summary>
    public void ApplyResolution(int scale)
    {
        currentScale = Mathf.Clamp(scale, 1, 8);
        int width = referenceResolution.x * currentScale;
        int height = referenceResolution.y * currentScale;

        Screen.SetResolution(width, height, FullScreenMode.Windowed);

        PlayerPrefs.SetInt(PlayerPrefsKey, currentScale);
        PlayerPrefs.Save();

        Debug.Log($"设置分辨率：{width}x{height}（x{currentScale}倍）");
    }

    void LoadScaleFromPrefs()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
            currentScale = PlayerPrefs.GetInt(PlayerPrefsKey);
        else
            currentScale = CalculateMaxScale();
    }

    /// <summary>
    /// 增加缩放
    /// </summary>
    public void IncreaseScale()
    {
        ApplyResolution(currentScale + 1);
    }

    /// <summary>
    /// 减少缩放
    /// </summary>
    public void DecreaseScale()
    {
        ApplyResolution(currentScale - 1);
    }

    public int GetCurrentScale()
    {
        return currentScale;
    }
}
