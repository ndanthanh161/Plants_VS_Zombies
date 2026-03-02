using UnityEngine;

public class PotatoMine : MonoBehaviour
{
    [Header("Cài đặt Mìn")]
    public float armTime = 3f;       // Thời gian chờ mìn trồi lên (Sẵn sàng nổ) - Đã chỉnh nhanh gấp 5 lần
    public int explosionDamage = 1800; // Sát thương nổ (Chết ngay 1-2 zombie)
    public float explosionRadius = 1.5f; // Bán kính vụ nổ (Mặc định bằng 1.5 ô vuông đất)

    // Tùy chọn: Thêm 2 hình ảnh mìn lúc lún dưới đất và lúc đã trồi lên
    public Sprite unArmedSprite;
    public Sprite armedSprite;

    [Header("Audio")]
    [Tooltip("Tiếng phát nổ khi mìn chạm zombie")]
    public AudioClip explosionSound;

    private bool isArmed = false;     // Trạng thái đã sẵn sàng chưa
    private float timer = 0f;
    private SpriteRenderer sr;
    private PlantHealth health;
    private Animator anim;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        health = GetComponent<PlantHealth>();
        anim = GetComponent<Animator>();

        // Mới đặt xuống thì dùng hình ảnh ẩn dưới đất
        if (sr != null && unArmedSprite != null)
            sr.sprite = unArmedSprite;
    }

    void Update()
    {
        if (isArmed) return;

        timer += Time.deltaTime;
        if (timer >= armTime)
        {
            ArmMine(); // Trồi mìn lên sau 14 giây
        }
    }

    void ArmMine()
    {
        isArmed = true;

        if (anim != null)
        {
            // Báo cho Animator biết Mìn đã vào tư thế sẵn sàng chờ nổ
            anim.SetBool("IsArmed", true);
        }
        else
        {
            // Dự phòng nếu lỗi không gắn Animator thì vẫn đổi ảnh tĩnh
            if (sr != null && armedSprite != null)
                sr.sprite = armedSprite;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isArmed) return;

        if (other.CompareTag("Zombie"))
        {
            ZombieHealth zombie = other.GetComponentInParent<ZombieHealth>();
            if (zombie != null)
            {
                // Khi chạm mảng Zombie, chuyển sang kích nổ
                if (anim != null)
                {
                    anim.SetTrigger("Explode");
                }
                else
                {
                    Explode(); // Dự phòng nếu ko có Animation thì nổ luôn lập tức
                }
            }
        }
    }

    void Explode()
    {
        // 1. Gây sát thương nổ theo hình hộp bọc quanh củ khoai tây (explosionRadius)
        Collider2D[] hitZombies = Physics2D.OverlapBoxAll(transform.position, new Vector2(explosionRadius, explosionRadius), 0f);
        foreach (var hit in hitZombies)
        {
            if (hit.CompareTag("Zombie"))
            {
                ZombieHealth zHealth = hit.GetComponent<ZombieHealth>();
                if (zHealth != null)
                {
                    zHealth.TakeDamage(explosionDamage);
                }
            }
        }

        // 2. Phát tiếng nổ
        if (explosionSound != null)
            AudioManager.GetInstance().PlaySound(explosionSound);

        // 3. Tự xoá bản thân củ mìn đi
        Destroy(gameObject);
    }

    // Phần này giúp bạn hiển thị phạm vi vụ nổ bằng hình vuông Đỏ ở cửa sổ Scene khi chọn Củ Mìn (Giúp bạn dễ canh xem nó nổ mấy ô đất)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(explosionRadius, explosionRadius, 1f));
    }
}
