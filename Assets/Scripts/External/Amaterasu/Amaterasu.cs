using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmaterasuSkill : MonoBehaviour
{
    public static AmaterasuSkill Instance;

    [Header("Cài đặt Skill")]
    public int sunCost = 300;
    public float cooldownTime = 45f;
    public float activationDelay = 1.2f;
    public int damage = 99999;

    [Header("Hiệu ứng (Cinematic)")]
    [Tooltip("Prefab hiệu ứng lửa đen (Sẽ hiển thị ngay giữa màn hình như bản gốc)")]
    public GameObject fireEffectPrefab;
    public float effectDuration = 3f;
    public float cameraShakeIntensity = 0.1f;
    
    [Tooltip("Kích hoạt ngưng đọng thời gian (Bullet-time) khi gồng ấn")]
    public bool useSlowMotion = true;
    [Tooltip("Mức độ làm chậm (0.2 là siêu chậm, 1 là bình thường)")]
    public float slowMotionScale = 0.2f;

    [Header("Audio")]
    public AudioClip activationSound;

    [Header("UI References")]
    public Button skillButton;
    public Image cooldownOverlay;
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

        if (SunManager.Instance == null || !SunManager.Instance.CanAfford(sunCost)) return;

        SunManager.Instance.SpendSun(sunCost);

        StartCoroutine(ActivateAmaterasu());
    }

    IEnumerator ActivateAmaterasu()
    {
        isActivating = true;

        if (activationSound != null)
            AudioManager.GetInstance().PlaySound(activationSound);

        if (useSlowMotion)
        {
            Time.timeScale = slowMotionScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale; 
        }

        StartCoroutine(DarkenScreenRoutine(activationDelay));
        StartCoroutine(CameraShakeRoutine(activationDelay + effectDuration, cameraShakeIntensity));

        // Đợi theo thời gian thực (realtime)
        yield return new WaitForSecondsRealtime(activationDelay);

        if (useSlowMotion)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }

        // Đã xóa hiệu ứng nhá màn hình đỏ theo yêu cầu

        // GIỮ NGUYÊN CƠ CHẾ SPAWN GIỮA BẢN ĐỒ CỦA BẢN GỐC
        SpawnFireEffect();

        // GIỮ NGUYÊN CƠ CHẾ XÓA SỔ ZOMBIE VÀ THỰC VẬT CỦA BẢN GỐC
        ZombieHealth[] allZombies = FindObjectsByType<ZombieHealth>(FindObjectsSortMode.None);
        foreach (ZombieHealth zombie in allZombies)
        {
            if (zombie != null)
                zombie.TakeDamage(damage);
        }
        
        PlantHealth[] allPlants = FindObjectsByType<PlantHealth>(FindObjectsSortMode.None);
        foreach (PlantHealth plant in allPlants)
        {
            if (plant != null)
                plant.TakeDamage(damage);
        }

        Cell[] allCells = FindObjectsByType<Cell>(FindObjectsSortMode.None);
        foreach (Cell cell in allCells)
        {
            if (cell != null)
                cell.isOccupied = false;
        }

        Debug.Log($"[Amaterasu] Đã thiêu rụi {allZombies.Length} zombie và {allPlants.Length} cây!");

        isCoolingDown = true;
        cooldownTimer = cooldownTime;
        isActivating = false;

        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(true);
            cooldownOverlay.fillAmount = 1f;
        }
    }

    // Cơ chế gốc: Đẻ 1 cục lửa khổng lồ giữa bản đồ
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

    IEnumerator DarkenScreenRoutine(float delayBeforeHit)
    {
        GameObject canvasObj = new GameObject("AmaterasuDarkOverlay");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; 

        Image img = canvasObj.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0);

        float elapsed = 0f;
        while (elapsed < delayBeforeHit)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 0.85f, elapsed / delayBeforeHit); 
            img.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(effectDuration);

        elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0.85f, 0f, elapsed / 1f);
            img.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        Destroy(canvasObj);
    }

    IEnumerator CameraShakeRoutine(float duration, float magnitude)
    {
        if (Camera.main == null) yield break;
        Vector3 originalPos = Camera.main.transform.localPosition;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            Camera.main.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
    }
}
