using UnityEngine;

public class ZombieWaveSpawner : MonoBehaviour
{
    public ZombieWaveLevelData levelData;
    public GameObject zombiePrefab;
    public LaneManager laneManager;
    public float zombieSpawnX = 10f;

    int currentWaveIndex;
    float spawnTimer;
    float currentSpawnInterval;

    int spawnedInWave;
    int aliveZombies;


    void Start()
    {
        CalculateNextInterval();
    }

    void Update()
    {
        if (levelData == null || laneManager == null)
            return;

        if (currentWaveIndex >= levelData.waves.Length)
            return;

        ZombieWave wave = levelData.waves[currentWaveIndex];

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentSpawnInterval && spawnedInWave < wave.zombieCount)
        {
            int spawnAmount = GetSpawnAmount(wave);

            for (int i = 0; i < spawnAmount; i++)
            {
                if (spawnedInWave < wave.zombieCount)
                    SpawnZombie(wave);
            }

            spawnTimer = 0f;
            CalculateNextInterval();
        }
    }

    void CalculateNextInterval()
    {
        ZombieWave wave = levelData.waves[currentWaveIndex];

        float progress = (float)spawnedInWave / wave.zombieCount;

        float curve = progress * progress;

        float dynamicInterval = Mathf.Lerp(
            wave.spawnInterval,
            wave.spawnInterval * 0.3f,
            curve
        );

        currentSpawnInterval =
            dynamicInterval +
            Random.Range(-wave.intervalRandomOffset, wave.intervalRandomOffset);

        if (currentSpawnInterval < 0.2f)
            currentSpawnInterval = 0.2f;
    }

    int GetSpawnAmount(ZombieWave wave)
    {
        float progress = (float)spawnedInWave / wave.zombieCount;

        if (wave.isHugeWave)
            return Random.Range(2, 4);

        if (progress > 0.75f)
            return Random.Range(2, 4);

        if (progress > 0.4f)
            return Random.value < 0.4f ? 2 : 1;

        return 1;
    }

    void SpawnZombie(ZombieWave wave)
    {
        int laneCount = laneManager.grid.rows;

        int laneIndex = Random.Range(0, laneCount);

        float yPos = laneManager.grid.startPosition.y
                     - laneIndex * laneManager.grid.cellHeight;

        ZombieData data = GetRandomZombieByWeight(wave.zombieChances);

        Vector3 pos = new Vector3(zombieSpawnX, yPos, 0);

        GameObject zombie = Instantiate(
            zombiePrefab,
            pos,
            Quaternion.identity,
            transform
        );

        ZombieController controller = zombie.GetComponent<ZombieController>();
        if (controller != null)
            controller.Init(data, laneIndex);

        ZombieHealth health = zombie.GetComponent<ZombieHealth>();
        if (health != null)
            health.OnDeath += OnZombieKilled;

        spawnedInWave++;
        aliveZombies++;
    }



    void OnZombieKilled(ZombieHealth health)
    {
        aliveZombies--;

        if (health != null)
        {
            health.OnDeath -= OnZombieKilled;

            GameManager.Instance.AddScore(health.ScoreValue);
        }
        if (currentWaveIndex >= levelData.waves.Length)
            return;

        ZombieWave wave = levelData.waves[currentWaveIndex];

        if (spawnedInWave >= wave.zombieCount && aliveZombies <= 0)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= levelData.waves.Length)
        {
            Debug.Log("YOU WIN");

            GameManager.Instance.WinGame();   // 🔥 thêm cái này
            LevelProgressManager.UnlockNextLevel(1);

            return; // 🛑 QUAN TRỌNG: dừng tại đây
        }

        spawnedInWave = 0;
        aliveZombies = 0;
        spawnTimer = 0f;

        CalculateNextInterval();

        Debug.Log("Wave " + (currentWaveIndex + 1) + " Start");
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
