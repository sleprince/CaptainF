using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun; // Add Photon namespace

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
        if (inputManager == null)
        {
            inputManager = FindObjectOfType<InputManager>();
        }

        if (inputManager == null || !inputManager.playerPhotonView.IsMine) return;

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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (inputManager == null || !inputManager.playerPhotonView.IsMine) return;

        returnToStartPos = false;
        var handleOffset = GetJoystickOffset(eventData);
        handle.anchoredPosition = handleOffset;
        inputManager.OnTouchScreenJoystickEvent(handleOffset.normalized);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inputManager == null || !inputManager.playerPhotonView.IsMine) return;

        var handleOffset = GetJoystickOffset(eventData);
        handle.anchoredPosition = handleOffset;
        inputManager.OnTouchScreenJoystickEvent(handleOffset.normalized);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (inputManager == null || !inputManager.playerPhotonView.IsMine) return;

        returnToStartPos = true;
    }

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
