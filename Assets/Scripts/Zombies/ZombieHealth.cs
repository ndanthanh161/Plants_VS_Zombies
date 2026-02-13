using System;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    int currentHP;
    int scoreValue;

    public event Action<ZombieHealth> OnDeath;

    public int ScoreValue => scoreValue;   // 🔥 THÊM DÒNG NÀY

    public void Init(ZombieData data)
    {
        currentHP = data.maxHp;
        scoreValue = data.scoreValue;   // 🔥 lấy điểm từ data
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
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
