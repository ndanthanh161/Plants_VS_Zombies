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

        // CHECK SUN MANAGER
        if (SunManager.Instance == null) return;

        // CHECK TIỀN
        if (!SunManager.Instance.CanAfford(data.cost))
        {
            Debug.Log("Not enough sun!");
            return;
        }

        // TRỪ TIỀN
        SunManager.Instance.SpendSun(data.cost);
        PlantCardUI[] cards = FindObjectsByType<PlantCardUI>(FindObjectsSortMode.None);

        foreach (PlantCardUI card in cards)
        {
            if (card != null && card.GetPlantData() == data)
            {
                card.StartCooldown();
            }
        }



        // SPAWN CÂY
        // SPAWN CÂY
        currentPlant = Instantiate(
            data.plantPrefab,
            transform.position,
            Quaternion.identity
        );

        // INIT HEALTH
        PlantHealth health = currentPlant.GetComponent<PlantHealth>();
        if (health != null)
        {
            health.Init(data);
        }

        // INIT ATTACK
        PlantAttack attack = currentPlant.GetComponent<PlantAttack>();
        if (attack != null)
        {
            attack.Init(data);
        }

        isOccupied = true;


        manager.ClearSelectedPlant();
        PlantPreviewController.Instance.ClearPreview();
    }
}
