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
    }

    // Called when the Keyboard button is clicked
    public void SetKeyboardControls()
    {
        // Ensure InputManager reference is valid
        if (inputManager != null)
        {
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
        // Ensure InputManager reference is valid
        if (inputManager != null)
        {
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
        // Ensure InputManager reference is valid
        if (inputManager != null)
        {
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
}
