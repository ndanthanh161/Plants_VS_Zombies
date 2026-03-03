using System.Collections;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    int damage;
    float attackInterval;

    float timer;
    Plant target;
    ZombieMovement movement;

    [Header("Audio")]
    [Tooltip("Tiếng zombie ăn cây")]
    public AudioClip eatSound;
    [Tooltip("Thời gian (giây) giữa các lần phát tiếng ăn")]
    public float eatSoundInterval = 1.2f;

    private Coroutine eatSoundCoroutine;

    void Awake()
    {
        movement = GetComponent<ZombieMovement>();

        if (movement == null)
            Debug.LogError("ZombieMovement missing!", gameObject);
    }

    public void Init(ZombieData data)
    {
        damage = data.damage;
        attackInterval = data.attackInterval;
    }

    /// <summary>Nhân sát thương hiện tại theo hệ số. Boss dùng khi vào Phase 2.</summary>
    public void SetDamageMultiplier(float multiplier)
    {
        damage = Mathf.RoundToInt(damage * multiplier);
    }

    void Update()
    {
        if (target == null)
        {
            movement.SetEating(false);
            StopEatSound();
            return;
        }

        // 🔥 nếu cây đã bị destroy
        if (target.gameObject == null)
        {
            target = null;
            movement.SetEating(false);
            StopEatSound();
            return;
        }

        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            target.TakeDamage(damage);
            timer = 0f;
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Plant") && !other.CompareTag("Torch"))
            return;

        Plant plant = other.GetComponent<Plant>();
        if (plant == null) return;

        target = plant;
        timer = 0f;
        movement.SetEating(true);
        StartEatSound(); // Bắt đầu phát tiếng ăn
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Plant") && !other.CompareTag("Torch"))
            return;

        if (other.GetComponent<Plant>() == target)
        {
            target = null;
            movement.SetEating(false);
            StopEatSound(); // Dừng tiếng ăn
        }
    }

    // ===== Phát tiếng ăn lặp định kỳ =====
    void StartEatSound()
    {
        if (eatSound == null) return;
        if (eatSoundCoroutine != null) StopCoroutine(eatSoundCoroutine);
        eatSoundCoroutine = StartCoroutine(EatSoundLoop());
    }

    void StopEatSound()
    {
        if (eatSoundCoroutine != null)
        {
            StopCoroutine(eatSoundCoroutine);
            eatSoundCoroutine = null;
        }

        // Dừng ngay clip đang phát dở, không để chạy tiếp
        if (eatSound != null)
            AudioManager.GetInstance().StopSoundByClip(eatSound);
    }

    IEnumerator EatSoundLoop()
    {
        while (true)
        {
            AudioManager.GetInstance().PlaySoundOnce(eatSound); // Không chồng nhau dù nhiều zombie
            yield return new WaitForSeconds(eatSoundInterval);
        }
    }

    void OnDestroy()
    {
        StopEatSound();
    }
}

