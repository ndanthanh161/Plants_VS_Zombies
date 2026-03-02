using TMPro;
using UnityEngine;

public class SunManager : MonoBehaviour
{
    public static SunManager Instance;

    public int currentSun = 150;
    public TextMeshProUGUI sunText;

    [Header("Audio")]
    [Tooltip("Tiếng khi click nhặt mặt trời")]
    public AudioClip sunPickupSound;

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

        // Phát tiếng nhặt mặt trời
        if (sunPickupSound != null)
            AudioManager.GetInstance().PlaySound(sunPickupSound);
    }

    void UpdateUI()
    {
        if (sunText != null)
            sunText.text = currentSun.ToString();
    }
}
