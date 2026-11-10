using UnityEngine;

public class SkillProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 20f;             // Tốc độ bay của viên đạn
    public float damage = 40f;            // Lượng sát thương gây ra
    public float lifetime = 3.0f;         // Thời gian tự hủy nếu bay quá xa không trúng ai
    public LayerMask targetLayer;         // Các layer chịu sát thương (tích chọn Enemy và JungleMonster)

    void Start()
    {
        // Tự hủy sau khoảng thời gian lifetime để giải phóng bộ nhớ
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Viên đạn luôn bay thẳng về phía trước theo hướng mặt của nó
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem đối tượng va chạm có thuộc layer mục tiêu hay không
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            // Lấy component Health của mục tiêu để trừ máu
            Health targetHealth = other.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
                Debug.Log("🎯 Chiêu 1 trúng mục tiêu: " + other.gameObject.name + " | Gây " + damage + " sát thương!");
            }

            // Hủy viên đạn ngay lập tức sau khi va chạm trúng đích
            Destroy(gameObject);
        }
    }
}