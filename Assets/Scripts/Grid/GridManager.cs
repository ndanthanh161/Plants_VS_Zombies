using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int rows = 5;
    public int columns = 9;
    public float cellWidth = 1.8f;
    public float cellHeight = 2f;
    public Vector2 startPosition = new Vector2(-7.2f, 4f);

    public GameObject cellPrefab;

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
