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

    [Header("Combat Stats")]
    public int maxHP = 100;
    public int damage = 20;
    public float attackInterval = 1f;
}
