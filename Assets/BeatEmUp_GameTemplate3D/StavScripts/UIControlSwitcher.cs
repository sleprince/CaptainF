using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIControlSwitcher : MonoBehaviour
{
    private InputManager inputManager;
    public GameObject touchControlsOverlay;

    private bool isTouchModeActive = true;

    // Button references (assigned in the Unity Editor)
    public Button touchscreenButton;

    void Start()
    {
        InvokeRepeating("FindInputManager", 0f, 0.5f); // Keep trying to find the InputManager every 0.5 seconds

        if (touchscreenButton != null)
        {
            touchscreenButton.onClick.AddListener(ToggleTouchscreenControls);
        }

        // Default to touchscreen controls at the start
        SetTouchscreenControls(true);
    }

    void Update()
    {
        if (inputManager == null)
        {
            return; // Avoid null references if inputManager hasn't been found yet
        }

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

        if (inputManager != null)
        {
            inputManager.inputType = active ? INPUTTYPE.TOUCHSCREEN : INPUTTYPE.KEYBOARD;
        }
    }

    public static void HideTouchControlsOverlay()
    {
        var switcher = FindObjectOfType<UIControlSwitcher>();
        if (switcher != null && switcher.touchControlsOverlay != null)
        {
            switcher.touchControlsOverlay.SetActive(false);
        }
    }

    // Find the InputManager in the scene
    private void FindInputManager()
    {
        if (inputManager != null) return;  // Stop searching if already found

        if (PhotonNetwork.InRoom)
        {
            // Look for the local player's Photon Player prefab
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                var photonView = player.GetComponent<PhotonView>();
                if (photonView != null && photonView.IsMine)
                {
                    // Look for the InputManager inside the local player's prefab
                    inputManager = player.GetComponentInChildren<InputManager>();
                    if (inputManager != null)
                    {
                        Debug.Log("InputManager found inside local player's Photon prefab.");
                        CancelInvoke("FindInputManager"); // Stop searching after it's found
                        break;
                    }
                }
            }
        }
        else
        {
            // Local play - find the InputManager if it exists
            inputManager = FindObjectOfType<InputManager>();
            if (inputManager != null)
            {
                Debug.Log("InputManager found for local play.");
                CancelInvoke("FindInputManager"); // Stop searching after it's found
            }
        }

        if (inputManager == null)
        {
            Debug.LogError("InputManager not found.");
        }
    }
}
