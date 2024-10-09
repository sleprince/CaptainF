using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public string actionDown = "";
    public string actionUp = "";
    private InputManager inputmanager;

    void Update()
    {
        if (inputmanager == null) inputmanager = GameObject.FindObjectOfType<InputManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
<<<<<<< Updated upstream
        if (inputmanager != null && actionDown != "") inputmanager.OnTouchScreenInputEvent(actionDown, BUTTONSTATE.PRESS);
=======
        Debug.Log($"UIButton: OnPointerDown - Action {actionDown} Pressed");
        if (inputManager != null && actionDown != "")
        {
            inputManager.OnTouchScreenInputEvent(actionDown, BUTTONSTATE.PRESS);
        }
>>>>>>> Stashed changes
    }

    public void OnPointerUp(PointerEventData eventData)
    {
<<<<<<< Updated upstream
        if (inputmanager != null && actionUp != "") inputmanager.OnTouchScreenInputEvent(actionUp, BUTTONSTATE.RELEASE);
=======
        Debug.Log($"UIButton: OnPointerUp - Action {actionUp} Released");
        if (inputManager != null && actionUp != "")
        {
            inputManager.OnTouchScreenInputEvent(actionUp, BUTTONSTATE.RELEASE);
        }
>>>>>>> Stashed changes
    }
}