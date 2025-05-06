using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private float initialTime = 60f;
    [SerializeField] private GameObject resultWindow;

    [Header("表示設定")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private string timeFormat = "残り時間: {0:F1}秒";
    [SerializeField] private float warningThreshold = 10f;
    [SerializeField] private Color warningColor = Color.red;

    [SerializeField] private GameObject resultWindowParent;

    private float currentTime;
    private bool isTimerRunning = true;
    [SerializeField] private Color defaultColor;

    // 他のスクリプトから参照可能な状態
    public bool IsGameEnded { get; private set; }
    public float RemainingTime => currentTime;
    public bool IsPlayerAlive => playerObject != null;

    private void Awake()
    {
        PauseTimer();
    }

    private void Start()
    {
        InitializeTimer();
        CacheDefaultColor();
    }

    private void InitializeTimer()
    {
        currentTime = initialTime;
        UpdateTimerDisplay(currentTime);
    }

    private void CacheDefaultColor()
    {
        if (timerText != null)
        {
            defaultColor = timerText.color;
        }
    }

    private void Update()
    {
        if (!IsGameEnded && isTimerRunning)
        {
            UpdateTimerState();
            CheckGameEndConditions();
        }
    }

    private void UpdateTimerState()
    {
        currentTime -= Time.deltaTime;
        currentTime = Mathf.Max(currentTime, 0f);
        UpdateTimerDisplay(currentTime);
    }

    private void UpdateTimerDisplay(float time)
    {
        if (timerText != null)
        {
            timerText.text = string.Format(timeFormat, time);
            UpdateTextColor(time);
        }
    }

    private void UpdateTextColor(float time)
    {
        if (time <= warningThreshold)
        {
            timerText.color = warningColor;
        }
        else
        {
            timerText.color = defaultColor;
        }
    }

    private void CheckGameEndConditions()
    {
        if (currentTime <= 0f || !IsPlayerAlive)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        IsGameEnded = true;
        isTimerRunning = false;
        ShowResultWindow();
        //HandleGameEndState();
    }

    

    private void ShowResultWindow()
    {
        if (resultWindow != null && resultWindowParent != null)
        {
            resultWindow.SetActive(true);
        }
    }

    private void InitializeResultWindow(GameObject window)
    {
        var resultText = window.GetComponentInChildren<TextMeshProUGUI>();
        if (resultText != null)
        {
            resultText.text = GenerateResultText();
        }
    }

    private string GenerateResultText()
    {
        return IsPlayerAlive ? 
            $"制限時間終了!\n残り時間: {currentTime:F1}秒" : 
            "プレイヤーが破壊されました!";
    }
/*
    private void HandleGameEndState()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }
*/
    // 外部からの操作用メソッド
    public void PauseTimer() => isTimerRunning = false;
    public void ResumeTimer() => isTimerRunning = true;

    public void AddTime(float seconds) => currentTime += seconds;
}
