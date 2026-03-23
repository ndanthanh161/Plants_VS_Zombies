using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton để truy cập GameManager toàn cục
    public static GameManager Instance;

    [Header("Audio")]
    public AudioClip backgroundMusic; // Nhạc nền
    public AudioClip loseSound;       // Âm thanh khi thua
    public AudioClip winSound;        // Âm thanh khi thắng

    // Điểm hiện tại (chỉ cho đọc từ bên ngoài)
    public int CurrentScore { get; private set; }

    // Kiểm tra game đã kết thúc chưa (tránh gọi nhiều lần)
    bool isGameEnded = false;

    void Awake()
    {
        // Thiết lập Singleton (chỉ giữ lại 1 GameManager)
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // Nếu có cái thứ 2 thì xoá
    }

    void Start()
    {
        // Đảm bảo game chạy bình thường (không bị pause)
        Time.timeScale = 1f;

        // Reset điểm về 0 khi bắt đầu
        CurrentScore = 0;

        // Cập nhật UI hiển thị điểm
        UIManager.Instance.UpdateScore(CurrentScore);

        // Phát nhạc nền khi vào game
        if (backgroundMusic != null)
            AudioManager.GetInstance().PlayMusic(backgroundMusic);
    }

    // Hàm cộng điểm
    public void AddScore(int amount)
    {
        CurrentScore += amount; // Cộng điểm

        // Cập nhật UI
        UIManager.Instance.UpdateScore(CurrentScore);
    }
    // Khi thua game
    public void GameOver()
    {
        // Nếu game đã kết thúc rồi thì không làm gì nữa
        if (isGameEnded) return;
        isGameEnded = true;

        // Dừng nhạc nền
        AudioManager.GetInstance().StopMusic();

        // Phát âm thanh thua nếu có
        if (loseSound != null)
            AudioManager.GetInstance().PlaySound(loseSound);

        // Gọi Coroutine để chờ 3 giây rồi mới hiện giao diện thua
        StartCoroutine(ShowLoseDelayed(3f));
    }

    // Khi thắng game
    public void WinGame()
    {
        // Tránh gọi nhiều lần
        if (isGameEnded) return;
        isGameEnded = true;

        // Dừng nhạc nền
        AudioManager.GetInstance().StopMusic();

        // Phát âm thanh thắng nếu có
        if (winSound != null)
            AudioManager.GetInstance().PlaySound(winSound);

        // Gọi Coroutine để chờ 3 giây rồi mới hiện giao diện thắng
        StartCoroutine(ShowWinDelayed(3f));
    }

    // --- CÁC HÀM COROUTINE XỬ LÝ DELAY ---

    private IEnumerator ShowLoseDelayed(float delayTime)
    {
        // Chờ delayTime giây
        yield return new WaitForSeconds(delayTime);

        // Hiện UI thua (bên trong UIManager.ShowLose() đã có sẵn Time.timeScale = 0f)
        UIManager.Instance.ShowLose();
    }

    private IEnumerator ShowWinDelayed(float delayTime)
    {
        // Chờ delayTime giây
        yield return new WaitForSeconds(delayTime);

        // Hiện UI thắng + truyền điểm
        UIManager.Instance.ShowWin(CurrentScore);
    }
    // Chơi lại màn hiện tại
    public void Retry()
    {
        Time.timeScale = 1f; // Đảm bảo không bị pause

        // Load lại scene hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Sang level tiếp theo
    public void NextLevel()
    {
        Time.timeScale = 1f;

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // Nếu còn level tiếp thì load tiếp
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            // Nếu hết level thì quay về menu
            SceneManager.LoadScene("MainMenu");
    }

    // Quay về menu chính
    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // Tạm dừng game
    public void PauseGame()
    {
        Time.timeScale = 0f; // Dừng thời gian → game pause
        UIManager.Instance.ShowPause(true); // Hiện UI pause
    }

    // Tiếp tục game
    public void ResumeGame()
    {
        Time.timeScale = 1f; // Tiếp tục thời gian
        UIManager.Instance.ShowPause(false); // Ẩn UI pause
    }
}