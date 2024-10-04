using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string actionDown = "";
    public string actionUp = "";
    private InputManager inputManager;

    void Update()
    {
        if (inputManager == null)
        {
            inputManager = FindObjectOfType<InputManager>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"UIButton: OnPointerDown - Action {actionDown} Pressed");
        if (inputManager != null && actionDown != "")
        {
            inputManager.OnTouchScreenInputEvent(actionDown, BUTTONSTATE.PRESS);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"UIButton: OnPointerUp - Action {actionUp} Released");
        if (inputManager != null && actionUp != "")
        {
            inputManager.OnTouchScreenInputEvent(actionUp, BUTTONSTATE.RELEASE);
        }
    }
}
