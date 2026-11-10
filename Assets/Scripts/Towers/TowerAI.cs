using UnityEngine;

public class TowerAI : MonoBehaviour
{
    [Header("Tower Attributes")]
    public float attackRange = 12f;
    public float attackCooldown = 1.0f;
    public LayerMask targetLayer; // Chọn layer của kẻ địch (Player hoặc Lính)

    [Header("Projectile Reference")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private float nextAttackTime = 0f;

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            Transform nearestTarget = FindNearestTarget();
            if (nearestTarget != null)
            {
                ShootTarget(nearestTarget);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    Transform FindNearestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, targetLayer);
        Transform nearest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider col in hits)
        {
            float distance = Vector3.Distance(transform.position, col.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearest = col.transform;
            }
        }
        return nearest;
    }

    void ShootTarget(Transform target)
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            TowerProjectile projScript = projObj.GetComponent<TowerProjectile>();
            if (projScript != null)
            {
                projScript.SetTarget(target);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}