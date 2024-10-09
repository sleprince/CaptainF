using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InputManager : MonoBehaviour
{
    public static InputManager instance;  // Singleton instance

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
    public event DirectionInputEventHandler onDirectionInputEvent;
    public delegate void InputEventHandler(string action, BUTTONSTATE buttonState);
    public event InputEventHandler onInputEvent;

    [Space(15)]
    public static bool defendKeyDown;
    private bool isRetrying = false;  // To track retry state

    void Awake()
    {
        // Implementing Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Ensure InputManager persists between scene changes
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instance
        }
    }

    void Start()
    {
        SetDefaultInputType();
<<<<<<< Updated upstream
=======

        // Attempt to find and assign the player's PhotonView if it wasn't set
        if (isNetworkedGame && playerPhotonView == null)
        {
            foreach (GameObject playerObject in GameObject.FindGameObjectsWithTag("Player")) // Assuming your players have the "Player" tag
            {
                PhotonView view = playerObject.GetComponent<PhotonView>();
                if (view != null && view.Owner == PhotonNetwork.LocalPlayer)
                {
                    playerPhotonView = view;
                    Debug.Log("Player's PhotonView assigned in Start for PlayerID: " + PlayerID);
                    break;
                }
            }
        }
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            onDirectionInputEvent(dir, doubleTapActive);
=======
            // Only process input if the playerPhotonView belongs to the local player
            if (playerPhotonView.IsMine)
            {
                ProcessInput();
            }
        }
        else
        {
            // Local play
            ProcessInput();
>>>>>>> Stashed changes
        }
    }

    void Update()
    {
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

<<<<<<< Updated upstream
=======


    // Handle keyboard input
    // Handle keyboard input
>>>>>>> Stashed changes
    void KeyboardControls()
    {
        float x = 0;
        float y = 0;
        bool doubleTapState = false;

        // Exit early if this is a networked game and the local player doesn't own this object
        if (isNetworkedGame && (playerPhotonView == null || !playerPhotonView.IsMine))
        {
            return;
        }

        foreach (InputControl inputControl in keyBoardControls)
        {
<<<<<<< Updated upstream
            if (onInputEvent == null) return;

            // On keyboard key down
            if (Input.GetKeyDown(inputControl.key))
            {
                doubleTapState = DetectDoubleTap(inputControl.Action);
                Debug.Log($"Key Down: {inputControl.Action} (Key: {inputControl.key})");  // Debug for all key presses
                onInputEvent(inputControl.Action, BUTTONSTATE.PRESS);
=======
            // Key down event
            if (Input.GetKeyDown(inputControl.key))
            {
                doubleTapState = DetectDoubleTap(inputControl.Action);
                onInputEvent?.Invoke(inputControl.Action, BUTTONSTATE.PRESS);
>>>>>>> Stashed changes
            }

            // On keyboard key up
            if (Input.GetKeyUp(inputControl.key))
            {
<<<<<<< Updated upstream
                Debug.Log($"Key Up: {inputControl.Action} (Key: {inputControl.key})");  // Debug for key releases
                onInputEvent(inputControl.Action, BUTTONSTATE.RELEASE);
            }

            // Convert keyboard direction keys to x, y values
=======
                onInputEvent?.Invoke(inputControl.Action, BUTTONSTATE.RELEASE);
            }

            // Handle directional movement input
>>>>>>> Stashed changes
            if (Input.GetKey(inputControl.key))
            {
                if (inputControl.Action == "Left") x = -1f;
                else if (inputControl.Action == "Right") x = 1f;
                else if (inputControl.Action == "Up") y = 1;
                else if (inputControl.Action == "Down") y = -1;
            }

            // Special handling for Defend key (continuous check every frame)
            if (inputControl.Action == "Defend")
            {
                Debug.Log($"Defend Key Pressed: {Input.GetKey(inputControl.key)}");  // Debug to check Defend key detection
                defendKeyDown = Input.GetKey(inputControl.key);
<<<<<<< Updated upstream
                Debug.Log($"Defend state: {(defendKeyDown ? "Pressed" : "Released")}");
                onInputEvent(inputControl.Action, Input.GetKey(inputControl.key) ? BUTTONSTATE.PRESS : BUTTONSTATE.RELEASE);
            }
        }

        // Send a direction event
        DirectionEvent(new Vector2(x, y), doubleTapState);
=======
                onInputEvent?.Invoke(inputControl.Action, Input.GetKey(inputControl.key) ? BUTTONSTATE.PRESS : BUTTONSTATE.RELEASE);
            }
        }

        // Send directional input to instance-based direction input event handler
        onDirectionInputEvent?.Invoke(new Vector2(x, y), doubleTapState);
    

        // Send a direction event
        if (isNetworkedGame && (playerPhotonView != null && playerPhotonView.IsMine))
        {
            onDirectionInputEvent?.Invoke(new Vector2(x, y), doubleTapState);
        }
>>>>>>> Stashed changes
    }

    void JoyPadControls()
    {
        if (onInputEvent == null) return;

        // On joypad button press
        foreach (InputControl inputControl in joypadControls)
        {
            if (Input.GetKeyDown(inputControl.key))
                onInputEvent?.Invoke(inputControl.Action, BUTTONSTATE.PRESS);

            // On joypad button release
            if (Input.GetKeyUp(inputControl.key))
                onInputEvent?.Invoke(inputControl.Action, BUTTONSTATE.RELEASE);

<<<<<<< Updated upstream
            // Defend key exception (checks the defend state every frame)
=======
>>>>>>> Stashed changes
            if (inputControl.Action == "Defend")
            {
                defendKeyDown = Input.GetKey(inputControl.key);
                onInputEvent?.Invoke(inputControl.Action, Input.GetKey(inputControl.key) ? BUTTONSTATE.PRESS : BUTTONSTATE.RELEASE);
            }


            float x = Input.GetAxis("Joypad Left-Right");
            float y = Input.GetAxis("Joypad Up-Down");

            // Check if the joystick is neutral
            if (Mathf.Abs(x) > 0.1f || Mathf.Abs(y) > 0.1f)
            {
                onDirectionInputEvent?.Invoke(new Vector2(x, y).normalized, false);
            }
            else
            {
                onDirectionInputEvent?.Invoke(Vector2.zero, false); // Stop movement when joystick is neutral
            }

        }

<<<<<<< Updated upstream
        // Get Joypad direction axis
        float x = Input.GetAxis("Joypad Left-Right");
        float y = Input.GetAxis("Joypad Up-Down");

        // Handle joystick axis movement
        if (Mathf.Abs(x) > 0.1f || Mathf.Abs(y) > 0.1f)
        {
            DirectionEvent(new Vector2(x, y).normalized, false);
        }
=======
>>>>>>> Stashed changes
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

//-------------
//   EDITOR SCRIPT
//-------------
#if UNITY_EDITOR
[CustomEditor(typeof(InputManager))]
public class InputManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        InputManager inputManager = (InputManager)target;
        EditorGUIUtility.labelWidth = 120;
        EditorGUIUtility.fieldWidth = 100;

        //input type
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Input Type", EditorStyles.boldLabel);
        inputManager.inputType = (INPUTTYPE)EditorGUILayout.EnumPopup("Input Type:", inputManager.inputType);
        GUILayout.Space(15);

        //keyboard controls	
        if (inputManager.inputType == INPUTTYPE.KEYBOARD)
        {
            EditorGUILayout.LabelField("Keyboard Keys", EditorStyles.boldLabel);
            foreach (InputControl inputControl in inputManager.keyBoardControls)
            {
                GUILayout.BeginHorizontal();
                inputControl.Action = EditorGUILayout.TextField("Action:", inputControl.Action);
                inputControl.key = (KeyCode)EditorGUILayout.EnumPopup("Key:", inputControl.key, GUILayout.Width(350));
                GUILayout.EndHorizontal();
            }
        }

        //joypad controls	
        if (inputManager.inputType == INPUTTYPE.JOYPAD)
        {
            EditorGUILayout.LabelField("Joypad Keys", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("* The direction keys are mapped onto the joypad thumbstick.");

            foreach (InputControl inputControl in inputManager.joypadControls)
            {
                GUILayout.BeginHorizontal();
                inputControl.Action = EditorGUILayout.TextField("Action:", inputControl.Action);
                inputControl.key = (KeyCode)EditorGUILayout.EnumPopup("Key:", inputControl.key, GUILayout.Width(350));
                GUILayout.EndHorizontal();
            }
        }

        //touch Screen controls
        if (inputManager.inputType == INPUTTYPE.TOUCHSCREEN)
        {
            EditorGUILayout.LabelField("* You can edit the touchscreen buttons in the 'UI' prefab in the project folder.");
            EditorGUILayout.LabelField("   Inside the prefab go to: UI/Canvas/TouchScreenControls");
        }
        GUILayout.Space(15);

        if (inputManager.inputType == INPUTTYPE.KEYBOARD || inputManager.inputType == INPUTTYPE.JOYPAD)
        {
            GUILayout.BeginHorizontal();

            //button: add a new action 
            if (GUILayout.Button("Add Input Action", GUILayout.Width(130), GUILayout.Height(25)))
            {
                if (inputManager.inputType == INPUTTYPE.KEYBOARD) inputManager.keyBoardControls.Add(new InputControl());
                if (inputManager.inputType == INPUTTYPE.JOYPAD) inputManager.joypadControls.Add(new InputControl());
            }

            //button: delete last action 
            bool showDeleteButton = (inputManager.inputType == INPUTTYPE.KEYBOARD && inputManager.keyBoardControls.Count > 0) || (inputManager.inputType == INPUTTYPE.JOYPAD && inputManager.joypadControls.Count > 0) ? true : false;
            if (showDeleteButton && GUILayout.Button("Delete Input Action", GUILayout.Width(130), GUILayout.Height(25)))
            {
                if (inputManager.inputType == INPUTTYPE.KEYBOARD && inputManager.keyBoardControls.Count > 0) inputManager.keyBoardControls.RemoveAt(inputManager.keyBoardControls.Count - 1);
                if (inputManager.inputType == INPUTTYPE.JOYPAD && inputManager.joypadControls.Count > 0) inputManager.joypadControls.RemoveAt(inputManager.joypadControls.Count - 1);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(15);
        }

        //double tap settings
        EditorGUILayout.LabelField("Double Tap Settings", EditorStyles.boldLabel);
        inputManager.doubleTapSpeed = EditorGUILayout.FloatField("Double Tap Speed:", inputManager.doubleTapSpeed);
        EditorUtility.SetDirty(inputManager);
    }
}
#endif