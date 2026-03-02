using UnityEngine;

public class PlantAttack : MonoBehaviour
{
    public Transform shootPoint;
    public GameObject peaPrefab;

    private float attackTimer;
    private PlantData data;

    private float laneY;
    private float laneTolerance = 0.2f;

    private Animator anim; // MỚI THÊM: Biến lưu trữ Animator

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
        // Đã xóa phần tạo viên đạn ở đây!
    }

    // 2. MỚI THÊM: Hàm này SẼ CHỈ CHẠY khi Animation chạm đến đúng khung hình
    // Lưu ý: Bắt buộc phải có chữ "public" ở đầu!
    public void SpawnPea()
    {
        GameObject pea = Instantiate(peaPrefab, shootPoint.position, Quaternion.identity);

        Pea peaScript = pea.GetComponent<Pea>();
        if (peaScript != null)
        {
            peaScript.Init(data.damage);
        }
    }
}