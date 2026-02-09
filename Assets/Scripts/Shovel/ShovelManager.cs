using UnityEngine;
using UnityEngine.InputSystem;

public class ShovelManager : MonoBehaviour
{
    public static ShovelManager Instance;

    [Header("Preview")]
    public GameObject shovelPreviewPrefab;

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
        }
    }

    public void ActivateShovel()
    {
        IsShovelActive = true;

        if (currentPreview == null)
        {
            currentPreview = Instantiate(shovelPreviewPrefab);
        }

        Debug.Log("SHOVEL MODE ON");
    }

    public void DeactivateShovel()
    {
        IsShovelActive = false;

        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }

        Debug.Log("SHOVEL MODE OFF");
    }
}
