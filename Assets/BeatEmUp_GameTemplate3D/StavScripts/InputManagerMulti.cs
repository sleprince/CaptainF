using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class InputManagerMulti : MonoBehaviour
{
    [Header("Input Type")]
    public INPUTTYPE inputType;
    public List<InputControl> keyBoardControls = new List<InputControl>(); // a list of keyboard control input
    public List<InputControl> joypadControls = new List<InputControl>();   // a list of joypad control input

    [Header("Double Tap Settings")]
    public float doubleTapSpeed = .3f;
    private float lastInputTime = 0f;
    private string lastInputAction = "";

    // Delegates
    public delegate void DirectionInputEventHandler(Vector2 dir, bool doubleTapActive);
    public static event DirectionInputEventHandler onDirectionInputEvent;
    public delegate void InputEventHandler(string action, BUTTONSTATE buttonState);
    public static event InputEventHandler onInputEvent;

    [Space(15)]
    public static bool defendKeyDown;
    private bool isRetrying = false;  // To track retry state

    private PhotonView photonView; // Reference to the PhotonView for multiplayer synchronization

    void Awake()
    {
        // Ensure this script only controls the local player in multiplayer
        photonView = GetComponent<PhotonView>();

        if (!photonView.IsMine)
        {
            enabled = false; // Disable this script for non-local players
        }
    }

    void Start()
    {
        SetDefaultInputType();
    }

    // Set retry flag
    public void SetRetry(bool retrying)
    {
        isRetrying = retrying;
    }

    // Check if the game is retrying
    public bool IsRetrying()
    {
        return isRetrying;
    }

    // Set default input type based on the scene
    void SetDefaultInputType()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Default to keyboard for non-gameplay scenes, touchscreen for gameplay scenes
        if (!isRetrying && (currentSceneName == "01_Game" || currentSceneName == "04_TrainingRoom"))
        {
            inputType = INPUTTYPE.TOUCHSCREEN;
        }
        else if (currentSceneName != "01_Game" && currentSceneName != "04_TrainingRoom")
        {
            inputType = INPUTTYPE.KEYBOARD;
        }
    }

    public static void DirectionEvent(Vector2 dir, bool doubleTapActive)
    {
        if (onDirectionInputEvent != null)
        {
            onDirectionInputEvent(dir, doubleTapActive);
        }
    }

    void Update()
    {
        // Ensure the script only runs for the local player
        if (!photonView.IsMine) return;

        // Touchscreen control overrides everything
        if (inputType == INPUTTYPE.TOUCHSCREEN)
        {
            if (DetectKeyboardInput() || DetectJoypadInput() || DetectJoystickAxis())
            {
                inputType = INPUTTYPE.KEYBOARD;  // Default to both keyboard and joystick
                UIControlSwitcher.HideTouchControlsOverlay();  // Hide touch controls
            }
        }

        // Keyboard and joystick can both run simultaneously if not in touch mode
        if (inputType != INPUTTYPE.TOUCHSCREEN)
        {
            KeyboardControls();
            if (!DetectKeyboardInput())
            {
                JoyPadControls();
            }
        }
    }

    void KeyboardControls()
    {
        float x = 0;
        float y = 0;
        bool doubleTapState = false;

        foreach (InputControl inputControl in keyBoardControls)
        {
            if (onInputEvent == null) return;

            // On keyboard key down
            if (Input.GetKeyDown(inputControl.key))
            {
                doubleTapState = DetectDoubleTap(inputControl.Action);
                onInputEvent(inputControl.Action, BUTTONSTATE.PRESS);
            }

            // On keyboard key up
            if (Input.GetKeyUp(inputControl.key))
            {
                onInputEvent(inputControl.Action, BUTTONSTATE.RELEASE);
            }

            // Convert keyboard direction keys to x, y values
            if (Input.GetKey(inputControl.key))
            {
                if (inputControl.Action == "Left") x = -1f;
                else if (inputControl.Action == "Right") x = 1f;
                else if (inputControl.Action == "Up") y = 1;
                else if (inputControl.Action == "Down") y = -1;
            }

            // Defend key exception (checks the defend state every frame)
            if (inputControl.Action == "Defend")
            {
                defendKeyDown = Input.GetKey(inputControl.key);
                onInputEvent(inputControl.Action, Input.GetKey(inputControl.key) ? BUTTONSTATE.PRESS : BUTTONSTATE.RELEASE);
            }
        }

        // Send a direction event
        DirectionEvent(new Vector2(x, y), doubleTapState);
    }

    void JoyPadControls()
    {
        if (onInputEvent == null) return;

        // On joypad button press
        foreach (InputControl inputControl in joypadControls)
        {
            if (Input.GetKeyDown(inputControl.key))
                onInputEvent(inputControl.Action, BUTTONSTATE.PRESS);

            // On joypad button release
            if (Input.GetKeyUp(inputControl.key))
                onInputEvent(inputControl.Action, BUTTONSTATE.RELEASE);

            // Defend key exception (checks the defend state every frame)
            if (inputControl.Action == "Defend")
            {
                defendKeyDown = Input.GetKey(inputControl.key);
                onInputEvent(inputControl.Action, Input.GetKey(inputControl.key) ? BUTTONSTATE.PRESS : BUTTONSTATE.RELEASE);
            }
        }

        // Get Joypad direction axis
        float x = Input.GetAxis("Joypad Left-Right");
        float y = Input.GetAxis("Joypad Up-Down");

        // Handle joystick axis movement
        if (Mathf.Abs(x) > 0.1f || Mathf.Abs(y) > 0.1f)
        {
            DirectionEvent(new Vector2(x, y).normalized, false);
        }
    }

    // Detects if any keyboard input was pressed
    public bool DetectKeyboardInput()
    {
        foreach (InputControl inputControl in keyBoardControls)
        {
            if (Input.GetKey(inputControl.key))
            {
                return true;
            }
        }
        return false;
    }

    // Detects if any joystick input was pressed (buttons)
    public bool DetectJoypadInput()
    {
        foreach (InputControl inputControl in joypadControls)
        {
            if (Input.GetKey(inputControl.key))
            {
                return true;
            }
        }
        return false;
    }

    // Detects if the joystick axis (stick movement) was used
    public bool DetectJoystickAxis()
    {
        float x = Input.GetAxis("Joypad Left-Right");
        float y = Input.GetAxis("Joypad Up-Down");

        return Mathf.Abs(x) > 0.1f || Mathf.Abs(y) > 0.1f;
    }

    // Clear movement input (to handle stuck movement)
    public void ClearMovementInput()
    {
        DirectionEvent(Vector2.zero, false);  // Send a zero-vector direction event
    }

    // Called when a touch screen button is pressed
    public void OnTouchScreenInputEvent(string action, BUTTONSTATE buttonState)
    {
        if (onInputEvent != null)
        {
            onInputEvent(action, buttonState);

            // Defend exception
            if (action == "Defend") defendKeyDown = (buttonState == BUTTONSTATE.PRESS);
        }
    }

    // This function is used for the touch screen thumb-stick
    public void OnTouchScreenJoystickEvent(Vector2 joystickDir)
    {
        if (onDirectionInputEvent != null)
        {
            DirectionEvent(joystickDir.normalized, false);
        }
    }

    // Returns true if a key double-tap is detected
    bool DetectDoubleTap(string action)
    {
        bool doubleTapDetected = ((Time.time - lastInputTime < doubleTapSpeed) && (lastInputAction == action));
        lastInputAction = action;
        lastInputTime = Time.time;
        return doubleTapDetected;
    }

    //---------------
    //    ENUMS
    //---------------
    [System.Serializable]
    public class InputControl
    {
        public string Action;
        public INPUTTYPE inputType;
        public KeyCode key;
    }

    public enum INPUTTYPE
    {
        KEYBOARD = 0,
        JOYPAD = 5,
        TOUCHSCREEN = 10,
    }

    public enum BUTTONSTATE
    {
        PRESS = 0,
        RELEASE = 5,
        HOLD = 10,
    }

}