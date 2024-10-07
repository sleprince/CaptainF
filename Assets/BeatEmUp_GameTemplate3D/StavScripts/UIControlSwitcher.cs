using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class UIControlSwitcher : MonoBehaviour
{
    private InputManager inputManager;
    public GameObject touchControlsOverlay;

    private bool isTouchModeActive = true;

    // Button references (assigned in the Unity Editor)
    public Button touchscreenButton;

    void Start()
    {
        // Start searching for the InputManager with repeated attempts
        InvokeRepeating("FindLocalPlayerInputManager", 0f, 0.5f);

        if (touchscreenButton != null)
        {
            touchscreenButton.onClick.AddListener(ToggleTouchscreenControls);
        }

        // Default to touchscreen controls at the start
        SetTouchscreenControls(true);

        StartCoroutine(DelayedTouchscreenToggle());
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

    private IEnumerator DelayedTouchscreenToggle()
    {
        // Wait for 0.5 seconds before toggling
        yield return new WaitForSeconds(0.5f);

        // Effectively "press" the toggle button on and off to ensure controls work correctly
        SetTouchscreenControls(false);
        SetTouchscreenControls(true);
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

    // Find the InputManager for the local player
    private void FindLocalPlayerInputManager()
    {
        if (inputManager != null) return;  // Stop searching if already found

        if (PhotonNetwork.InRoom)
        {
            int localPlayerID = PhotonNetwork.LocalPlayer.ActorNumber; // Get the local player's Photon Actor Number

            // Look for the InputManager by the naming convention
            string inputManagerName = "InputManager_Player" + localPlayerID;
            GameObject inputManagerObject = GameObject.Find(inputManagerName);

            if (inputManagerObject != null)
            {
                inputManager = inputManagerObject.GetComponent<InputManager>();
                if (inputManager != null)
                {
                    Debug.Log("InputManager found: " + inputManagerName);
                    CancelInvoke("FindLocalPlayerInputManager"); // Stop searching after it's found
                }
            }
        }
        else
        {
            // For local play, find the InputManager
            inputManager = FindObjectOfType<InputManager>();
            if (inputManager != null)
            {
                Debug.Log("InputManager found for local play.");
                CancelInvoke("FindLocalPlayerInputManager"); // Stop searching after it's found
            }
        }

        if (inputManager == null)
        {
            Debug.LogError("InputManager not found.");
        }
    }
}
