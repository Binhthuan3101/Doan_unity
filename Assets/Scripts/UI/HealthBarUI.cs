using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image fillImage;
    public Health targetHealth;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // TỰ ĐỘNG TÌM component Health ở đối tượng cha (Lính, Tướng, Trụ) nếu chưa được gán
        if (targetHealth == null)
        {
            targetHealth = GetComponentInParent<Health>();
        }

        if (targetHealth != null)
        {
            // Đăng ký nhận sự kiện thay đổi máu
            targetHealth.onHealthChanged.AddListener(UpdateHealthBar);

            // Cập nhật hiển thị đầy máu ngay khi vừa xuất hiện
            UpdateHealthBar(targetHealth.currentHealth, targetHealth.maxHealth);
        }
        else
        {
            Debug.LogWarning("HealthBarUI không tìm thấy component Health trên đối tượng: " + gameObject.name);
        }
    }

    void LateUpdate()
    {
        // Giúp thanh máu luôn quay mặt về phía Camera góc nhìn MOBA
        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (fillImage != null && maxHealth > 0)
        {
            fillImage.fillAmount = currentHealth / maxHealth;
        }
    }
}