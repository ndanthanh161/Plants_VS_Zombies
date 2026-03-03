using System;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    int currentHP;
    int scoreValue;
    ZombieVisual visual;
    bool isDead = false;

    public event Action<ZombieHealth> OnDeath;

    public int ScoreValue => scoreValue;
    public int CurrentHP => currentHP;  // Cho ZombieBoss đọc HP hiện tại

    void Awake()
    {
        visual = GetComponent<ZombieVisual>();
    }

    public void Init(ZombieData data)
    {
        currentHP = data.maxHp;
        scoreValue = data.scoreValue;   // 🔥 lấy điểm từ data
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Đổi tag để các loại đạn/cây/chức năng khác không target nhầm vào Zombie đã chết
        gameObject.tag = "Untagged";
        gameObject.layer = 0;

        // Tắt hết collider để không va chạm thêm với đạn/cây nào nữa
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // Gọi animation chết bằng cách lấy trực tiếp lại nếu cần bảo đạm
        if (visual != null)
        {
            Animator an = GetComponentInChildren<Animator>();
            if (an != null)
            {
                an.SetTrigger("IsDead");
            }
            else
            {
                Debug.LogWarning("Khong tim thay Animator! Zombie se bien mat.");
            }
        }

        // Tắt khả năng đi và cắn
        var mov = GetComponent<ZombieMovement>();
        if (mov != null) mov.enabled = false;

        var atk = GetComponent<ZombieAttack>();
        if (atk != null) atk.enabled = false;

        // Truyền event ra ngoài (chủ yếu báo cho WaveSpawner biết đã chết để đếm số lượng)
        OnDeath?.Invoke(this);

        // Huỷ event ngay sau khi gọi để tránh việc một thành phần khác xóa nhầm khi nó nhận lệnh nhiều lần
        OnDeath = null;

        // Đẩy xuống cuối sorting order (hoặc lùi lại z) để xác zombie nằm dưới các con zombie khác đang sống
        transform.SetAsFirstSibling();

        // Delay 1.5 - 2s (tuỳ độ dài animation Die) rồi mới xóa bỏ GameObject khỏi scene
        Destroy(gameObject, 1.5f);
    }
}
