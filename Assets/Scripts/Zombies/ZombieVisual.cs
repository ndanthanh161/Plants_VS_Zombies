using UnityEngine;

public class ZombieVisual : MonoBehaviour
{
    GameObject currentVisual;
    public Animator Anim { get; private set; } // Thêm biến chứa Animator

    public void Init(ZombieData data)
    {
        if (currentVisual != null)
            Destroy(currentVisual);

        currentVisual = Instantiate(
            data.visualPrefab,
            transform
        );
        
        // Tự động tìm Animator bên trong visual prefab vừa sinh ra (lấy component bên trong con)
        Anim = currentVisual.GetComponentInChildren<Animator>();
        
        if (Anim == null)
        {
            Debug.LogError("Khong tim thay Animator trong prefab hinh anh cua Zombie! " + data.visualPrefab.name);
        }
    }
}
