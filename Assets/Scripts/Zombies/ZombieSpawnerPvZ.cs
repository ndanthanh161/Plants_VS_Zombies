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
    
    // Lưu trữ danh sách zombie sẽ xuất hiện trong Wave hiện tại (Deck)
    System.Collections.Generic.List<ZombieData> zombieDeck = new System.Collections.Generic.List<ZombieData>();

    void Start()
    {
        if (levelData != null && levelData.waves.Length > 0)
        {
            GenerateWaveDeck(levelData.waves[currentWaveIndex]);
        }
        CalculateNextInterval();
    }

    void GenerateWaveDeck(ZombieWave wave)
    {
        zombieDeck.Clear();

        if (wave.zombieChances == null || wave.zombieChances.Length == 0) return;
        
        // BƯỚC 1: Đảm bảo nhét ít nhất 1 con của mỗi loại được định cấu hình vào cỗ bài
        int guaranteedCount = Mathf.Min(wave.zombieChances.Length, wave.zombieCount);
        for (int i = 0; i < guaranteedCount; i++)
        {
            zombieDeck.Add(wave.zombieChances[i].zombieData);
        }
        
        // BƯỚC 2: Các slot còn lại (nếu có) sẽ bốc thăm Gacha dựa trên Weight như cũ
        for (int i = guaranteedCount; i < wave.zombieCount; i++)
        {
            zombieDeck.Add(GetRandomZombieByWeight(wave.zombieChances));
        }
        
        // BƯỚC 3: Xào bài để bọn chúng ra ngẫu nhiên chứ không bị ra có thứ tự
        for (int i = 0; i < zombieDeck.Count; i++)
        {
            int randomIndex = Random.Range(i, zombieDeck.Count);
            ZombieData temp = zombieDeck[i];
            zombieDeck[i] = zombieDeck[randomIndex];
            zombieDeck[randomIndex] = temp;
        }
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

        if (zombieDeck.Count == 0) return; // Bảo vệ bộ bài rỗng

        // Bốc thẻ bài trên cùng trong bộ bài đã xào (Deck)
        ZombieData data = zombieDeck[0];
        zombieDeck.RemoveAt(0);
        
        // Nếu không lấy được data (do chưa cấu hình) thì thoát ra
        if (data == null) return;

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

        GenerateWaveDeck(levelData.waves[currentWaveIndex]);
        CalculateNextInterval();

        Debug.Log("Wave " + (currentWaveIndex + 1) + " Start");
    }


    ZombieData GetRandomZombieByWeight(ZombieSpawnChance[] chances)
    {
        // Kiểm tra xem đã thêm list Zombie vào trong Wave trên Inspector chưa để tránh lỗi
        if (chances == null || chances.Length == 0)
        {
            Debug.LogError("Loi: Chua them Zombie vào danh sach zombieChances của Wave " + (currentWaveIndex + 1) + " ! Vui lòng cài trên Inspector.");
            return null;
        }

        int totalWeight = 0;

        for (int i = 0; i < chances.Length; i++)
            totalWeight += chances[i].weight;

        // Tránh lỗi chia 0 nếu để tất cả Chance Weight = 0
        if (totalWeight <= 0) return chances[0].zombieData;

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
