using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 15f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Ưu tiên lấy tín hiệu từ Virtual Joystick trên Mobile
        if (VirtualJoystick.instance != null)
        {
            if (Mathf.Abs(VirtualJoystick.instance.Horizontal) > 0.05f || Mathf.Abs(VirtualJoystick.instance.Vertical) > 0.05f)
            {
                moveX = VirtualJoystick.instance.Horizontal;
                moveZ = VirtualJoystick.instance.Vertical;
            }
        }

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;

        if (moveDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0), rotationSpeed * Time.deltaTime);

            controller.Move(moveDir * moveSpeed * Time.deltaTime);
        }

        Vector3 gravityMove = Vector3.zero;
        if (!controller.isGrounded)
        {
            gravityMove.y -= 9.81f * Time.deltaTime;
        }
        controller.Move(gravityMove);
    }
}