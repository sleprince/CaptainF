using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun; // Add Photon namespace

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
        if (inputManager == null || !inputManager.playerPhotonView.IsMine) return;

        if (actionDown != "")
        {
            inputManager.OnTouchScreenInputEvent(actionDown, BUTTONSTATE.PRESS);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (inputManager == null || !inputManager.playerPhotonView.IsMine) return;

        if (actionUp != "")
        {
            inputManager.OnTouchScreenInputEvent(actionUp, BUTTONSTATE.RELEASE);
        }
    }
}
