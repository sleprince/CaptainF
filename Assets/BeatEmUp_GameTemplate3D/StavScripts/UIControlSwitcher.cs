using UnityEngine;
using UnityEngine.UI;

public class UIControlSwitcher : MonoBehaviour
{
    private InputManager inputManager;
    public GameObject touchControlsOverlay;

    private bool isTouchModeActive = true;

    // Button references (assigned in the Unity Editor)
    public Button touchscreenButton;

    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();

        if (inputManager == null)
        {
            Debug.LogError("InputManager not found.");
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
