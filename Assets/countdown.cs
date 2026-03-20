using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CountdownTimer : MonoBehaviour
{
    public float totalTime = 600f;  // 总时间（例如 10 分钟）
    private float remainingTime;
    private bool hasEnded = false;

    public Text timerText;
    public UnityEvent onCountdownFinished;  // 外部事件（例如冻结控制）

    void Start()
    {
        remainingTime = totalTime;
        hasEnded = false;
    }

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime < 0) remainingTime = 0;
            UpdateTimerDisplay(remainingTime);
        }
        else if (!hasEnded)
        {
            hasEnded = true;
            onCountdownFinished.Invoke();  // 通知 GameManager
        }
    }

    void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = string.Format("Time left: {0:00}:{1:00}", minutes, seconds);
    }

    public void ResetTimer()
    {
        remainingTime = totalTime;
        hasEnded = false;
    }
}