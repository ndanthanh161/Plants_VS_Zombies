using UnityEngine;
using UnityEngine.InputSystem;

public class ShovelManager : MonoBehaviour
{
    public static ShovelManager Instance;

    [Header("Preview")]
    public GameObject shovelPreviewPrefab;

    [Header("Audio")]
    [Tooltip("Tiếng xẻ ng khi xóa cây")]
    public AudioClip shovelSound;

    private GameObject currentPreview;

    public bool IsShovelActive { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!IsShovelActive || currentPreview == null) return;
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(mousePos.x, mousePos.y, 10f)
        );
        worldPos.z = 0;

        currentPreview.transform.position = worldPos;

        // Chuột phải → hủy xẻng
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            DeactivateShovel();
            return;
        }

        // Chuột trái → xẻng cây (dùng Physics2D, bỏ qua EventSystem)
        // Cách này hoạt động với MỌI loại cây kể cả SunFlower, PotatoMine
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Collider2D[] hits = Physics2D.OverlapPointAll((Vector2)worldPos);
            foreach (var hit in hits)
            {
                Cell cell = hit.GetComponent<Cell>();
                if (cell != null && cell.isOccupied)
                {
                    cell.RemovePlant();
                    break;
                }
            }
        }
    }

    public void ActivateShovel()
    {
        IsShovelActive = true;

        // 🔥 ẨN CHUỘT
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        if (currentPreview == null)
        {
            currentPreview = Instantiate(shovelPreviewPrefab);
        }

        Debug.Log("SHOVEL MODE ON");
    }

    public void DeactivateShovel()
    {
        IsShovelActive = false;

        // 🔥 HIỆN LẠI CHUỘT
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }

        Debug.Log("SHOVEL MODE OFF");
    }
}
