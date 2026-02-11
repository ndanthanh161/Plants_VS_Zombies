using UnityEngine;

public class PlantAttack : MonoBehaviour
{
    public Transform shootPoint;
    public GameObject peaPrefab;

    private float attackTimer;
    private PlantData data;

    private float laneY;
    private float laneTolerance = 0.2f;

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
        GameObject pea = Instantiate(peaPrefab, shootPoint.position, Quaternion.identity);

        Pea peaScript = pea.GetComponent<Pea>();
        if (peaScript != null)
        {
            peaScript.Init(data.damage);
        }
    }
}
