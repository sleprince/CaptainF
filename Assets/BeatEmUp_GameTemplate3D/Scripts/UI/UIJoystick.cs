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
    private InputManager inputmanager;

    void OnEnable()
    {
        returnToStartPos = true;
        handle.transform.SetParent(transform);
        parentRect = GetComponent<RectTransform>();
    }

    void Update()
    {
<<<<<<< Updated upstream
        if (inputmanager == null) inputmanager = GameObject.FindObjectOfType<InputManager>();

        //return to start position
=======
        if (inputManager == null)
        {
            inputManager = FindObjectOfType<InputManager>();
        }

        if (inputManager == null) return;

        // Return to start position
>>>>>>> Stashed changes
        if (returnToStartPos)
        {
            if (handle.anchoredPosition.magnitude > Mathf.Epsilon)
            {
                handle.anchoredPosition -= new Vector2(handle.anchoredPosition.x * autoReturnSpeed, handle.anchoredPosition.y * autoReturnSpeed) * Time.deltaTime;
<<<<<<< Updated upstream
                inputmanager.OnTouchScreenJoystickEvent(Vector2.zero);
=======
                inputManager.OnTouchScreenJoystickEvent(Vector2.zero);  // Send zero vector when no movement
>>>>>>> Stashed changes
            }
            else
            {
                returnToStartPos = false;
            }
        }
    }

<<<<<<< Updated upstream
    //return coordinates
    public Vector2 Coordinates
    {
        get
        {
            if (handle.anchoredPosition.magnitude < radius)
            {
                return handle.anchoredPosition / radius;
            }
            return handle.anchoredPosition.normalized;
        }
    }

    //touch down
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        returnToStartPos = false;
        var handleOffset = GetJoystickOffset(eventData);
        handle.anchoredPosition = handleOffset;
        if (inputmanager != null) inputmanager.OnTouchScreenJoystickEvent(handleOffset.normalized);
=======
    public Vector2 Coordinates
    {
        get
        {
            if (handle.anchoredPosition.magnitude < radius)
            {
                return handle.anchoredPosition / radius;
            }
            return handle.anchoredPosition.normalized;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        returnToStartPos = false;
        var handleOffset = GetJoystickOffset(eventData);
        handle.anchoredPosition = handleOffset;

        Debug.Log($"UIJoystick: OnPointerDown - Joystick Moved to {handleOffset}");

        if (inputManager != null)
        {
            inputManager.OnTouchScreenJoystickEvent(handleOffset.normalized);  // Send joystick direction
        }
>>>>>>> Stashed changes
    }

    //touch drag
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        var handleOffset = GetJoystickOffset(eventData);
        handle.anchoredPosition = handleOffset;
<<<<<<< Updated upstream
        if (inputmanager != null) inputmanager.OnTouchScreenJoystickEvent(handleOffset.normalized);
=======

        Debug.Log($"UIJoystick: OnDrag - Joystick Moved to {handleOffset}");

        if (inputManager != null)
        {
            inputManager.OnTouchScreenJoystickEvent(handleOffset.normalized);  // Send joystick direction
        }
>>>>>>> Stashed changes
    }

    //touch up
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        returnToStartPos = true;
        Debug.Log("UIJoystick: OnPointerUp - Joystick Released");
    }

    //get offset
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