using UnityEngine;
using UnityEngine.InputSystem;

public class PlantPreviewController : MonoBehaviour
{
    public static PlantPreviewController Instance;

    private GameObject currentPreview;
    private PlantCardManager manager;

    void Awake()
    {
        Instance = this;
        manager = FindFirstObjectByType<PlantCardManager>();
    }

    void Update()
    {
        if (currentPreview == null) return;
        if (Mouse.current == null) return;

        // ===== Di chuyển preview theo chuột =====
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f)
        );
        worldPos.z = 0;
        currentPreview.transform.position = worldPos;

        // ===== Chuột phải -> hủy preview =====
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            ClearPreview();
            if (manager != null)
                manager.ClearSelectedPlant();
        }
    }

    public void ShowPreview(GameObject previewPrefab)
    {
        ClearPreview();
        currentPreview = Instantiate(previewPrefab);
    }

    public void ClearPreview()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }
    }
}
