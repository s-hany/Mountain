using UnityEngine;
using TMPro;

public class HeightMonitor : MonoBehaviour
{
    [Header("参照設定")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private TextMeshPro heightText3D;
    [SerializeField] private TextMeshProUGUI heightTextUI;

    [Header("計測設定")]
    [SerializeField] private bool useRelativeHeight = true; // 相対高度を使用するか
    [SerializeField] private bool trackDestroyedObjects = false;

    [Header("表示設定")]
    [SerializeField] private string heightUnit = "m";
    [SerializeField] private string displayFormat = "Height: {0:F2}{1}";
    [SerializeField] private float updateInterval = 0.1f;

    public float CurrentHeight { get; private set; } // 現在の高さ
    public float AbsoluteHeight { get; private set; } // 絶対高度
    private float initialYPosition;
    private float timer;

    private void Start()
    {
        InitializeTarget();
        RecordInitialHeight();
    }

    private void InitializeTarget()
    {
        if (targetTransform == null)
        {
            targetTransform = transform;
        }
    }

    private void RecordInitialHeight()
    {
        // 現在のオブジェクトのY座標を基準として記録
        initialYPosition = targetTransform.position.y;
        CurrentHeight = 0f;
        AbsoluteHeight = targetTransform.position.y;
    }

    private void Update()
    {
        if (targetTransform == null) return;

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdateHeightMeasurement();
            UpdateDisplay();
        }
    }

    private void UpdateHeightMeasurement()
    {
        float currentY = targetTransform.position.y;
        AbsoluteHeight = currentY; // 絶対高度を更新

        // 相対高度または絶対高度を選択し、絶対値を計算
        CurrentHeight = useRelativeHeight ? 
            Mathf.Abs(currentY - initialYPosition) : 
            Mathf.Abs(currentY);
    }

    private void UpdateDisplay()
    {
        float heightToDisplay = useRelativeHeight ? CurrentHeight : AbsoluteHeight; // 表示する高さを切り替え
        string formattedText = string.Format(
            displayFormat, 
            heightToDisplay, 
            heightUnit
        );

        UpdateTextDisplay(heightText3D, formattedText);
        UpdateTextDisplay(heightTextUI, formattedText);
    }

    private void UpdateTextDisplay(Component textComponent, string text)
    {
        if (textComponent != null)
        {
            var tmp = textComponent as TMP_Text;
            tmp.text = text;
        }
    }

    private void OnDestroy()
    {
        if (trackDestroyedObjects)
        {
            Debug.Log($"最終高度: {CurrentHeight}{heightUnit}");
        }
    }

    // 他のスクリプトから呼び出すためのメソッド
    public void ResetInitialHeight()
    {
        RecordInitialHeight();
    }

    public float GetAbsoluteHeight()
    {
        return targetTransform.position.y;
    }

    public float GetRelativeHeight()
    {
        return targetTransform.position.y - initialYPosition;
    }

    public float GetHeight()
    {
        return useRelativeHeight ? GetRelativeHeight() : GetAbsoluteHeight();
    }
}
