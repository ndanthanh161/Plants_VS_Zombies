using UnityEngine;

public static class LevelProgressManager
{
    const string KEY = "UnlockedLevel";

    public static int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt(KEY, 1);
    }

    public static void UnlockNextLevel(int currentLevel)
    {
        int unlocked = GetUnlockedLevel();

        if (currentLevel >= unlocked)
        {
            PlayerPrefs.SetInt(KEY, currentLevel + 1);
            PlayerPrefs.Save();
        }
    }
}
