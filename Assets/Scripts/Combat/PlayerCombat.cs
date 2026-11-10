using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 4.0f;
    public float attackDamage = 25f;
    public float attackCooldown = 1.0f;
    public LayerMask enemyLayer;

    private float nextAttackTime = 0f;

    void Update()
    {
        // Vẫn giữ phím Space để test trên PC nếu cần
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryAttack();
        }
    }

    // Hàm gọi khi bấm nút Đánh Thường trên UI Mobile hoặc phím Space
    public void TryAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            PerformAttack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void PerformAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider enemyCollider in hitEnemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemyCollider.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemyCollider.transform;
            }
        }

        if (nearestEnemy != null)
        {
            Vector3 lookDir = (nearestEnemy.position - transform.position).normalized;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }

            Health enemyHealth = nearestEnemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
        else
        {
            Debug.Log("Không có mục tiêu trong tầm đánh!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}