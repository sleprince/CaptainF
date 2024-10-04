using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

[RequireComponent(typeof(RectTransform))]
public class UIJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform handle;
    public float radius = 40f;
    public float autoReturnSpeed = 8f;
    private bool returnToStartPos;
    private RectTransform parentRect;
    private InputManager inputmanager;
    private bool isLocalPlayer;

    void OnEnable()
    {
        returnToStartPos = true;
        handle.transform.SetParent(transform);
        parentRect = GetComponent<RectTransform>();

        // Check if the object is for the local player in multiplayer mode
        if (PhotonNetwork.InRoom)
        {
            PhotonView photonView = GetComponentInParent<PhotonView>();
            if (photonView != null)
            {
                isLocalPlayer = photonView.IsMine; // Check if this is the local player's joystick
            }
        }
        else
        {
            isLocalPlayer = true; // Local play
        }
    }

    void Update()
    {
        if (inputmanager == null)
        {
            inputmanager = GameObject.FindObjectOfType<InputManager>();
        }

        if (returnToStartPos)
        {
            if (handle.anchoredPosition.magnitude > Mathf.Epsilon)
            {
                handle.anchoredPosition -= new Vector2(handle.anchoredPosition.x * autoReturnSpeed, handle.anchoredPosition.y * autoReturnSpeed) * Time.deltaTime;
                if (isLocalPlayer && inputmanager != null)
                {
                    inputmanager.OnTouchScreenJoystickEvent(Vector2.zero);
                }
            }
            else
            {
                returnToStartPos = false;
            }
        }
    }

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
        if (!isLocalPlayer) return;

        returnToStartPos = false;
        var handleOffset = GetJoystickOffset(eventData);
        handle.anchoredPosition = handleOffset;
        if (inputmanager != null) inputmanager.OnTouchScreenJoystickEvent(handleOffset.normalized);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isLocalPlayer) return;

        var handleOffset = GetJoystickOffset(eventData);
        handle.anchoredPosition = handleOffset;
        if (inputmanager != null) inputmanager.OnTouchScreenJoystickEvent(handleOffset.normalized);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isLocalPlayer) return;

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
