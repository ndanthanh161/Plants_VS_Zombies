using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    float moveSpeed;
    bool isEating;

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
    }
}
