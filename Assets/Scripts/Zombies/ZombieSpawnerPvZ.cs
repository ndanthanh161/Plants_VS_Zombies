using System.Collections.Generic;
using UnityEngine;

public class ZombieWaveSpawner : MonoBehaviour
{
    public ZombieWaveLevelData levelData;
    public GameObject zombiePrefab;
    public LaneManager laneManager;
    public float zombieSpawnX = 10f;

    int currentWaveIndex;
    float waveTimer;
    float spawnTimer;

    List<int> availableLanes = new List<int>();

    void Start()
    {
        InitLanes();
    }

    void Update()
    {
        if (currentWaveIndex >= levelData.waves.Length)
            return;

        ZombieWave wave = levelData.waves[currentWaveIndex];

        waveTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        float realInterval =
            wave.spawnInterval +
            Random.Range(-wave.intervalRandomOffset, wave.intervalRandomOffset);

        if (spawnTimer >= realInterval)
        {
            SpawnZombie(wave);
            spawnTimer = 0f;
        }

        if (waveTimer >= wave.duration)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        currentWaveIndex++;
        waveTimer = 0f;
        spawnTimer = 0f;

        Debug.Log(
            currentWaveIndex < levelData.waves.Length
            ? $"Wave {currentWaveIndex + 1} start"
            : "All waves completed"
        );
    }

    void InitLanes()
    {
        availableLanes.Clear();

        if (laneManager == null) return;

        for (int i = 0; i < laneManager.transform.childCount; i++)
            availableLanes.Add(i);
    }

    void SpawnZombie(ZombieWave wave)
    {
        if (availableLanes.Count == 0)
            InitLanes();

        int laneIndex = availableLanes[Random.Range(0, availableLanes.Count)];
        availableLanes.Remove(laneIndex);

        Transform lane = laneManager.transform.GetChild(laneIndex);

        ZombieData data = GetRandomZombieByWeight(wave.zombieChances);

        Vector3 pos = new Vector3(zombieSpawnX, lane.position.y, 0);

        GameObject zombie = Instantiate(zombiePrefab, pos, Quaternion.identity, transform);

        ZombieController controller = zombie.GetComponent<ZombieController>();
        if (controller != null)
            controller.Init(data);

        if (wave.isHugeWave)
            SpawnExtraZombie(wave, laneIndex);
    }

    void SpawnExtraZombie(ZombieWave wave, int laneIndex)
    {
        Transform lane = laneManager.transform.GetChild(laneIndex);

        ZombieData data = GetRandomZombieByWeight(wave.zombieChances);

        Vector3 pos = new Vector3(
            zombieSpawnX + Random.Range(0.5f, 1.5f),
            lane.position.y,
            0
        );

        GameObject zombie = Instantiate(zombiePrefab, pos, Quaternion.identity, transform);

        ZombieController controller = zombie.GetComponent<ZombieController>();
        if (controller != null)
            controller.Init(data);
    }

    ZombieData GetRandomZombieByWeight(ZombieSpawnChance[] chances)
    {
        int totalWeight = 0;

        for (int i = 0; i < chances.Length; i++)
            totalWeight += chances[i].weight;

        int random = Random.Range(0, totalWeight);
        int current = 0;

        for (int i = 0; i < chances.Length; i++)
        {
            current += chances[i].weight;

            if (random < current)
                return chances[i].zombieData;
        }

        return chances[0].zombieData;
    }
}
