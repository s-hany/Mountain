using UnityEngine;
using TMPro;

public class SpeedMonitor : MonoBehaviour
{
    [Header("参照設定")]
    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private TextMeshPro speedTextDisplay;
    [SerializeField] private TextMeshProUGUI uiSpeedTextDisplay;
    [SerializeField] private TextMeshProUGUI highSpeedTextDisplay; // 高速時の表示用TextMeshPro

    [Header("表示設定")]
    [SerializeField] private bool showFullSpeed = true;
    [SerializeField] private bool showHorizontalSpeed = false;
    [SerializeField] private bool showVerticalSpeed = false;
    [SerializeField] private string speedUnit = "m/s";
    [SerializeField] private string displayFormat = "Speed: {0:F2} {1}";
    [SerializeField] private float updateInterval = 0.1f;

    [Header("高速時の切り替え設定")]
    [SerializeField] private bool enableHighSpeedSwitch = true; // 高速時の切り替えを有効にするか
    [SerializeField] private float highSpeedThreshold = 10f; // 高速とみなす速度の閾値

    // 速度情報の公開プロパティ（他のスクリプトから参照用）
    public float CurrentSpeed { get; private set; }
    public float HorizontalSpeed { get; private set; }
    public float VerticalSpeed { get; private set; }
    public Vector3 VelocityVector { get; private set; }

    private float timer;

    private void Start()
    {
        // ターゲットRigidbodyが設定されていない場合は自身のオブジェクトから取得を試みる
        if (targetRigidbody == null)
        {
            targetRigidbody = GetComponent<Rigidbody>();
        }

        if (targetRigidbody == null)
        {
            Debug.LogError("SpeedMonitor: Rigidbodyが設定されていないか、見つかりませんでした。");
            enabled = false;
            return;
        }

        // 初期値の設定
        CurrentSpeed = 0f;
        HorizontalSpeed = 0f;
        VerticalSpeed = 0f;
        timer = 0f;

        // 高速時のTextMeshProを非表示にする
        if (highSpeedTextDisplay != null)
        {
            highSpeedTextDisplay.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // 指定した間隔で更新
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdateSpeedDisplay();
        }
    }

    private void UpdateSpeedDisplay()
    {
        if (targetRigidbody == null) return;

        // 速度情報の更新
        VelocityVector = targetRigidbody.linearVelocity;
        CurrentSpeed = VelocityVector.magnitude;
        HorizontalSpeed = new Vector3(VelocityVector.x, 0f, VelocityVector.z).magnitude;
        VerticalSpeed = Mathf.Abs(VelocityVector.y);

        // 表示する速度の決定
        float displaySpeed = showFullSpeed ? CurrentSpeed : 
                            (showHorizontalSpeed ? HorizontalSpeed : 
                            (showVerticalSpeed ? VerticalSpeed : CurrentSpeed));

        // 高速時の切り替え機能が有効な場合
        if (enableHighSpeedSwitch && highSpeedTextDisplay != null)
        {
            if (CurrentSpeed >= highSpeedThreshold)
            {
                // 高速時のTextMeshProを表示し、通常のTextMeshProを非表示にする
                if (speedTextDisplay != null) speedTextDisplay.gameObject.SetActive(false);
                if (uiSpeedTextDisplay != null) uiSpeedTextDisplay.gameObject.SetActive(false);
                highSpeedTextDisplay.gameObject.SetActive(true);
                highSpeedTextDisplay.text = string.Format(displayFormat, displaySpeed, speedUnit);
                return;
            }
            else
            {
                // 通常のTextMeshProを表示し、高速時のTextMeshProを非表示にする
                if (speedTextDisplay != null) speedTextDisplay.gameObject.SetActive(true);
                if (uiSpeedTextDisplay != null) uiSpeedTextDisplay.gameObject.SetActive(true);
                highSpeedTextDisplay.gameObject.SetActive(false);
            }
        }

        // 通常のTextMeshProの表示を更新
        if (speedTextDisplay != null)
        {
            speedTextDisplay.text = string.Format(displayFormat, displaySpeed, speedUnit);
        }

        if (uiSpeedTextDisplay != null)
        {
            uiSpeedTextDisplay.text = string.Format(displayFormat, displaySpeed, speedUnit);
        }
    }

    // 他のスクリプトから呼び出し可能な関数
    public string GetSpeedText()
    {
        return string.Format(displayFormat, CurrentSpeed, speedUnit);
    }

    public string GetHorizontalSpeedText()
    {
        return string.Format(displayFormat, HorizontalSpeed, speedUnit);
    }

    public string GetVerticalSpeedText()
    {
        return string.Format(displayFormat, VerticalSpeed, speedUnit);
    }
}
