using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlantCardUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI costText;
    public Button button;
    public Image cooldownOverlay;

    private PlantData data;
    private PlantCardManager manager;

    private float cooldownTimer = 0f;
    private bool isCoolingDown = false;
    [Header("Cooldown Settings")]
    [SerializeField] private float overrideCooldown = -1f;

    public void Setup(PlantData plantData, PlantCardManager cardManager)
    {
        data = plantData;
        manager = cardManager;

        icon.sprite = data.icon;
        costText.text = data.cost.ToString();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);

        if (cooldownOverlay != null)
            cooldownOverlay.gameObject.SetActive(false);
    }

    void Update()
    {
        if (data == null || SunManager.Instance == null)
            return;

        // Cooldown countdown
        if (isCoolingDown)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                isCoolingDown = false;
                if (cooldownOverlay != null)
                    cooldownOverlay.gameObject.SetActive(false);
            }
        }

        bool canAfford = SunManager.Instance.CanAfford(data.cost);
        bool canUse = canAfford && !isCoolingDown;

        button.interactable = canUse;

        Color c = icon.color;
        c.a = canUse ? 1f : 0.5f;
        icon.color = c;
    }

    void OnClick()
    {
        if (isCoolingDown)
            return;

        if (!SunManager.Instance.CanAfford(data.cost))
            return;

        manager.SelectPlant(data);
    }

    public void StartCooldown()
    {
        isCoolingDown = true;

        if (overrideCooldown > 0f)
            cooldownTimer = overrideCooldown;
        else
            cooldownTimer = data.cooldown;

        if (cooldownOverlay != null)
            cooldownOverlay.gameObject.SetActive(true);
    }

    public PlantData GetPlantData()
    {
        return data;
    }

}
