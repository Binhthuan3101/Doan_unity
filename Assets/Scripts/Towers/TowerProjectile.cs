using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    public float speed = 22f;
    public float damage = 35f;
    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.LookAt(target);

        // Kiểm tra nếu đạn chạm vào mục tiêu (bán kính va chạm là 0.4f)
        if (Vector3.Distance(transform.position, target.position) < 0.4f)
        {
            Health enemyHealth = target.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}