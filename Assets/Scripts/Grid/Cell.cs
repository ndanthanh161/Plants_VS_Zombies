using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public bool isOccupied;
    private GameObject currentPlant;

    private PlantCardManager manager;

    void Awake()
    {
        manager = FindFirstObjectByType<PlantCardManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // CHỈ xử lý chuột trái
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // ===== ƯU TIÊN XẺNG =====
        if (ShovelManager.Instance != null &&
            ShovelManager.Instance.IsShovelActive &&
            isOccupied)
        {
            Destroy(currentPlant);
            currentPlant = null;
            isOccupied = false;

            ShovelManager.Instance.DeactivateShovel();

            Debug.Log("PLANT REMOVED");
            return;
        }

        // ===== TRỒNG CÂY =====
        if (manager == null) return;
        if (!manager.HasSelectedPlant()) return;
        if (isOccupied) return;

        PlantData data = manager.SelectedPlant;

        currentPlant = Instantiate(
            data.plantPrefab,
            transform.position,
            Quaternion.identity
        );

        isOccupied = true;
        manager.ClearSelectedPlant();
        PlantPreviewController.Instance.ClearPreview();
    }
}
