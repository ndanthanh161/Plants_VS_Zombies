using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Audio")]
    public AudioClip backgroundMusic; // Kéo file .mp3 nhạc nền vào đây trong Inspector
    public AudioClip loseSound;       // Tiếng khi thua cuộc
    public AudioClip winSound;        // Tiếng khi thắng cuộc

    public int CurrentScore { get; private set; }

    bool isGameEnded = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        CurrentScore = 0;

        UIManager.Instance.UpdateScore(CurrentScore);

        // Phát nhạc nền khi vào màn chơi
        if (backgroundMusic != null)
            AudioManager.GetInstance().PlayMusic(backgroundMusic);
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        UIManager.Instance.UpdateScore(CurrentScore);
    }

    public void GameOver()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        // Dừng nhạc nền, phát tiếng thua
        AudioManager.GetInstance().StopMusic();
        if (loseSound != null)
            AudioManager.GetInstance().PlaySound(loseSound);

        UIManager.Instance.ShowLose();
    }

    public void WinGame()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        // Dừng nhạc nền, phát tiếng thắng
        AudioManager.GetInstance().StopMusic();
        if (winSound != null)
            AudioManager.GetInstance().PlaySound(winSound);

        UIManager.Instance.ShowWin(CurrentScore);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            SceneManager.LoadScene("MainMenu");
    }

    // Chuyển ra màn hình chính
    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // Tạm dừng game
    public void PauseGame()
    {
        Time.timeScale = 0f;
        UIManager.Instance.ShowPause(true);
    }

    // Tiếp tục game
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        UIManager.Instance.ShowPause(false);
    }
}
