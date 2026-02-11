using UnityEngine;

public class Sunflower : MonoBehaviour
{
    public GameObject sunPrefab;
    public float produceInterval = 6f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= produceInterval)
        {
            ProduceSun();
            timer = 0f;
        }
    }

    void ProduceSun()
    {
        Instantiate(
            sunPrefab,
            transform.position + new Vector3(0, 0.5f, 0),
            Quaternion.identity
        );
    }
}
