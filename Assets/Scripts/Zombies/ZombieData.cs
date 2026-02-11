using UnityEngine;

[CreateAssetMenu(menuName = "PvZ/Zombie/Data")]
public class ZombieData : ScriptableObject
{
    [Header("Stat")]
    public float moveSpeed;
    public int maxHp;

    [Header("Attack")]
    public int damage;
    public float attackInterval;

    [Header("Visual")]
    public GameObject visualPrefab;
}
