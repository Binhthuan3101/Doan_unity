using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Status")]
    public bool isGameOver = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TriggerWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("========================================");
        Debug.Log("🎉 CHIẾN THẮNG! BẠN ĐÃ PHÁ HỦY CĂN CỨ ĐỊCH!");
        Debug.Log("========================================");
    }

    public void TriggerLose()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("========================================");
        Debug.Log("❌ THẤT BẠI! CĂN CỨ CỦA BẠN ĐÃ BỊ PHÁ HỦY!");
        Debug.Log("========================================");
    }
}