using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class HeroRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    public float baseRespawnTime = 7f;
    public float timeScalingFactor = 0.5f;

    [Header("Spawn Settings")]
    public Transform spawnPoint;

    private static float matchTimer = 0f;
    private bool isDead = false;

    private NavMeshAgent agent;
    private CharacterController characterController; // Thêm biến CharacterController
    private Collider heroCollider;
    private Renderer[] renderers;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>(); // Lấy component CharacterController
        heroCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();

        SetHeroState(true);
    }

    void Update()
    {
        if (!isDead)
        {
            matchTimer += Time.deltaTime;
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        float minutesPassed = matchTimer / 60f;
        float respawnTime = Mathf.Round(baseRespawnTime + (minutesPassed * timeScalingFactor));

        Debug.Log(gameObject.name + " đã chết! Hồi sinh sau: " + respawnTime + "s");

        SetHeroState(false);
        StartCoroutine(RespawnRoutine(respawnTime));
    }

    IEnumerator RespawnRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (spawnPoint != null)
        {
            // QUAN TRỌNG: Phải tắt NavMeshAgent và CharacterController trước khi đổi vị trí
            if (agent != null) agent.enabled = false;
            if (characterController != null) characterController.enabled = false;

            // Dịch chuyển về Base
            transform.position = spawnPoint.position;

            // Bật lại sau khi đã dịch chuyển xong
            if (characterController != null) characterController.enabled = true;
            if (agent != null) agent.enabled = true;
        }
        else
        {
            Debug.LogWarning("Chưa gán Spawn Point cho " + gameObject.name + "!");
        }

        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.ResetHealth();
        }

        SetHeroState(true);

        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.ResetAI();
        }

        isDead = false;
        Debug.Log(gameObject.name + " đã hồi sinh thành công về Base!");
    }

    void SetHeroState(bool active)
    {
        if (renderers != null)
        {
            foreach (Renderer r in renderers) r.enabled = active;
        }
        if (heroCollider != null) heroCollider.enabled = active;
        if (agent != null) agent.enabled = active;
        if (characterController != null) characterController.enabled = active;
    }
}