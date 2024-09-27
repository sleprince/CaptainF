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

        // Try to set up InputManager after the initial setup in the scene
        InitializeInputManager();
    }

    void OnEnable()
    {
        // Try to set up InputManager every time the object is enabled (such as after respawn)
        InitializeInputManager();
    }

    // Called when the Keyboard button is clicked
    public void SetKeyboardControls()
    {
        if (inputManager != null)
        {
            inputManager.ClearMovementInput();
            inputManager.inputType = INPUTTYPE.KEYBOARD;

            // Disable touchscreen overlay
            if (touchControlsOverlay != null)
            {
                touchControlsOverlay.SetActive(false);
            }
        }
    }

    // Called when the Joypad button is clicked
    public void SetJoypadControls()
    {
        if (inputManager != null)
        {
            inputManager.ClearMovementInput();
            inputManager.inputType = INPUTTYPE.JOYPAD;

            // Disable touchscreen overlay
            if (touchControlsOverlay != null)
            {
                touchControlsOverlay.SetActive(false);
            }
        }
    }

    // Called when the Touchscreen button is clicked
    public void SetTouchscreenControls()
    {
        if (inputManager != null)
        {
            inputManager.ClearMovementInput();
            inputManager.inputType = INPUTTYPE.TOUCHSCREEN;

            // Enable touchscreen overlay
            if (touchControlsOverlay != null)
            {
                touchControlsOverlay.SetActive(true);
            }
        }
    }

    // This ensures that the InputManager is correctly found after the game has started or reloaded
    private void InitializeInputManager()
    {
        // Try to find the InputManager in the scene
        inputManager = FindObjectOfType<InputManager>();

        if (inputManager != null)
        {
            // Set default input to touchscreen
            inputManager.inputType = INPUTTYPE.TOUCHSCREEN;

            // Enable touchscreen overlay at the start
            if (touchControlsOverlay != null)
            {
                touchControlsOverlay.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("InputManager not found in the scene. Make sure it's instantiated before trying to switch controls.");
        }
    }
}
