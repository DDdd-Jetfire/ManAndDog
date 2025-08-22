using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// ����������������������ֱ������ã���ֹ������Ұ��ü�����
/// </summary>
[RequireComponent(typeof(PixelPerfectCamera))]
public class PixelPerfectWindowManager : MonoBehaviour
{
    [Header("����������������")]
    public int referencePPU = 16; // ���ز�һ��
    public Vector2Int referenceResolution = new Vector2Int(320, 180); // ��Ʒֱ���

    [Header("����ʱ����")]
    public bool applyOnStart = true;
    public bool upscaleRT = true; // ���÷Ŵ���Ⱦ���Ƽ���
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

        // �������������
        var cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = referenceResolution.y / (2f * referencePPU);
        }

        // Pixel Perfect ����
        ppCamera.assetsPPU = referencePPU;
        ppCamera.refResolutionX = referenceResolution.x;
        ppCamera.refResolutionY = referenceResolution.y;

        // �ü��������ݣ���ֹ��Ұ�������ڱ߻��Զ���ӣ�
        ppCamera.cropFrameX = true;
        ppCamera.cropFrameY = true;

        // ʹ��RT���зŴ󣨲�ģ��������������ʾ��
        ppCamera.upscaleRT = upscaleRT;

        // ��ֹ���ض���
        ppCamera.pixelSnapping = pixelSnapping;
    }

    /// <summary>
    /// ����������ű�������ֹ����
    /// </summary>
    int CalculateMaxScale()
    {
        int screenHeight = Screen.currentResolution.height;
        int maxScale = Mathf.FloorToInt(screenHeight / (float)referenceResolution.y);
        return Mathf.Clamp(maxScale, 1, 8);
    }

    /// <summary>
    /// Ӧ�÷ֱ��ʣ�����ģʽ��
    /// </summary>
    public void ApplyResolution(int scale)
    {
        currentScale = Mathf.Clamp(scale, 1, 8);
        int width = referenceResolution.x * currentScale;
        int height = referenceResolution.y * currentScale;

        Screen.SetResolution(width, height, FullScreenMode.Windowed);

        PlayerPrefs.SetInt(PlayerPrefsKey, currentScale);
        PlayerPrefs.Save();

        Debug.Log($"���÷ֱ��ʣ�{width}x{height}��x{currentScale}����");
    }

    void LoadScaleFromPrefs()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
            currentScale = PlayerPrefs.GetInt(PlayerPrefsKey);
        else
            currentScale = CalculateMaxScale();
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void IncreaseScale()
    {
        ApplyResolution(currentScale + 1);
    }

    /// <summary>
    /// ��������
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
