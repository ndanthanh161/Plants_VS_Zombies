using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    float moveSpeed;
    bool isEating;
    ZombieVisual visual;

    void Awake()
    {
        visual = GetComponent<ZombieVisual>();
    }

    public void Init(ZombieData data)
    {
        moveSpeed = data.moveSpeed;
    }

    void Update()
    {
        if (isEating) return;
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }

    public void SetEating(bool value)
    {
        isEating = value;
        
        // Gọi animation khi chuyển trạng thái Ăn/Đi bộ
        if (visual != null && visual.Anim != null)
        {
            visual.Anim.SetBool("IsEating", value);
        }
    }
}
