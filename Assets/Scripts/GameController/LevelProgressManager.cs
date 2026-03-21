using UnityEngine;

public static class LevelProgressManager
{
    const string LEVEL_KEY = "UnlockedLevel";
    const string SCORE_KEY = "TotalHighScore";

    public static int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt(LEVEL_KEY, 1);
    }

    public static void UnlockNextLevel(int currentLevel)
    {
        int unlocked = GetUnlockedLevel();
        if (currentLevel >= unlocked)
        {
            PlayerPrefs.SetInt(LEVEL_KEY, currentLevel + 1);
            PlayerPrefs.Save();
        }
    }

    public static int GetTotalHighScore()
    {
        return PlayerPrefs.GetInt(SCORE_KEY, 0);
    }

    public static void SaveHighScore(int newScore)
    {
        int currentHigh = GetTotalHighScore();
        if (newScore > currentHigh)
        {
            PlayerPrefs.SetInt(SCORE_KEY, newScore);
            PlayerPrefs.Save();
        }
    }
}
