using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GeneralDefine;

public class TurnTimer : MonoBehaviour
{
    [Header("UI Component")]
    public TextMeshProUGUI timeText;   
    public ProgressBarShrink progressBar;
    public RectTransform backGround;

    [Header("Timer Settings")]
    public float totalTime = 10f;    
    private float currentTime;
    private bool isRunning = false;

    [Header("Team Side")]
    public TEAM_SIDE teamSide;

    private static string BG_COLOR_BLACK = "#000000";
    private static string BG_COLOR_RED = "#C34000";

    void Start()
    {
        SetUI();
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0) currentTime = 0;

            UpdateUI();

            if (currentTime <= 0)
            {
                isRunning = false;
                OnTimerEnd();
            }
        }
    }

    public void SetTimerData(float duration)
    {
        totalTime = duration;
        ResetTimer();
        progressBar.SetProgress(duration, duration);
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
    public void ResetTimer()
    {
        currentTime = totalTime;
        UpdateUI();
    }

    private void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        progressBar.SetProgress(currentTime, totalTime);
    }

    private void OnTimerEnd()
    {
    }

    private void SetUI()
    {
        if (teamSide == TEAM_SIDE.ALLY) backGround.localScale = new Vector3(backGround.localScale.x, -backGround.localScale.y, backGround.localScale.z);
        else if (teamSide == TEAM_SIDE.ENEMY) backGround.localScale = new Vector3(backGround.localScale.x, backGround.localScale.y, backGround.localScale.z);
        progressBar.SetPositionY(teamSide == TEAM_SIDE.ALLY ? 83 : 0);
        SetBackgroundColor();
    }

    private void SetBackgroundColor()
    {
        Image img = backGround.GetComponent<Image>();
        if (img == null) return;
        Color newColor = Color.gray;
        if (this.teamSide == TEAM_SIDE.ENEMY) ColorUtility.TryParseHtmlString(BG_COLOR_BLACK, out newColor);
        else if (this.teamSide == TEAM_SIDE.ALLY) ColorUtility.TryParseHtmlString(BG_COLOR_RED, out newColor);
        newColor.a = 224f / 255f;
        img.color = newColor;
    }
}
