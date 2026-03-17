using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Top UI")]
    public TextMeshProUGUI scoreText;

    [Header("Panels")]
    public GameObject losePanel;
    public GameObject winPanel;
    public GameObject pausePanel;

    [Header("Win UI")]
    public TextMeshProUGUI winScoreText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (losePanel != null)
            losePanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "Your Score:\n" + score;
    }

    public void ShowLose()
    {
        if (losePanel != null)
            losePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ShowWin(int score)
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        if (winScoreText != null)
            winScoreText.text = "Your Score:\n" + score;

        Time.timeScale = 0f;
    }

    public void ShowPause(bool isShow)
    {
        if (pausePanel != null)
            pausePanel.SetActive(isShow);
    }
}
