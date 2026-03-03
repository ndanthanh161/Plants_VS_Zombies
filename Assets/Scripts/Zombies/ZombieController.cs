using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public ZombieData data;
    public int rowIndex;

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

    public void Init(ZombieData zombieData, int laneIndex)
    {
        data = zombieData;
        rowIndex = laneIndex;

        if (movement != null)
            movement.Init(data);

        if (attack != null)
            attack.Init(data);

        if (visual != null)
            visual.Init(data);

        if (health != null)
            health.Init(data);

        // Nếu là Boss → khởi tạo thêm boss logic
        ZombieBoss boss = GetComponent<ZombieBoss>();
        if (boss != null && data is ZombieBossData bossData)
        {
            boss.InitBoss(bossData);
        }
    }
}
