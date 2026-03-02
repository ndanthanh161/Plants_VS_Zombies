using UnityEngine;

public class Sunflower : MonoBehaviour
{
    public GameObject sunPrefab;
    public float produceInterval = 6f;

    private float timer;
    private Animator anim;

    private void Awake()
    {
        // Lấy component Animator đang gắn trên cây hoa
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= produceInterval)
        {
            // Kích hoạt animation "rặn đẻ" mặt trời
            if (anim != null)
            {
                anim.SetTrigger("Produce");
            }

            // Reset lại đồng hồ đếm ngược
            timer = 0f;
        }
    }

    // HÀM NÀY SẼ ĐƯỢC GỌI BỞI ANIMATION EVENT
    public void SpawnSun()
    {
        Instantiate(
            sunPrefab,
            transform.position + new Vector3(0, 0.5f, 0), // Mặt trời xuất hiện cao hơn gốc cây một chút
            Quaternion.identity
        );
    }
}