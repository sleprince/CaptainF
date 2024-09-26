using UnityEngine;

public class UIControlSwitcher : MonoBehaviour
{
    private InputManager inputManager;

    // The InputManager prefab (drag your InputManager prefab here in the Inspector)
    public GameObject inputManagerPrefab;

    void Start()
    {
        // Ensure we find the InputManager after it's instantiated at runtime
        FindInputManager();
    }

    // Try to find the InputManager in the scene after the game starts
    void FindInputManager()
    {
        // Check if InputManager already exists in the scene
        inputManager = FindObjectOfType<InputManager>();

        if (inputManager == null && inputManagerPrefab != null)
        {
            // Instantiate the InputManager if it's not found in the scene
            GameObject inputManagerInstance = Instantiate(inputManagerPrefab);
            inputManager = inputManagerInstance.GetComponent<InputManager>();

            if (inputManager == null)
            {
                Debug.LogError("InputManager component not found on instantiated prefab!");
            }
            else
            {
                Debug.Log("InputManager found and set.");
            }
        }
        else
        {
            Debug.Log("InputManager already exists in the scene.");
        }
    }

    // Call this when the button for keyboard is pressed
    public void SwitchToKeyboard()
    {
        if (inputManager == null) FindInputManager();

        if (inputManager != null)
        {
            inputManager.inputType = INPUTTYPE.KEYBOARD;
            Debug.Log("Switched to Keyboard Controls");
        }
    }

    // Call this when the button for joystick is pressed
    public void SwitchToJoystick()
    {
        if (inputManager == null) FindInputManager();

        if (inputManager != null)
        {
            inputManager.inputType = INPUTTYPE.JOYPAD;
            Debug.Log("Switched to Joystick Controls");
        }
    }

    // Call this when the button for touchscreen is pressed
    public void SwitchToTouchscreen()
    {
        if (inputManager == null) FindInputManager();

        if (inputManager != null)
        {
            inputManager.inputType = INPUTTYPE.TOUCHSCREEN;
            Debug.Log("Switched to Touchscreen Controls");
        }
    }
}
