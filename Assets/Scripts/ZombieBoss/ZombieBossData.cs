using UnityEngine;

[CreateAssetMenu(menuName = "PvZ/Zombie/Boss Data")]
public class ZombieBossData : ZombieData
{
    [Header("Boss - Phase 2")]
    [Tooltip("HP % để kích hoạt Phase 2 (0.0 - 1.0)")]
    [Range(0f, 1f)]
    public float phase2Threshold = 0.5f;

    [Tooltip("Hệ số nhân tốc độ di chuyển ở Phase 2")]
    public float phase2SpeedMultiplier = 1.5f;

    [Tooltip("Hệ số nhân sát thương ở Phase 2")]
    public float phase2DamageMultiplier = 2f;

    [Header("Boss - Visual Phase 2")]
    [Tooltip("Màu sắc zombie khi vào Phase 2")]
    public Color phase2Color = new Color(1f, 0.3f, 0.3f); // Đỏ

    [Header("Boss - UI")]
    [Tooltip("Tên hiển thị trên thanh máu Boss")]
    public string bossName = "Zombie Boss";
}
