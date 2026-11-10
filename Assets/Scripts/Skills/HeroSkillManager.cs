using UnityEngine;
using System.Collections;

public class HeroSkillManager : MonoBehaviour
{
    private Health playerHealth;
    private CharacterController controller;

    [Header("Skill 1: Projectile (Đạn năng lượng)")]
    public GameObject skill1Prefab;
    public Transform firePoint;
    public float skill1Cooldown = 2.0f;
    public float skill1ManaCost = 25f;
    private float nextSkill1Time = 0f;

    [Header("Skill 2: Dash & Slash (Lướt chém)")]
    public float dashDistance = 5.0f;
    public float dashDamage = 35f;
    public float dashCooldown = 4.0f;
    public float dashManaCost = 40f;
    public LayerMask enemyLayer;
    private float nextDashTime = 0f;

    [Header("Ultimate: Storm Zone (Vùng bão năng lượng liên tục)")]
    public float ultiRadius = 6.0f;          // Bán kính vùng ảnh hưởng chiêu cuối
    public float ultiTotalDamage = 100f;     // Tổng sát thương (sẽ chia đều qua các đợt)
    public int ultiTicks = 4;                // Số đợt giật sát thương (ví dụ: giật 4 lần)
    public float ultiDuration = 2.0f;        // Thời gian kéo dài vùng bão (giây)
    public float ultiCooldown = 12.0f;       // Hồi chiêu
    public float ultiManaCost = 80f;         // Tốn Mana
    public GameObject ultiEffectPrefab;      // Hiệu ứng vòng bão (nếu có)
    private float nextUltiTime = 0f;

    [Header("Heal & Mana Restore (Hồi phục)")]
    public float healAmount = 50f;         // Lượng máu hồi phục
    public float manaRestoreAmount = 40f;  // Lượng năng lượng hồi phục
    public float restoreCooldown = 15.0f;  // Thời gian hồi chiêu của nút (giây)
    private float nextRestoreTime = 0f;

    [Header("Recall (Biến Về Nhà)")]
    public Transform baseSpawnPoint;         // Vị trí nhà chính (kéo điểm Spawn/Base của bạn vào đây)
    public float recallTime = 3.0f;          // Thời gian đứng gồng biến về (giây)
    public float recallCooldown = 20.0f;     // Thời gian hồi chiêu nút biến về
    private float nextRecallTime = 0f;
    private bool isRecalling = false;

    void Start()
    {
        playerHealth = GetComponent<Health>();
        controller = GetComponent<CharacterController>();
    }

    // ==========================================
    // 1. KỸ NĂNG 1: BẮN ĐẠN TỪ XA
    // ==========================================
    public void CastSkill1()
    {
        if (Time.time < nextSkill1Time)
        {
            Debug.Log("⏳ Kỹ năng 1 đang hồi chiêu!");
            return;
        }

        if (playerHealth != null && !playerHealth.UseMana(skill1ManaCost))
        {
            Debug.Log("❌ Không đủ Mana dùng Kỹ năng 1!");
            return;
        }

        if (skill1Prefab != null && firePoint != null)
        {
            Instantiate(skill1Prefab, firePoint.position, firePoint.rotation);
            Debug.Log("⚡ Đã bắn Kỹ năng 1!");
        }

        nextSkill1Time = Time.time + skill1Cooldown;
    }

    // ==========================================
    // 2. KỸ NĂNG 2: LƯỚT VÀ CHÉM VÙNG
    // ==========================================
    public void CastSkill2()
    {
        if (Time.time < nextDashTime)
        {
            Debug.Log("⏳ Kỹ năng 2 đang hồi chiêu!");
            return;
        }

        if (playerHealth != null && !playerHealth.UseMana(dashManaCost))
        {
            Debug.Log("❌ Không đủ Mana dùng Kỹ năng 2!");
            return;
        }

        StartCoroutine(PerformDashRoutine());
        nextDashTime = Time.time + dashCooldown;
    }

    IEnumerator PerformDashRoutine()
    {
        Debug.Log("⚡ Đang lướt chém!");
        Vector3 dashDirection = transform.forward;
        float duration = 0.2f;
        float elapsed = 0f;
        float speed = dashDistance / duration;

        while (elapsed < duration)
        {
            if (controller != null)
            {
                controller.Move(dashDirection * speed * Time.deltaTime);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 2.5f, enemyLayer);
        foreach (Collider col in hitEnemies)
        {
            Health enemyHealth = col.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(dashDamage);
            }
        }
    }

    // ==========================================
    // 3. CHIÊU CUỐI (ULTIMATE): BÃO NĂNG LƯỢNG LIÊN TỤC
    // ==========================================
    public void CastUltimate()
    {
        if (Time.time < nextUltiTime)
        {
            Debug.Log("⏳ Chiêu cuối đang hồi chiêu!");
            return;
        }

        if (playerHealth != null && !playerHealth.UseMana(ultiManaCost))
        {
            Debug.Log("❌ Không đủ Mana dùng Chiêu cuối!");
            return;
        }

        StartCoroutine(PerformUltimateStorm());
        nextUltiTime = Time.time + ultiCooldown;
    }

    IEnumerator PerformUltimateStorm()
    {
        Debug.Log("🌀 THI TRIỂN CHIÊU CUỐI: BÃO NĂNG LƯỢNG BẮT ĐẦU!");

        // Sinh hiệu ứng bão nếu có
        GameObject fx = null;
        if (ultiEffectPrefab != null)
        {
            fx = Instantiate(ultiEffectPrefab, transform.position, Quaternion.identity, transform);
            Destroy(fx, ultiDuration);
        }

        float damagePerTick = ultiTotalDamage / ultiTicks;
        float interval = ultiDuration / ultiTicks;

        for (int i = 0; i < ultiTicks; i++)
        {
            // Quét liên tục xung quanh vị trí nhân vật trong mỗi đợt giật sát thương
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, ultiRadius, enemyLayer);
            Debug.Log($"⚡ Bão giật đợt {i + 1}/{ultiTicks} trúng {hitEnemies.Length} mục tiêu!");

            foreach (Collider col in hitEnemies)
            {
                Health enemyHealth = col.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damagePerTick);
                }
            }

            // Chờ đến đợt giật tiếp theo
            yield return new WaitForSeconds(interval);
        }

        Debug.Log("🌀 Vùng bão năng lượng đã kết thúc.");
    }

    // Vẽ vùng tầm đánh kỹ năng trên Scene View
    void OnDrawGizmosSelected()
    {
        // Tầm chém kỹ năng 2 (Màu xanh cyan)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 2.5f);

        // Tầm bão Chiêu cuối (Màu vàng/cam rộng hơn)
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, ultiRadius);
    }
    // ==========================================
    // 4. KỸ NĂNG HỒI MÁU & NĂNG LƯỢNG
    // ==========================================
    public void CastRestore()
    {
        if (Time.time < nextRestoreTime)
        {
            Debug.Log("⏳ Kỹ năng hồi phục đang trong thời gian hồi chiêu!");
            return;
        }

        if (playerHealth != null)
        {
            // Gọi hàm cộng máu và mana từ component Health
            playerHealth.Heal(healAmount);
            playerHealth.RestoreMana(manaRestoreAmount);

            Debug.Log("❤️ Đã hồi " + healAmount + " Máu và 💧 " + manaRestoreAmount + " Mana thành công!");
        }

        nextRestoreTime = Time.time + restoreCooldown;
    }

    // ==========================================
    // 5. KỸ NĂNG BIẾN VỀ (RECALL)
    // ==========================================
    public void CastRecall()
    {
        if (isRecalling)
        {
            Debug.Log("⏳ Đang trong quá trình biến về...");
            return;
        }

        if (Time.time < nextRecallTime)
        {
            Debug.Log("⏳ Nút Biến Về đang hồi chiêu!");
            return;
        }

        StartCoroutine(RecallRoutine());
    }

    IEnumerator RecallRoutine()
    {
        isRecalling = true;
        Debug.Log("🏠 Bắt đầu biến về nhà sau " + recallTime + " giây... (Đứng yên đừng di chuyển!)");

        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        // Đứng gồng trong thời gian recallTime (nếu di chuyển có thể hủy, ở đây tạm làm gồng đủ thời gian)
        while (elapsed < recallTime)
        {
            // Nếu tướng bị dịch chuyển quá xa (bị đẩy/đánh văng) có thể hủy, tạm thời ta cho gồng đủ thời gian
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 1. Dịch chuyển nhân vật về vị trí Nhà Chính (Base)
        if (baseSpawnPoint != null)
        {
            CharacterController charController = GetComponent<CharacterController>();
            if (charController != null) charController.enabled = false; // Tắt tạm CharacterController để ép đổi vị trí

            transform.position = baseSpawnPoint.position;
            transform.rotation = baseSpawnPoint.rotation;

            if (charController != null) charController.enabled = true;  // Bật lại
        }
        else
        {
            Debug.LogWarning("⚠️ Chưa gán BaseSpawnPoint cho nút Biến Về trong Inspector!");
        }

        // 2. Hồi đầy 100% Máu và Năng lượng khi về đến nhà
        if (playerHealth != null)
        {
            playerHealth.Heal(playerHealth.maxHealth);
            playerHealth.RestoreMana(playerHealth.maxMana);
        }

        Debug.Log("✨ Biến về thành công! Đã hồi đầy Máu và Mana tại Nhà Chính!");

        isRecalling = false;
        nextRecallTime = Time.time + recallCooldown;
    }
}