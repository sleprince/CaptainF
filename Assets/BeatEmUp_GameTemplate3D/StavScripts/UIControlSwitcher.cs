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
        AssignInputManager();

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

    void OnEnable()
    {
        // Reassign InputManager when the UI is enabled
        AssignInputManager();
    }

    // This method tries to find the InputManager again, in case it was destroyed and recreated
    private void AssignInputManager()
    {
        inputManager = FindObjectOfType<InputManager>();

        if (inputManager == null)
        {
            Debug.LogError("InputManager not found in the scene. Make sure it's instantiated before trying to switch controls.");
        }
    }

    // Called when the Keyboard button is clicked
    public void SetKeyboardControls()
    {
        if (inputManager != null)
        {
            inputManager.ClearMovementInput();
            inputManager.inputType = INPUTTYPE.KEYBOARD;

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

            if (touchControlsOverlay != null)
            {
                touchControlsOverlay.SetActive(true);
            }
        }
    }
}
