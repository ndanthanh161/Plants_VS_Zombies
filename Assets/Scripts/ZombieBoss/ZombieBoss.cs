using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component gắn thêm vào Prefab ZombieBoss (cùng với ZombieController, ZombieHealth, v.v.)
/// Quản lý Phase 2 và thanh máu Boss trên UI.
/// </summary>
[RequireComponent(typeof(ZombieHealth))]
[RequireComponent(typeof(ZombieMovement))]
[RequireComponent(typeof(ZombieAttack))]
public class ZombieBoss : MonoBehaviour
{
    [Header("UI - Health Bar")]
    [Tooltip("Prefab thanh máu boss (spawn vào Canvas)")]
    public GameObject bossHealthBarPrefab;

    // Runtime references
    private ZombieHealth health;
    private ZombieMovement movement;
    private ZombieAttack attack;
    private ZombieBossData bossData;

    private bool isPhase2 = false;
    private int maxHP;

    // UI
    private GameObject healthBarInstance;
    private Slider hpSlider;
    private Text bossNameText;

    void Awake()
    {
        health = GetComponent<ZombieHealth>();
        movement = GetComponent<ZombieMovement>();
        attack = GetComponent<ZombieAttack>();
    }

    /// <summary>Gọi sau ZombieController.Init() để khởi tạo boss-specific logic</summary>
    public void InitBoss(ZombieBossData data)
    {
        bossData = data;
        maxHP = data.maxHp;

        SpawnHealthBar();

        // Theo dõi damage để cập nhật HP bar và kiểm tra Phase 2
        health.OnDeath += _ => DespawnHealthBar();
    }

    void Update()
    {
        if (bossData == null || health == null) return;

        UpdateHealthBar();
        CheckPhase2();
    }

    // ===== Phase 2 =====

    void CheckPhase2()
    {
        if (isPhase2) return;

        float hpPercent = GetHPPercent();
        if (hpPercent <= bossData.phase2Threshold)
        {
            EnterPhase2();
        }
    }

    void EnterPhase2()
    {
        isPhase2 = true;
        Debug.Log($"[Boss] {bossData.bossName} enters PHASE 2!");

        // Tăng tốc độ
        if (movement != null)
            movement.SetSpeedMultiplier(bossData.phase2SpeedMultiplier);

        // Tăng sát thương
        if (attack != null)
            attack.SetDamageMultiplier(bossData.phase2DamageMultiplier);

        // Đổi màu sprite để báo hiệu phase 2
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.color = bossData.phase2Color;
    }

    // ===== Health Bar UI =====

    void SpawnHealthBar()
    {
        if (bossHealthBarPrefab == null) return;

        // Tìm Canvas trong scene để spawn UI vào
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        healthBarInstance = Instantiate(bossHealthBarPrefab, canvas.transform);
        hpSlider = healthBarInstance.GetComponentInChildren<Slider>();
        bossNameText = healthBarInstance.GetComponentInChildren<Text>();

        if (hpSlider != null)
        {
            hpSlider.minValue = 0f;
            hpSlider.maxValue = 1f;
            hpSlider.value = 1f;
        }

        if (bossNameText != null)
            bossNameText.text = bossData.bossName;
    }

    void UpdateHealthBar()
    {
        if (hpSlider == null) return;
        hpSlider.value = GetHPPercent();
    }

    void DespawnHealthBar()
    {
        if (healthBarInstance != null)
            Destroy(healthBarInstance);
    }

    float GetHPPercent()
    {
        // Lấy % HP từ ZombieHealth (cần expose property CurrentHP)
        int current = health.CurrentHP;
        return (float)current / maxHP;
    }

    void OnDestroy()
    {
        DespawnHealthBar();
    }
}
