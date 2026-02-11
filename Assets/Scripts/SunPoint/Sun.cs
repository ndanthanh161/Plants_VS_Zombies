using UnityEngine;
using UnityEngine.EventSystems;

public class Sun : MonoBehaviour, IPointerClickHandler
{
    public int value = 25;
    public float lifeTime = 8f;
    public float fallSpeed = 2f;

    private Vector3 targetPosition;
    private bool isFalling = false;

    public void SetTargetPosition(Vector3 target)
    {
        targetPosition = target;
        isFalling = true;
    }

    void Update()
    {
        if (isFalling)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                fallSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                isFalling = false;
            }
        }
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SunManager.Instance != null)
        {
            SunManager.Instance.AddSun(value);
        }

        Destroy(gameObject);
    }
}
