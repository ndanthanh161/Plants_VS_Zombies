using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;

    [Header("Audio")]
    public AudioClip menuMusic;

    void Start()
    {
        if (menuMusic != null)
            AudioManager.GetInstance().PlayMusic(menuMusic);
    }

    public void PressStart()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    // Hàm dùng chung để Load bất kỳ Level nào theo tên
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    // Giữ lại hàm cũ nếu bạn vẫn muốn dùng cho Level 1
    public void PlayLevel1()
    {
        SceneManager.LoadScene("Level_01");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}