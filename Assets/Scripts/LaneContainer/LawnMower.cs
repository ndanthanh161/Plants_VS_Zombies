using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class LawnMower : MonoBehaviour
{
    public int rowIndex;
    public float moveSpeed = 8f;

    private bool activated = false;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    void Update()
    {
        if (activated)
        {
            rb.linearVelocity = Vector2.right * moveSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ZombieController zombie = other.GetComponent<ZombieController>();

        if (zombie == null)
            return;

        if (zombie.rowIndex != rowIndex)
            return;

        Activate();

        ZombieHealth zh = zombie.GetComponent<ZombieHealth>();
        if (zh != null)
            zh.TakeDamage(99999);
    }

    void Activate()
    {
        if (activated)
            return;

        activated = true;
    }

    private void OnBecameInvisible()
    {
        if (activated)
            Destroy(gameObject);
    }
}
