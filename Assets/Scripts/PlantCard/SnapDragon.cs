using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// SnapDragon — bắn ra 1 cơn sóng lửa bay xa, gây sát thương xuyên thấu cho mọi zombie trên đường bay.
/// </summary>
public class SnapDragon : MonoBehaviour
{
    [Header("Cấu hình Cơn Sóng (Wave)")]
    [Tooltip("Quãng đường cơn sóng bay đi (tầm đánh xa tới đâu)")]
    public float waveDistance = 5f;
    [Tooltip("Khoảng thời gian (giây) để cơn sóng bay chạm mốc Distance ở trên (ví dụ 1.2 là khá nhanh)")]
    public float waveDuration = 1.2f;

    [Tooltip("Độ rộng của cơn sóng lửa (chỉnh to ra nếu muốn nó quét rộng hơn)")]
    public float waveWidth = 1f;
    [Tooltip("Độ cao của luồng sát thương (chỉnh 2f để chắc chắn bao trùm cả con zombie)")]
    public float waveHeight = 2f;

    [Tooltip("Điểm xuất phát lửa bay (cách tâm cây bao nhiêu)")]
    public float fireOffsetX = 0.5f;

    [Header("Attack Timing")]
    [Tooltip("Thời gian nghỉ bắn tối thiểu (giây)")]
    public float attackIntervalMin = 1.85f;
    [Tooltip("Thời gian nghỉ bắn tối đa (giây)")]
    public float attackIntervalMax = 2f;

    [Header("Hiệu ứng ngọn lửa (Chọn 1 trong 2)")]
    [Tooltip("Cách 1 (Khuyên dùng): Kéo thả PREFAB ngọn lửa (chỉ cần có Sprite hình lửa) vào đây!")]
    public GameObject wavePrefab;
    
    [Tooltip("Cách 2: Gắn GameObject con đã có sẵn trong Snapdragon vào đây.")]
    public GameObject fireEffect;

    [Header("Animation Settings")]
    [Tooltip("Bật nếu Animation có Event 'ShowFireEffect'. Tắt nếu tự bắn wave bỏ qua Event (vì anim quá ngắn).")]
    public bool useAnimationEvent = false;
    [Tooltip("Delay chờ mồm cây mở ra (Chỉ xài khi Tắt Animation Event ở trên)")]
    public float delayBeforeShoot = 0.3f;

    [Header("Audio")]
    public AudioClip attackSound;

    // ─── Private ────────────────────────────────────────
    private int damage = 30;
    private float attackTimer;
    private float laneY;
    private const float LaneTolerance = 1.1f;

    private Animator anim;
    private static readonly int BiteHash = Animator.StringToHash("Bite");
    
    private bool isAttacking = false;
    private Vector3 initialFireEffectLocalPos;

    // ────────────────────────────────────────────────────
    void Awake()
    {
        anim = GetComponent<Animator>();

        // Nếu họ dùng Cách 2 mà lười kéo, code tự mò GameObject con có tên "FireEffect"
        if (wavePrefab == null && fireEffect == null)
        {
            Transform found = transform.Find("FireEffect");
            if (found != null) fireEffect = found.gameObject;
        }

        // Tắt ngọn lửa con lúc đầu
        if (fireEffect != null)
        {
            Vector3 pos = fireEffect.transform.localPosition;
            initialFireEffectLocalPos = pos;
            fireEffect.SetActive(false);
        }

        ResetTimer();
    }

    public void Init(PlantData plantData)
    {
        if (plantData != null && plantData.damage > 0) damage = plantData.damage;
        laneY = transform.position.y;
        ResetTimer();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            TryAttack();
            ResetTimer(); 
        }
    }

    void ResetTimer()
    {
        attackTimer = Random.Range(attackIntervalMin, attackIntervalMax);
    }

    void TryAttack()
    {
        if (!HasZombieInRange()) return;
        if (isAttacking) return; // Đang bắn sóng thì khoan

        // 1. Kích hoạt múa mồm
        if (anim != null) anim.SetTrigger(BiteHash);

        // 2. Kêu éc éc
        if (attackSound != null) AudioManager.GetInstance().PlaySound(attackSound);

        // 3. Tự đẻ sóng lửa nếu không mượn Animation Event
        if (!useAnimationEvent)
        {
            StartCoroutine(ShootWaveTimer());
        }
    }

    bool HasZombieInRange()
    {
        ZombieHealth[] zombies = FindObjectsByType<ZombieHealth>(FindObjectsSortMode.None);

        foreach (ZombieHealth zombie in zombies)
        {
            if (zombie == null) continue;

            float zY = zombie.transform.position.y;
            if (Mathf.Abs(zY - laneY) <= LaneTolerance) 
            {
                float zX = zombie.transform.position.x;
                float startX = transform.position.x + fireOffsetX;
                float endX = startX + waveDistance;

                if (zX >= startX && zX <= endX) return true;
            }
        }
        return false;
    }

    IEnumerator ShootWaveTimer()
    {
        yield return new WaitForSeconds(delayBeforeShoot);
        ShowFireEffect();
    }

    // Được gọi bằng Unity Animation Event (Nếu useAnimationEvent = true) hoặc gọi tự động
    public void ShowFireEffect()
    {
        if (isAttacking) return;
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        isAttacking = true;
        HashSet<ZombieHealth> damagedZombies = new HashSet<ZombieHealth>();

        GameObject spawnedWave = null;
        Vector3 startPos = transform.position + new Vector3(fireOffsetX, 0, 0);

        // Chuẩn bị đạn hiển thị ra mắt
        if (wavePrefab != null)
        {
            // Cách 1: Spawn một Prefab Lửa ra
            spawnedWave = Instantiate(wavePrefab, startPos, Quaternion.identity);
            
            // Xếp lớp để nó luôn nằm đè lên cây
            SpriteRenderer srWave = spawnedWave.GetComponentInChildren<SpriteRenderer>();
            SpriteRenderer srPlant = GetComponent<SpriteRenderer>();
            if (srWave != null && srPlant != null)
            {
                srWave.sortingLayerName = srPlant.sortingLayerName;
                srWave.sortingOrder = srPlant.sortingOrder + 5;
            }
        }
        else if (fireEffect != null)
        {
            // Cách 2: Dùng object con bật lên
            fireEffect.SetActive(true);
            fireEffect.transform.position = startPos;
        }
        else
        {
            Debug.LogWarning("SnapDragon: Bạn BẮT BUỘC phải kéo thả file hình ngọn lửa vào 'Wave Prefab' ở Inspector nhé! Nếu không thì sẽ bắn sóng tàng hình đấy.");
        }

        float elapsed = 0f;
        
        while (elapsed < waveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / waveDuration;
            
            // Cập nhật vị trí sóng bay tới
            Vector3 currentPos = startPos + new Vector3(waveDistance * t, 0, 0);
            
            // Trượt con sóng trên màn hình
            if (spawnedWave != null)
            {
                spawnedWave.transform.position = new Vector3(currentPos.x, currentPos.y, -0.1f);
            }
            else if (fireEffect != null)
            {
                fireEffect.transform.position = new Vector3(currentPos.x, currentPos.y, -0.1f);
            }

            // --- VÙNG SÁT THƯƠNG ---
            // Nới rộng kích thước sóng lửa theo hình hộp quét qua zombie
            Collider2D[] hits = Physics2D.OverlapBoxAll(currentPos, new Vector2(waveWidth, waveHeight), 0f);

            foreach (Collider2D col in hits)
            {
                if (col.CompareTag("Zombie"))
                {
                    ZombieHealth zombie = col.GetComponentInParent<ZombieHealth>();
                    // Xuyên thấu: zombie nào bị nướng rồi thì tha, chưa bị thì gây sát thương và nhớ mặt
                    if (zombie != null && !damagedZombies.Contains(zombie))
                    {
                        zombie.TakeDamage(damage);
                        damagedZombies.Add(zombie); // Nhớ mặt
                    }
                }
            }

            yield return null; 
        }

        // Hủy hoặc ẩn lửa khi bay xong
        if (spawnedWave != null)
        {
            Destroy(spawnedWave);
        }
        else if (fireEffect != null)
        {
            fireEffect.SetActive(false);
            fireEffect.transform.localPosition = initialFireEffectLocalPos;
        }

        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 startPoint = new Vector3(transform.position.x + fireOffsetX, transform.position.y, 0f);
        Vector3 endPoint = startPoint + new Vector3(waveDistance, 0, 0);
        
        Gizmos.color = new Color(1f, 0.4f, 0f, 0.35f);
        Gizmos.DrawLine(startPoint, endPoint);
        Gizmos.DrawWireCube(startPoint, new Vector3(waveWidth, waveHeight, 0.1f));
        Gizmos.DrawWireCube(endPoint, new Vector3(waveWidth, waveHeight, 0.1f));

#if UNITY_EDITOR
        UnityEditor.Handles.Label(
            startPoint + Vector3.up * (waveHeight / 2f + 0.15f),
            $"🌊 Sóng đi xa {waveDistance} ô\nHitbox: {waveWidth}x{waveHeight}\nDmg: {damage}"
        );
#endif
    }
}
