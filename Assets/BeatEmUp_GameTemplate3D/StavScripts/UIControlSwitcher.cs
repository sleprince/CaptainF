using UnityEngine;
using UnityEngine.UI;
<<<<<<< Updated upstream
=======
using Photon.Pun;
using System.Collections;
using System.Linq;
>>>>>>> Stashed changes

public class UIControlSwitcher : MonoBehaviour
{
    private InputManager inputManager;
    public GameObject touchControlsOverlay;

    private bool isTouchModeActive = true;

    // Button references (assigned in the Unity Editor)
    public Button touchscreenButton;

    void Start()
    {
<<<<<<< Updated upstream
        inputManager = FindObjectOfType<InputManager>();

        if (inputManager == null)
        {
            Debug.LogError("InputManager not found.");
=======
        if (PhotonNetwork.IsConnected)
        {
            //inputManager = FindObjectsOfType<InputManager>().FirstOrDefault(im => im.isNetworkedGame && im.playerPhotonView != null && im.playerPhotonView.IsMine);
            InvokeRepeating("FindLocalPlayerInputManager", 0f, 0.5f);
        }
        else
        {
            // Start searching for the InputManager with repeated attempts
            InvokeRepeating("FindLocalPlayerInputManager", 0f, 0.5f);
>>>>>>> Stashed changes
        }

        if (touchscreenButton != null)
        {
            touchscreenButton.onClick.AddListener(ToggleTouchscreenControls);
        }

        // Default to touchscreen controls at the start
        SetTouchscreenControls(true);
    }

    void Update()
    {
        if (!isTouchModeActive && (inputManager.DetectKeyboardInput() || inputManager.DetectJoypadInput()))
        {
            // Automatically exit touchscreen mode
            SetTouchscreenControls(false);
        }
    }

    public void ToggleTouchscreenControls()
    {
        isTouchModeActive = !isTouchModeActive;
        SetTouchscreenControls(isTouchModeActive);
    }

    private void SetTouchscreenControls(bool active)
    {
        if (touchControlsOverlay != null)
        {
            touchControlsOverlay.SetActive(active);
        }

        inputManager.inputType = active ? INPUTTYPE.TOUCHSCREEN : INPUTTYPE.KEYBOARD;
    }

    public static void HideTouchControlsOverlay()
    {
        // Static method to hide the touch overlay, called when switching to other inputs
        var switcher = FindObjectOfType<UIControlSwitcher>();
        if (switcher != null && switcher.touchControlsOverlay != null)
        {
            switcher.touchControlsOverlay.SetActive(false);
        }
    }
}
