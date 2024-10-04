using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string actionDown = "";
    public string actionUp = "";
    private InputManager inputmanager;
    private bool isLocalPlayer;

    void Start()
    {
        // Check if the button belongs to the local player in multiplayer mode
        if (PhotonNetwork.InRoom)
        {
            PhotonView photonView = GetComponentInParent<PhotonView>();
            if (photonView != null)
            {
                isLocalPlayer = photonView.IsMine;
            }
        }
        else
        {
            isLocalPlayer = true; // Local play
        }
    }

    void Update()
    {
        if (inputmanager == null) inputmanager = GameObject.FindObjectOfType<InputManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isLocalPlayer) return;

        if (inputmanager != null && actionDown != "") inputmanager.OnTouchScreenInputEvent(actionDown, BUTTONSTATE.PRESS);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isLocalPlayer) return;

        if (inputmanager != null && actionUp != "") inputmanager.OnTouchScreenInputEvent(actionUp, BUTTONSTATE.RELEASE);
    }
}
