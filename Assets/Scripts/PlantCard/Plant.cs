using UnityEngine;

public class Plant : MonoBehaviour
{
    private PlantData data;

    private PlantHealth health;
    private PlantAttack attack;
    private SnapDragon  snapDragon;

    void Awake()
    {
        health     = GetComponent<PlantHealth>();
        attack     = GetComponent<PlantAttack>();
        snapDragon = GetComponent<SnapDragon>();
    }

    public void Init(PlantData plantData)
    {
        data = plantData;

        if (health != null)
            health.Init(data);

        if (attack != null)
            attack.Init(data);

        if (snapDragon != null)
            snapDragon.Init(data);
    }

    public void TakeDamage(int damage)
    {
        if (health != null)
            health.TakeDamage(damage);
    }
}
