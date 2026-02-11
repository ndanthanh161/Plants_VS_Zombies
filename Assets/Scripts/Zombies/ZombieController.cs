using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public ZombieData data;

    ZombieMovement movement;
    ZombieAttack attack;
    ZombieVisual visual;
    ZombieHealth health;

    void Awake()
    {
        movement = GetComponent<ZombieMovement>();
        attack = GetComponent<ZombieAttack>();
        visual = GetComponent<ZombieVisual>();
        health = GetComponent<ZombieHealth>();
    }

    public void Init(ZombieData zombieData)
    {
        data = zombieData;

        if (movement != null)
            movement.Init(data);

        if (attack != null)
            attack.Init(data);

        if (visual != null)
            visual.Init(data);

        if (health != null)
            health.Init(data);
    }
}
