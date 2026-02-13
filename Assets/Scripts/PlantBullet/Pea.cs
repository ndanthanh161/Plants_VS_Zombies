using UnityEngine;

public class Pea : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 5f;

    private int damage;
    private bool hasHit = false;

    private bool isFire = false;     // 🔥 thêm
    public int fireMultiplier = 2;   // 🔥 nhân damage

    public Sprite normalSprite;      // 🔥 sprite thường
    public Sprite fireSprite;        // 🔥 sprite lửa

    private SpriteRenderer sr;

    public void Init(int dmg)
    {
        damage = dmg;
        sr = GetComponent<SpriteRenderer>();
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    public void TurnIntoFire()
    {
        if (isFire) return;   // 🔥 chỉ buff 1 lần

        isFire = true;
        damage *= fireMultiplier;

        if (sr != null && fireSprite != null)
            sr.sprite = fireSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        // 🔥 nếu va vào TorchPlant thì buff
        if (other.CompareTag("Torch"))
        {
            TurnIntoFire();
            return;
        }

        if (!other.CompareTag("Zombie")) return;

        ZombieHealth zombie = other.GetComponentInParent<ZombieHealth>();
        if (zombie != null)
        {
            hasHit = true;
            zombie.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
