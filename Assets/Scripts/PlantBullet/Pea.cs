using UnityEngine;

public class Pea : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 5f;

    private int damage;
    private bool hasHit = false;   // 🔥 thêm biến này

    public void Init(int dmg)
    {
        damage = dmg;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;   // 🔥 chặn hit nhiều lần
        if (!other.CompareTag("Zombie")) return;

        ZombieHealth zombie = other.GetComponentInParent<ZombieHealth>();
        if (zombie != null)
        {
            hasHit = true;    // 🔥 đánh dấu đã hit
            zombie.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
