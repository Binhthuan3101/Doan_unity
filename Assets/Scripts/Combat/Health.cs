using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Mana Settings")]
    public float maxMana = 100f;
    public float currentMana;

    [Header("Reward on Death")]
    public bool isEnemy = false;
    public int goldReward = 20;
    public float expReward = 30f;

    [Header("Events")]
    public UnityEvent<float, float> onHealthChanged;
    public UnityEvent<float, float> onManaChanged;
    public UnityEvent onDeath;

    [HideInInspector]
    public bool isDead = false;

    void Start()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        isDead = false;

        onHealthChanged?.Invoke(currentHealth, maxHealth);
        onManaChanged?.Invoke(currentMana, maxMana);
    }

    public void UpdateHealthBar()
    {
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        onManaChanged?.Invoke(currentMana, maxMana);
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead || currentHealth <= 0) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log(gameObject.name + " nhận " + damageAmount + " sát thương. Máu còn lại: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public bool UseMana(float manaAmount)
    {
        if (isDead) return false;

        if (currentMana >= manaAmount)
        {
            currentMana -= manaAmount;
            currentMana = Mathf.Clamp(currentMana, 0f, maxMana); // FIX: Dùng maxMana thay vì maxHealth
            onManaChanged?.Invoke(currentMana, maxMana);        // FIX: Truyền maxMana lên UI
            return true;
        }
        return false;
    }

    // Hồi máu cho nhân vật (không vượt quá MaxHealth)
    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth); // FIX: Cập nhật thanh Máu UI lập tức

        Debug.Log("❤️ " + gameObject.name + " hồi " + amount + " HP. Máu hiện tại: " + currentHealth + "/" + maxHealth);
    }

    // Hồi năng lượng cho nhân vật (không vượt quá MaxMana)
    public void RestoreMana(float amount)
    {
        if (isDead) return;

        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0f, maxMana);
        onManaChanged?.Invoke(currentMana, maxMana);     // FIX: Cập nhật thanh Mana UI lập tức

        Debug.Log("💧 " + gameObject.name + " hồi " + amount + " MP. Mana hiện tại: " + currentMana + "/" + maxMana);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " đã bị tiêu diệt!");

        if (isEnemy && PlayerStats.instance != null)
        {
            PlayerStats.instance.AddGold(goldReward);
            PlayerStats.instance.AddExp(expReward);
        }

        onDeath?.Invoke();

        // Kiểm tra xem có script hồi sinh (dành cho Tướng/Hero) hay không
        HeroRespawn respawnScript = GetComponent<HeroRespawn>();
        if (respawnScript != null)
        {
            respawnScript.Die();
        }
        else
        {
            // Nếu là Lính hoặc Trụ thì hủy đối tượng sau 0.2 giây
            Destroy(gameObject, 0.2f);
        }
    }
}