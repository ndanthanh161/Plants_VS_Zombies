using UnityEngine;

public class ZombieVisual : MonoBehaviour
{
    GameObject currentVisual;

    public void Init(ZombieData data)
    {
        if (currentVisual != null)
            Destroy(currentVisual);

        currentVisual = Instantiate(
            data.visualPrefab,
            transform
        );
    }
}
