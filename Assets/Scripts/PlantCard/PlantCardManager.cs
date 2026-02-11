using UnityEngine;

public class PlantCardManager : MonoBehaviour
{
    [Header("Plant Data")]
    public PlantData[] plantDatas;

    [Header("UI References")]
    public RectTransform cardParent;
    public PlantCardUI cardPrefab;

    [Header("Layout Settings")]
    [SerializeField] private float startX = 150f;
    [SerializeField] private float spacing = 130f;
    [SerializeField] private float posY = 0f;

    public PlantData SelectedPlant { get; private set; }

    void Start()
    {
        InitCards();
    }

    void InitCards()
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < plantDatas.Length; i++)
        {
            PlantCardUI card = Instantiate(cardPrefab, cardParent);
            RectTransform rt = card.GetComponent<RectTransform>();

            rt.anchorMin = new Vector2(0, 0.5f);
            rt.anchorMax = new Vector2(0, 0.5f);
            rt.pivot = new Vector2(0, 0.5f);
            rt.localScale = Vector3.one;

            rt.anchoredPosition = new Vector2(
                startX + i * spacing,
                posY
            );

            card.Setup(plantDatas[i], this);
        }
    }

    public void SelectPlant(PlantData plant)
    {
        SelectedPlant = plant;
        PlantPreviewController.Instance.ShowPreview(plant.previewPrefab);
    }

    public bool HasSelectedPlant()
    {
        return SelectedPlant != null;
    }

    public void ClearSelectedPlant()
    {
        SelectedPlant = null;
        PlantPreviewController.Instance.ClearPreview();
    }
}
