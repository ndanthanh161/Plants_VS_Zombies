using UnityEngine;

public class PlantHealth : MonoBehaviour
{
    private int currentHP;
    private PlantData data;

    public void Init(PlantData plantData)
    {
        data = plantData;
        currentHP = data.maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
