using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmaterasuSkill : MonoBehaviour
{
    public static AmaterasuSkill Instance;

    [Header("Cài đặt Skill")]
    [Tooltip("Giá sun để dùng skill")]
    public int sunCost = 300;

    [Tooltip("Cooldown sau mỗi lần dùng (giây)")]
    public float cooldownTime = 45f;

    [Tooltip("Delay trước khi hiệu ứng kích hoạt (giây)")]
    public float activationDelay = 1.0f;

    [Tooltip("Sát thương (đặt cực cao để giết ngay)")]
    public int damage = 99999;

    [Header("Hiệu ứng")]
    [Tooltip("Prefab hiệu ứng lửa đen (tùy chọn)")]
    public GameObject fireEffectPrefab;

    [Tooltip("Thời gian hiệu ứng tồn tại")]
    public float effectDuration = 3f;

    [Header("Audio")]
    public AudioClip activationSound;

    [Header("UI References")]
    [Tooltip("Nút bấm kích hoạt skill")]
    public Button skillButton;

    [Tooltip("Overlay cooldown (Image fillAmount)")]
    public Image cooldownOverlay;

    [Tooltip("Text hiển thị giá sun")]
    public TextMeshProUGUI costText;

    private bool isCoolingDown = false;
    private float cooldownTimer = 0f;
    private bool isActivating = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (skillButton != null)
            skillButton.onClick.AddListener(OnSkillButtonClicked);

        if (costText != null)
            costText.text = sunCost.ToString();

        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f;
            cooldownOverlay.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Xử lý cooldown
        if (isCoolingDown)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = cooldownTimer / cooldownTime;

            if (cooldownTimer <= 0f)
            {
                isCoolingDown = false;
                if (cooldownOverlay != null)
                    cooldownOverlay.gameObject.SetActive(false);
            }
        }

        // Cập nhật trạng thái nút
        if (skillButton != null)
        {
            bool canUse = !isCoolingDown
                          && !isActivating
                          && SunManager.Instance != null
                          && SunManager.Instance.CanAfford(sunCost);

            skillButton.interactable = canUse;
        }
    }

    void OnSkillButtonClicked()
    {
        if (isCoolingDown || isActivating) return;

        // Check tiền
        if (SunManager.Instance == null || !SunManager.Instance.CanAfford(sunCost))
        {
            Debug.Log("[Amaterasu] Không đủ sun!");
            return;
        }

        // Trừ tiền
        SunManager.Instance.SpendSun(sunCost);

        // Bắt đầu kích hoạt
        StartCoroutine(ActivateAmaterasu());
    }

    IEnumerator ActivateAmaterasu()
    {
        isActivating = true;

        // === Phát âm thanh ===
        if (activationSound != null)
            AudioManager.GetInstance().PlaySound(activationSound);

        // === Delay trước khi gây damage ===
        yield return new WaitForSeconds(activationDelay);

        // === Spawn hiệu ứng lửa đen ===
        SpawnFireEffect();

        // === Giết tất cả Zombie ===
        ZombieHealth[] allZombies = FindObjectsByType<ZombieHealth>(FindObjectsSortMode.None);
        foreach (ZombieHealth zombie in allZombies)
        {
            if (zombie != null)
                zombie.TakeDamage(damage);
        }
        Debug.Log($"[Amaterasu] Đã thiêu cháy {allZombies.Length} zombie!");

        // === Giết tất cả Plant ===
        PlantHealth[] allPlants = FindObjectsByType<PlantHealth>(FindObjectsSortMode.None);
        foreach (PlantHealth plant in allPlants)
        {
            if (plant != null)
                plant.TakeDamage(damage);
        }
        Debug.Log($"[Amaterasu] Đã thiêu cháy {allPlants.Length} cây trồng!");

        // === Reset tất cả ô đất ===
        Cell[] allCells = FindObjectsByType<Cell>(FindObjectsSortMode.None);
        foreach (Cell cell in allCells)
        {
            if (cell != null)
                cell.isOccupied = false;
        }
        Debug.Log("[Amaterasu] Đã reset tất cả ô đất!");

        // === Bắt đầu cooldown ===
        isCoolingDown = true;
        cooldownTimer = cooldownTime;
        isActivating = false;

        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(true);
            cooldownOverlay.fillAmount = 1f;
        }
    }

    void SpawnFireEffect()
    {
        if (fireEffectPrefab == null || GridManager.Instance == null) return;

        float centerX = GridManager.Instance.startPosition.x
            + (GridManager.Instance.columns - 1) * GridManager.Instance.cellWidth / 2f;
        float centerY = GridManager.Instance.startPosition.y
            - (GridManager.Instance.rows - 1) * GridManager.Instance.cellHeight / 2f;

        Vector3 center = new Vector3(centerX, centerY, -1f);

        GameObject effect = Instantiate(fireEffectPrefab, center, Quaternion.identity);
        Destroy(effect, effectDuration);
    }
}
