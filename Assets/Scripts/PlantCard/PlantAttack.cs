using UnityEngine;
using System.Collections;

public class PlantAttack : MonoBehaviour
{
    public Transform shootPoint;
    public GameObject peaPrefab;

    [Header("Cấu hình Tấn công (Burst)")]
    [Tooltip("Số viên đạn mỗi lần bắn (Ví dụ: 1 cho đậu thường, 4 cho đậu nhiều nòng)")]
    public int bulletCount = 1;
    [Tooltip("Thời gian chờ giữa 2 viên đạn trong cùng 1 lần bắn")]
    public float burstDelay = 0.15f;

    [Header("Cấu hình Animation")]
    [Tooltip("Bật nếu Animation của bạn được set Animation Event 'SpawnPea'. Tắt nếu tự bắn bỏ qua Event.")]
    public bool useAnimationEvent = true;
    [Tooltip("Delay chờ mồm cây mở ra (Chỉ xài khi Tắt Animation Event ở trên)")]
    public float delayBeforeShoot = 0.2f;

    private float attackTimer;
    private PlantData data;

    private float laneY;
    private float laneTolerance = 0.2f;

    private Animator anim; // MỚI THÊM: Biến lưu trữ Animator
    private bool isSpawningPea = false; // 🐛 Guard chống SpawnPea bị gọi 2 lần trong cùng 1 frame

    private void Awake()
    {
        // MỚI THÊM: Lấy component Animator đang gắn trên cây đậu này
        anim = GetComponent<Animator>();
    }

    public void Init(PlantData plantData)
    {
        data = plantData;
        attackTimer = data.attackInterval;

        laneY = transform.position.y;
    }

    void Update()
    {
        if (data == null) return;

        if (!HasZombieInLane())
            return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            Shoot();
            attackTimer = data.attackInterval;
        }
    }

    bool HasZombieInLane()
    {
        ZombieHealth[] zombies = FindObjectsByType<ZombieHealth>(FindObjectsSortMode.None);

        foreach (ZombieHealth zombie in zombies)
        {
            if (zombie == null) continue;

            float zombieY = zombie.transform.position.y;

            if (Mathf.Abs(zombieY - laneY) <= laneTolerance)
            {
                if (zombie.transform.position.x > transform.position.x)
                    return true;
            }
        }

        return false;
    }

    void Shoot()
    {
        // 1. Chỉ ra lệnh cho cây chạy Animation (bắt đầu giật đầu)
        if (anim != null)
        {
            anim.SetTrigger("Shoot");
        }

        // 2. Nếu không xài Event từ Unity Animation, lập tức tự khởi chạy luồng bắn
        if (!useAnimationEvent)
        {
            StartCoroutine(SpawnPeaBurstTimer());
        }
    }

    IEnumerator SpawnPeaBurstTimer()
    {
        yield return new WaitForSeconds(delayBeforeShoot);
        StartCoroutine(SpawnPeaBurst());
    }

    // 2. MỚI THÊM: Hàm này SẼ CHỈ CHẠY khi Animation chạm đến đúng khung hình
    // Lưu ý: Bắt buộc phải có chữ "public" ở đầu!
    public void SpawnPea()
    {
        // Nếu đã để tự động nhả đạn (useAnimationEvent tắt), thì bỏ qua lệnh này để tránh đạn bị đẻ ra gấp đôi
        if (!useAnimationEvent) return;

        // Guard: nếu frame này đã spawn rồi thì bỏ qua (chống Animation Event bị duplicate)
        if (isSpawningPea) return;
        isSpawningPea = true;

        StartCoroutine(SpawnPeaBurst());

        // Reset flag sau frame này
        Invoke(nameof(ResetSpawnGuard), 0.05f);
    }

    IEnumerator SpawnPeaBurst()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            if (this == null || gameObject == null) yield break;

            if (shootPoint != null && peaPrefab != null)
            {
                GameObject pea = Instantiate(peaPrefab, shootPoint.position, Quaternion.identity);
                Pea peaScript = pea.GetComponent<Pea>();
                if (peaScript != null)
                {
                    peaScript.Init(data.damage);
                }
            }
            else
            {
                Debug.LogWarning("PlantAttack: Hãy nhớ kéo ShootPoint và PeaPrefab vào Inspector cho cây này nhé!");
            }

            if (bulletCount > 1)
            {
                // Chờ khoảng giây trước khi tạo viên thứ 2, thứ 3,...
                yield return new WaitForSeconds(burstDelay);
            }
        }
    }

    void ResetSpawnGuard() => isSpawningPea = false;
}