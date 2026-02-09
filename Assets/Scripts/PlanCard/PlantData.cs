using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "PVZ/Plant Data")]
public class PlantData : ScriptableObject
{
    public GameObject previewPrefab;
    public string plantName;
    public Sprite icon;
    public int cost;
    public float cooldown;
    public GameObject plantPrefab;
}
