using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance;

    public TextMeshProUGUI scoreText;

    void Awake()
    {
        Instance = this;
        Debug.Log("[ScoreUI] Awake - Instance initialized");
    }

    public void UpdateScore(int score)
    {
        Debug.Log("[ScoreUI] UpdateScore called with value: " + score);

        if (scoreText == null)
        {
            Debug.LogError("[ScoreUI] scoreText is NULL!");
            return;
        }

        scoreText.text = "Score: " + score;

        Debug.Log("[ScoreUI] UI updated to: " + scoreText.text);
    }
}