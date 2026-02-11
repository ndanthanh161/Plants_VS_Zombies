using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    int damage;
    float attackInterval;

    float timer;
    Plant target;
    ZombieMovement movement;

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

    void Update()
    {
        if (target == null)
        {
            movement.SetEating(false);
            return;
        }

        // 🔥 nếu cây đã bị destroy
        if (target.gameObject == null)
        {
            target = null;
            movement.SetEating(false);
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
        if (!other.CompareTag("Plant")) return;

        Plant plant = other.GetComponent<Plant>();
        if (plant == null) return;

        target = plant;
        timer = 0f; // reset timer khi bắt đầu cắn
        movement.SetEating(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Plant")) return;

        if (other.GetComponent<Plant>() == target)
        {
            target = null;
            movement.SetEating(false);
        }
    }
}
