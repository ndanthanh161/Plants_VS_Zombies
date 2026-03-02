using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;

    [Header("Audio")]
    public AudioClip menuMusic; // Kéo file .mp3 nhạc Menu vào đây trong Inspector

    void Start()
    {
        // Phát nhạc nền khi ở màn hình chính
        if (menuMusic != null)
            AudioManager.GetInstance().PlayMusic(menuMusic);
    }

    public void PressStart()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void PlayLevel1()
    {
        SceneManager.LoadScene("Level_01");
    }
}
