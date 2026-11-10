using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform target;

    [Header("Camera Positioning")]
    public Vector3 offset = new Vector3(0f, 15f, -12f);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // Vị trí mong muốn của camera theo nhân vật
        Vector3 desiredPosition = target.position + offset;

        // Di chuyển mượt mà từ vị trí hiện tại đến vị trí mong muốn
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}