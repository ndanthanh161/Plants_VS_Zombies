using TMPro;
using UnityEngine;

public class SunManager : MonoBehaviour
{
    public static SunManager Instance;

    public int currentSun = 150;
    public TextMeshProUGUI sunText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public bool CanAfford(int cost)
    {
        return currentSun >= cost;
    }

    public void SpendSun(int cost)
    {
        currentSun -= cost;
        UpdateUI();
    }

    public void AddSun(int amount)
    {
        currentSun += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (sunText != null)
            sunText.text = currentSun.ToString();
    }
}
