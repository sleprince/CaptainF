using UnityEngine;
using UnityEngine.UI;  // Needed to reference buttons

public class UIControlSwitcher : MonoBehaviour
{
    private InputManager inputManager;  // Reference to the InputManager
    public GameObject touchControlsOverlay;  // Reference to the touch controls overlay

    // Button references (assign these in the Unity Editor)
    public Button keyboardButton;
    public Button joypadButton;
    public Button touchscreenButton;

    void Start()
    {
        // Try to find the InputManager in the scene
        inputManager = FindObjectOfType<InputManager>();

        if (inputManager == null)
        {
            Debug.LogError("InputManager not found in the scene. Make sure it's instantiated before trying to switch controls.");
        }

        // Attach button listeners to switch control types
        if (keyboardButton != null)
        {
            keyboardButton.onClick.AddListener(SetKeyboardControls);
        }

        if (joypadButton != null)
        {
            joypadButton.onClick.AddListener(SetJoypadControls);
        }

        if (touchscreenButton != null)
        {
            touchscreenButton.onClick.AddListener(SetTouchscreenControls);
        }

        // Set default controls (touchscreen) if not a retry, else handle retry case
        if (inputManager != null)
        {
            if (!inputManager.IsRetrying())
            {
                SetDefaultToTouchscreen();
            }
            else
            {
                HandleRetryControlState();
            }
        }
    }

    void OnEnable()
    {
        // Try to find the InputManager again in case it's not yet instantiated
        if (inputManager == null)
        {
            inputManager = FindObjectOfType<InputManager>();
        }

        // Ensure InputManager is found before proceeding
        if (inputManager != null)
        {
            if (!inputManager.IsRetrying())
            {
                SetDefaultToTouchscreen();
            }
            else
            {
                HandleRetryControlState();
            }
        }
    }

    // Called when the Keyboard button is clicked
    public void SetKeyboardControls()
    {
        if (inputManager != null)
        {
            inputManager.ClearMovementInput();  // Clear movement input before switching to avoid stuck movement
            inputManager.inputType = INPUTTYPE.KEYBOARD;

            // Disable touchscreen overlay
            if (touchControlsOverlay != null)
            {
                touchControlsOverlay.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("InputManager is missing or not referenced.");
        }
    }

    // Called when the Joypad button is clicked
    public void SetJoypadControls()
    {
        if (inputManager != null)
        {
            inputManager.ClearMovementInput();  // Clear movement input before switching to avoid stuck movement
            inputManager.inputType = INPUTTYPE.JOYPAD;

            // Disable touchscreen overlay
            if (touchControlsOverlay != null)
            {
                touchControlsOverlay.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("InputManager is missing or not referenced.");
        }
    }

    // Called when the Touchscreen button is clicked
    public void SetTouchscreenControls()
    {
        if (inputManager != null)
        {
            inputManager.ClearMovementInput();  // Clear movement input before switching to avoid stuck movement
            inputManager.inputType = INPUTTYPE.TOUCHSCREEN;

            // Enable touchscreen overlay
            if (touchControlsOverlay != null)
            {
                touchControlsOverlay.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("InputManager is missing or not referenced.");
        }
    }

    // Set default controls to touchscreen (only if not retrying)
    private void SetDefaultToTouchscreen()
    {
        if (inputManager != null)
        {
            inputManager.inputType = INPUTTYPE.TOUCHSCREEN;

            // Enable touchscreen overlay
            if (touchControlsOverlay != null)
            {
                touchControlsOverlay.SetActive(true);
            }
        }
    }

    // Handle control state when retrying (ensure overlay visibility matches control type)
    private void HandleRetryControlState()
    {
        if (inputManager != null)
        {
            // Check if the input type was set to TOUCHSCREEN
            if (inputManager.inputType == INPUTTYPE.TOUCHSCREEN)
            {
                // Enable touchscreen overlay
                if (touchControlsOverlay != null)
                {
                    touchControlsOverlay.SetActive(true);
                }
            }
            else
            {
                // Hide touchscreen overlay for non-touchscreen controls
                if (touchControlsOverlay != null)
                {
                    touchControlsOverlay.SetActive(false);
                }
            }
        }
    }
}
