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

    void Update()
    {
        // Khôi phục trạng thái trống nếu cây trên ô đã bị xóa bỏ (bị zombie ăn, Potato Mine nổ, v.v...)
        if (isOccupied && currentPlant == null)
        {
            isOccupied = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // CHỈ xử lý chuột trái
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // ===== TRỒNG CÂY (xẻng đã xử lý riêng qua ShovelManager.Update) =====
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

        // PHÁT TIẾNG KHI TRỒNG CÂY (để cuối cùng, không ảnh hưởng logic trồng)
        if (GridManager.Instance != null && GridManager.Instance.plantSound != null)
            AudioManager.GetInstance().PlaySound(GridManager.Instance.plantSound);
    }

    // Được gọi trực tiếp từ ShovelManager — hoạt động với MỌI loại cây
    public void RemovePlant()
    {
        if (!isOccupied) return;

        Destroy(currentPlant);
        currentPlant = null;
        isOccupied = false;

        ShovelManager.Instance.DeactivateShovel();

        // Phát tiếng xẻng
        if (ShovelManager.Instance.shovelSound != null)
            AudioManager.GetInstance().PlaySound(ShovelManager.Instance.shovelSound);

        Debug.Log("PLANT REMOVED");
    }
}
