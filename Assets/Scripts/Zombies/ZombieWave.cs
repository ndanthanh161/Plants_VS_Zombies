using UnityEngine;
[System.Serializable]
public class ZombieSpawnChance
{
    public ZombieData zombieData;
    [Range(1, 100)]
    public int weight;
}


[System.Serializable]
public class ZombieWave
{
    public float duration = 20f;
    public float spawnInterval = 3f;
    public float intervalRandomOffset = 1f;

    public ZombieSpawnChance[] zombieChances;

    public bool isHugeWave;
}
