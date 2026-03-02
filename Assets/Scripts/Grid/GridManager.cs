using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int rows = 5;
    public int columns = 9;
    public float cellWidth = 1.33f;
    public float cellHeight = 1.46f;
    public Vector2 startPosition = new Vector2(-2.9f, 2.1f);

    public GameObject cellPrefab;

    [Header("Audio")]
    [Tooltip("Tiếng trồng cây dùng chung cho tất cả các loại cây")]
    public AudioClip plantSound;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnGrid();
    }

    void SpawnGrid()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3 pos = new Vector3(
                    startPosition.x + c * cellWidth,
                    startPosition.y - r * cellHeight,
                    0
                );

                Instantiate(cellPrefab, pos, Quaternion.identity, transform);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3 center = new Vector3(
                    startPosition.x + c * cellWidth,
                    startPosition.y - r * cellHeight,
                    0
                );

                Gizmos.DrawWireCube(center, new Vector3(cellWidth, cellHeight, 0));
            }
        }
    }
}
