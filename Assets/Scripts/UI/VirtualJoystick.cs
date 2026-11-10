using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public static VirtualJoystick instance;

    [Header("UI Elements")]
    public Image joystickBackground;
    public Image joystickHandle;

    private Vector2 inputVector = Vector2.zero;

    public float Horizontal => inputVector.x;
    public float Vertical => inputVector.y;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground.rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pos))
        {
            pos.x = (pos.x / joystickBackground.rectTransform.sizeDelta.x);
            pos.y = (pos.y / joystickBackground.rectTransform.sizeDelta.y);

            inputVector = new Vector2(pos.x * 2, pos.y * 2);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            // Di chuyển hình Cần gạt nhỏ bên trong
            joystickHandle.rectTransform.anchoredPosition = new Vector2(
                inputVector.x * (joystickBackground.rectTransform.sizeDelta.x / 3),
                inputVector.y * (joystickBackground.rectTransform.sizeDelta.y / 3));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystickHandle.rectTransform.anchoredPosition = Vector2.zero;
    }
}