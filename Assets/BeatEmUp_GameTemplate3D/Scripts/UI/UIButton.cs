using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string actionDown = "";
    public string actionUp = "";
    private InputManager inputManager;

    void Update()
    {
        if (inputManager == null) inputManager = GameObject.FindObjectOfType<InputManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (inputManager.inputType != INPUTTYPE.TOUCHSCREEN) return;

        if (inputManager != null && actionDown != "") inputManager.OnTouchScreenInputEvent(actionDown, BUTTONSTATE.PRESS);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (inputManager.inputType != INPUTTYPE.TOUCHSCREEN) return;

        if (inputManager != null && actionUp != "") inputManager.OnTouchScreenInputEvent(actionUp, BUTTONSTATE.RELEASE);
    }
}
