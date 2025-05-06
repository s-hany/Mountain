using UnityEngine;
using TMPro;
using unityroom.Api;

public class Result : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] private HeightMonitor heightMonitor; // HeightMonitorコンポーネントへの参照
    [SerializeField] private TextMeshProUGUI resultText; // 結果表示用のTextMeshProUGUIコンポーネントへの参照
    [SerializeField] private string heightUnit = "m"; // 高さの単位
    [SerializeField] private string displayFormat = "最終高度: {0:F2}{1}"; // 表示フォーマット


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 結果を表示する
        if (resultText != null)
        {
            // HeightMonitorから取得した高さを表示
            float height = heightMonitor.CurrentHeight;
            resultText.text = string.Format(displayFormat, height, heightUnit);
            UnityroomApiClient.Instance.SendScore(1, float.Parse(height.ToString("f1")), ScoreboardWriteMode.HighScoreDesc);
        }
        else
        {
            Debug.LogError("Result Text is not assigned in the inspector.");
        }    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
