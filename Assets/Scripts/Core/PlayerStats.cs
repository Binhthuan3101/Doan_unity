using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    [Header("Player Economy & Level")]
    public int gold = 0;
    public int level = 1;
    public float currentExp = 0f;
    public float maxExpToLevelUp = 100f;

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

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log("Nhận " + amount + " vàng. Tổng vàng hiện tại: " + gold);
    }

    public void AddExp(float amount)
    {
        currentExp += amount;
        Debug.Log("Nhận " + amount + " EXP. EXP hiện tại: " + currentExp + "/" + maxExpToLevelUp);

        if (currentExp >= maxExpToLevelUp)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        currentExp -= maxExpToLevelUp;
        maxExpToLevelUp *= 1.5f;
        Debug.Log("CHÚC MỪNG! Tướng đã lên cấp " + level);
    }
}