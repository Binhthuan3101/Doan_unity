using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class LaneMinion : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Waypoints Settings")]
    public Transform[] waypoints; // Danh sách các điểm mốc trên đường
    private int currentWaypointIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Đi tới điểm mốc đầu tiên ngay khi xuất hiện
        if (waypoints != null && waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // Kiểm tra nếu lính đã chạy đến gần điểm mốc hiện tại
        if (!agent.pathPending && agent.remainingDistance <= 0.8f)
        {
            currentWaypointIndex++;

            // Nếu vẫn còn điểm mốc tiếp theo thì tiếp tục di chuyển tới đó
            if (currentWaypointIndex < waypoints.Length)
            {
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
            else
            {
                // Đã đi hết các điểm mốc
                currentWaypointIndex = waypoints.Length - 1;
            }
        }
    }
}