using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlantCardUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI costText;
    public Button button;

    private PlantData data;
    private PlantCardManager manager;

    public void Setup(PlantData plantData, PlantCardManager cardManager)
    {
        data = plantData;
        manager = cardManager;

        icon.sprite = data.icon;
        costText.text = data.cost.ToString();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        manager.SelectPlant(data);
    }
}
