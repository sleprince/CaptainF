using Photon.Pun;  // For Photon multiplayer functionality
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InputManager : MonoBehaviour
{

    [Header("Input Type")]
    public bool isNetworkedGame = false;  // Whether this is a networked game
    public INPUTTYPE inputType;  // The current input type (Keyboard, Joypad, or Touchscreen)
    public List<InputControl> keyBoardControls = new List<InputControl>(); // List of keyboard controls
    public List<InputControl> joypadControls = new List<InputControl>();   // List of joypad controls

    [Header("Double Tap Settings")]
    public float doubleTapSpeed = 0.3f;
    private float lastInputTime = 0f;
    private string lastInputAction = "";

    //[Header("Multiplayer Settings")]

    private PhotonView photonView;  // For controlling player-specific input in networked games

    // Delegates
    public delegate void DirectionInputEventHandler(Vector2 dir, bool doubleTapActive);
    public event DirectionInputEventHandler onDirectionInputEvent;
    public delegate void InputEventHandler(string action, BUTTONSTATE buttonState);
    public event InputEventHandler onInputEvent;

    [Space(15)]
    public static bool defendKeyDown;
    private bool isRetrying = false;  // To track retry state

    public int PlayerID;
    public PhotonView playerPhotonView; // Reference to the player's PhotonView

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

    }

    void Start()
    {

        // Determine if this is a networked game or a local game
        isNetworkedGame = PhotonNetwork.IsConnected;  // Check if Photon is connected

        Debug.Log("InputManager initialized for PlayerID: " + PlayerID);

        SetDefaultInputType();

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

    // Set the default input type depending on the scene
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

    void Update()
    {
        if (isNetworkedGame)
        {
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
        }
    }

    // Process input based on input type
    void ProcessInput()
    {
        if (inputType == INPUTTYPE.TOUCHSCREEN)
        {
            // Switch to keyboard if any keyboard or joypad input is detected
            if (DetectKeyboardInput() || DetectJoypadInput() || DetectJoystickAxis())
            {
                inputType = INPUTTYPE.KEYBOARD;
                UIControlSwitcher.HideTouchControlsOverlay();  // Hide the touch control UI
            }
        }

        // Handle both keyboard and joypad input in non-touchscreen modes
        if (inputType == INPUTTYPE.KEYBOARD || inputType == INPUTTYPE.JOYPAD)
        {
            KeyboardControls();
            if (!DetectKeyboardInput())
            {
                JoyPadControls();
            }
        }
    }

    // Handle keyboard input
    void KeyboardControls()
    {
        float x = 0;
        float y = 0;
        bool doubleTapState = false;

        foreach (InputControl inputControl in keyBoardControls)
        {
            if (onInputEvent == null) return;

            // Key down event
            if (Input.GetKeyDown(inputControl.key))
            {
                doubleTapState = DetectDoubleTap(inputControl.Action);
                onInputEvent(inputControl.Action, BUTTONSTATE.PRESS);
            }

            // Key up event
            if (Input.GetKeyUp(inputControl.key))
            {
                onInputEvent(inputControl.Action, BUTTONSTATE.RELEASE);
            }

            // Directional keys (for movement)
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
        onDirectionInputEvent?.Invoke(new Vector2(x, y), doubleTapState);
    }

    // Handle joypad input
    void JoyPadControls()
    {
        if (onInputEvent == null) return;

        foreach (InputControl inputControl in joypadControls)
        {
            if (Input.GetKeyDown(inputControl.key))
                onInputEvent(inputControl.Action, BUTTONSTATE.PRESS);

            if (Input.GetKeyUp(inputControl.key))
                onInputEvent(inputControl.Action, BUTTONSTATE.RELEASE);

            // Defend key handling
            if (inputControl.Action == "Defend")
            {
                defendKeyDown = Input.GetKey(inputControl.key);
                onInputEvent(inputControl.Action, Input.GetKey(inputControl.key) ? BUTTONSTATE.PRESS : BUTTONSTATE.RELEASE);
            }
        }

        // Get Joypad direction axis
        float x = Input.GetAxis("Joypad Left-Right");
        float y = Input.GetAxis("Joypad Up-Down");

        if (Mathf.Abs(x) > 0.1f || Mathf.Abs(y) > 0.1f)
        {
            onDirectionInputEvent?.Invoke(new Vector2(x, y).normalized, false);
        }
    }

    // Detect if any keyboard input was pressed
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

    // Detects joypad input (button presses)
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

    // Detects joystick axis input (stick movement)
    public bool DetectJoystickAxis()
    {
        float x = Input.GetAxis("Joypad Left-Right");
        float y = Input.GetAxis("Joypad Up-Down");

        return Mathf.Abs(x) > 0.1f || Mathf.Abs(y) > 0.1f;
    }

    // Clear movement input to handle "stuck" movement scenarios
    public void ClearMovementInput()
    {
        onDirectionInputEvent?.Invoke(Vector2.zero, false);  // Send zero-vector direction event
    }

    // Called when a touch screen button is pressed
    public void OnTouchScreenInputEvent(string action, BUTTONSTATE buttonState)
    {
        if (onInputEvent != null)
        {
            onInputEvent(action, buttonState);

            // Defend key exception
            if (action == "Defend") defendKeyDown = (buttonState == BUTTONSTATE.PRESS);
        }
    }

    // Called when the touchscreen joystick is used
    public void OnTouchScreenJoystickEvent(Vector2 joystickDir)
    {
        if (onDirectionInputEvent != null)
        {
            onDirectionInputEvent(joystickDir.normalized, false);
        }
    }

    // Detect double tap for input action
    bool DetectDoubleTap(string action)
    {
        bool doubleTapDetected = ((Time.time - lastInputTime < doubleTapSpeed) && (lastInputAction == action));
        lastInputAction = action;
        lastInputTime = Time.time;
        return doubleTapDetected;
    }
}

// Enums
[System.Serializable]
public class InputControl
{
    public string Action;
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

// Editor Script
#if UNITY_EDITOR
[CustomEditor(typeof(InputManager))]
public class InputManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InputManager inputManager = (InputManager)target;
        EditorGUIUtility.labelWidth = 120;
        EditorGUIUtility.fieldWidth = 100;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Input Type", EditorStyles.boldLabel);
        inputManager.inputType = (INPUTTYPE)EditorGUILayout.EnumPopup("Input Type:", inputManager.inputType);
        GUILayout.Space(15);

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

        if (inputManager.inputType == INPUTTYPE.TOUCHSCREEN)
        {
            EditorGUILayout.LabelField("* You can edit the touchscreen buttons in the 'UI' prefab.");
        }

        GUILayout.Space(15);

        if (inputManager.inputType == INPUTTYPE.KEYBOARD || inputManager.inputType == INPUTTYPE.JOYPAD)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Input Action", GUILayout.Width(130), GUILayout.Height(25)))
            {
                if (inputManager.inputType == INPUTTYPE.KEYBOARD) inputManager.keyBoardControls.Add(new InputControl());
                if (inputManager.inputType == INPUTTYPE.JOYPAD) inputManager.joypadControls.Add(new InputControl());
            }

            bool showDeleteButton = (inputManager.inputType == INPUTTYPE.KEYBOARD && inputManager.keyBoardControls.Count > 0) ||
                                    (inputManager.inputType == INPUTTYPE.JOYPAD && inputManager.joypadControls.Count > 0);
            if (showDeleteButton && GUILayout.Button("Delete Input Action", GUILayout.Width(130), GUILayout.Height(25)))
            {
                if (inputManager.inputType == INPUTTYPE.KEYBOARD && inputManager.keyBoardControls.Count > 0)
                    inputManager.keyBoardControls.RemoveAt(inputManager.keyBoardControls.Count - 1);
                if (inputManager.inputType == INPUTTYPE.JOYPAD && inputManager.joypadControls.Count > 0)
                    inputManager.joypadControls.RemoveAt(inputManager.joypadControls.Count - 1);
            }
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.LabelField("Double Tap Settings", EditorStyles.boldLabel);
        inputManager.doubleTapSpeed = EditorGUILayout.FloatField("Double Tap Speed:", inputManager.doubleTapSpeed);
        EditorUtility.SetDirty(inputManager);
    }
}
#endif
