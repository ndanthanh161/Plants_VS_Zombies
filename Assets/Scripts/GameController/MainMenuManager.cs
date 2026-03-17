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

    public void PlayLevel1()
    {
        SceneManager.LoadScene("Level_01");
    }

    // Thoát game hẳn (dùng cho bản Build)
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
