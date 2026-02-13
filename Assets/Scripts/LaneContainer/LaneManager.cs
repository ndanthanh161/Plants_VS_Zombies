using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public GridManager grid;
    public GameObject lanePrefab;
    public GameObject lawnMowerPrefab;

    public float spawnX = 10f;
    public float mowerSpawnX = -8f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }

    void Start()
    {
        for (int r = 0; r < grid.rows; r++)
        {
            Vector3 lanePos = new Vector3(
                spawnX,
                grid.startPosition.y - r * grid.cellHeight,
                0
            );

            GameObject laneObj = Instantiate(
                lanePrefab,
                lanePos,
                Quaternion.identity,
                transform
            );

            Lane lane = laneObj.GetComponent<Lane>();
            lane.rowIndex = r;

            // SPAWN MOWER
            Vector3 mowerPos = new Vector3(
                mowerSpawnX,
                grid.startPosition.y - r * grid.cellHeight,
                0
            );

            GameObject mowerObj = Instantiate(
                lawnMowerPrefab,
                mowerPos,
                Quaternion.identity,
                transform
            );

            LawnMower mower = mowerObj.GetComponent<LawnMower>();
            mower.rowIndex = r;
        }
    }
}
