using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  //单例声明
    public Text highScoreText; // 拖入 UI 中的 “High Score” 文本组件
    private int highScore = 0;
    public PlaneTiltController planeTiltController;   // 平台控制脚本
    public Rigidbody[] ballRigidbodies;               // 多个小球
    public GameObject gameOverPanel;                  // 游戏结束界面
    public Text gameOverScoreText;                    // 结算得分显示
    public Text scoreText;                            // 分数显示
    public CountdownTimer countdownTimer;             // 倒计时控制脚本

    private Vector3[] ballStartPositions;             // 小球初始位置记录
    private int score = 0;

    void Awake()
    {
        Instance = this; //单例初始化
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
        score = 0;
        UpdateScoreText();

        //读取历史最高分
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // 记录每个小球的初始位置
        ballStartPositions = new Vector3[ballRigidbodies.Length];
        for (int i = 0; i < ballRigidbodies.Length; i++)
        {
            ballStartPositions[i] = ballRigidbodies[i].transform.position;
        }
    }

    public void AddScore(int value)
    {
        score += value;
        Debug.Log("[Score] Added " + value + ", total now = " + score); //Debug 信息
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    // 由倒计时组件调用
    public void OnCountdownFinished()
    {
        Debug.Log("[CountdownFinished] score = " + score);

        planeTiltController.Freeze(); // 冻结平台控制

        foreach (Rigidbody rb in ballRigidbodies)
        {
            rb.isKinematic = true;    // 停止每个小球运动
        }

        //判断并更新历史最高分
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        //更新结算界面显示
        gameOverScoreText.text = $"Your score is: {score}";
        highScoreText.text = $"High Score: {highScore}";

        gameOverPanel.SetActive(true); // 显示结算UI
    }

    // 点击按钮后执行
    public void RestartGame()
    {
        planeTiltController.Unfreeze(); // 恢复平台控制

        for (int i = 0; i < ballRigidbodies.Length; i++)
        {
            Rigidbody rb = ballRigidbodies[i];
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.transform.position = ballStartPositions[i]; // 各自回到原始位置
        }

        score = 0;                //清零分数（替代 ScoreManager.ResetScore()）
        UpdateScoreText();

        countdownTimer.ResetTimer();   // 重置计时器
        gameOverPanel.SetActive(false); // 隐藏结算界面
    }
}