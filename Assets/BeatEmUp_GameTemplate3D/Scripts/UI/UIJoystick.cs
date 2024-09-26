using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class UIJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform handle;
    public float radius = 40f;
    public float autoReturnSpeed = 8f;
    private bool returnToStartPos;
    private RectTransform parentRect;
    private InputManager inputManager;

    void OnEnable()
    {
        returnToStartPos = true;
        handle.transform.SetParent(transform);
        parentRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (inputManager == null) inputManager = GameObject.FindObjectOfType<InputManager>();

        // Ensure only touchscreen input is processed
        if (inputManager.inputType != INPUTTYPE.TOUCHSCREEN) return;

        // Return to start position
        if (returnToStartPos)
        {
            if (handle.anchoredPosition.magnitude > Mathf.Epsilon)
            {
                handle.anchoredPosition -= new Vector2(handle.anchoredPosition.x * autoReturnSpeed, handle.anchoredPosition.y * autoReturnSpeed) * Time.deltaTime;
                inputManager.OnTouchScreenJoystickEvent(Vector2.zero);
            }
            else
            {
                returnToStartPos = false;
            }
        }
    }

    // Touch down
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (inputManager.inputType != INPUTTYPE.TOUCHSCREEN) return;

        returnToStartPos = false;
        var handleOffset = GetJoystickOffset(eventData);
        handle.anchoredPosition = handleOffset;
        inputManager.OnTouchScreenJoystickEvent(handleOffset.normalized);
    }

    // Touch drag
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (inputManager.inputType != INPUTTYPE.TOUCHSCREEN) return;

        var handleOffset = GetJoystickOffset(eventData);
        handle.anchoredPosition = handleOffset;
        inputManager.OnTouchScreenJoystickEvent(handleOffset.normalized);
    }

    // Touch up
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (inputManager.inputType != INPUTTYPE.TOUCHSCREEN) return;

        returnToStartPos = true;
    }

    // Get offset
    private Vector2 GetJoystickOffset(PointerEventData eventData)
    {
        Vector3 globalHandle;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out globalHandle))
        {
            handle.position = globalHandle;
        }

        var handleOffset = handle.anchoredPosition;
        if (handleOffset.magnitude > radius)
        {
            handleOffset = handleOffset.normalized * radius;
            handle.anchoredPosition = handleOffset;
        }
        return handleOffset;
    }
}
