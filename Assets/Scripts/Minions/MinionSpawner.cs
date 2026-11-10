using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [Header("Minion Prefabs")]
    public GameObject topMinionPrefab;
    public GameObject midMinionPrefab;
    public GameObject botMinionPrefab;

    [Header("Spawn Points (Vị trí xuất phát)")]
    public Transform topSpawnPoint;
    public Transform midSpawnPoint;
    public Transform botSpawnPoint;

    [Header("Waypoint Folders (Thư mục chứa đường đi)")]
    public Transform topWaypointParent;
    public Transform midWaypointParent;
    public Transform botWaypointParent;

    [Header("Spawn Settings")]
    public float spawnInterval = 30f; // Đợi đúng 30 giây
    private float spawnTimer = 0f;

    void Start()
    {
        // Đã xóa dòng SpawnWave() ở đây để khi vừa vào game lính chưa xuất hiện ngay lập tức
    }

    void Update()
    {
        // Cộng dồn thời gian theo từng giây
        spawnTimer += Time.deltaTime;

        // Khi đồng hồ đếm đủ 30 giây (hoặc thời gian cấu hình)
        if (spawnTimer >= spawnInterval)
        {
            SpawnWave();     // Sinh đợt lính
            spawnTimer = 0f; // Reset lại đồng hồ về 0 để đếm tiếp 30 giây cho đợt sau
        }
    }

    void SpawnWave()
    {
        SpawnLane(topMinionPrefab, topSpawnPoint, topWaypointParent);
        SpawnLane(midMinionPrefab, midSpawnPoint, midWaypointParent);
        SpawnLane(botMinionPrefab, botSpawnPoint, botWaypointParent);

        Debug.Log("Đã xuất hiện một đợt lính mới sau 30 giây!");
    }

    void SpawnLane(GameObject prefab, Transform spawnPoint, Transform waypointParent)
    {
        if (prefab == null || spawnPoint == null || waypointParent == null) return;

        // Sinh lính tại vị trí xuất phát
        GameObject minion = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        LaneMinion laneMinion = minion.GetComponent<LaneMinion>();

        if (laneMinion != null && waypointParent.childCount > 0)
        {
            // Tự động lấy tất cả các điểm con bên trong thư mục đường đi gán cho lính
            Transform[] points = new Transform[waypointParent.childCount];
            for (int i = 0; i < waypointParent.childCount; i++)
            {
                points[i] = waypointParent.GetChild(i);
            }
            laneMinion.waypoints = points;
        }
    }
}