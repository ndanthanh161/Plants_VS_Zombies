using UnityEngine;

public class SunSpawner : MonoBehaviour
{
    public GameObject sunPrefab;

    public float spawnInterval = 10f;
    public float spawnY = 6f;       // vị trí trên trời
    public float groundY = 2f;      // vị trí dừng lại

    public float minX = -4f;
    public float maxX = 4f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnSun();
            timer = 0f;
        }
    }

    void SpawnSun()
    {
        float randomX = Random.Range(minX, maxX);

        Vector3 spawnPos = new Vector3(randomX, spawnY, 0);
        Vector3 targetPos = new Vector3(randomX, groundY, 0);

        GameObject sunObj = Instantiate(sunPrefab, spawnPos, Quaternion.identity);

        Sun sun = sunObj.GetComponent<Sun>();
        if (sun != null)
        {
            sun.SetTargetPosition(targetPos);
        }
    }
}
