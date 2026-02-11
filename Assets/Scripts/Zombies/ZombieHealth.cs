using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    int currentHP;

    public void Init(ZombieData data)
    {
        currentHP = data.maxHp;
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
