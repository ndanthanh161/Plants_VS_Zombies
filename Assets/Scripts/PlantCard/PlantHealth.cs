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
        Debug.Log($"[PlantHealth] {gameObject.name} bị trừ {damage} máu. Máu còn lại: {currentHP}");

        if (currentHP <= 0)
        {
            Debug.Log($"[PlantHealth] {gameObject.name} ĐÃ CHẾT");
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
