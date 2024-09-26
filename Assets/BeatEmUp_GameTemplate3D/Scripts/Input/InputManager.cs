using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InputManager : MonoBehaviour
{

    [Header("Input Type")]
    public INPUTTYPE inputType;  // Enum to select the input type
    public List<InputControl> keyBoardControls = new List<InputControl>(); // A list of keyboard control inputs
    public List<InputControl> joypadControls = new List<InputControl>();   // A list of joypad control inputs

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
    private float doubleTapTime;

    void Start()
    {
        // Automatically set input type based on platform (can be overridden by buttons)
#if UNITY_IOS || UNITY_ANDROID
			inputType = INPUTTYPE.TOUCHSCREEN;
#else
        inputType = INPUTTYPE.KEYBOARD;
#endif
    }

    public static void DirectionEvent(Vector2 dir, bool doubleTapActive)
    {
        if (onDirectionInputEvent != null) onDirectionInputEvent(dir, doubleTapActive);
    }

    void Update()
    {
        // Handle input based on the current input type
        switch (inputType)
        {
            case INPUTTYPE.KEYBOARD:
                KeyboardControls();
                break;
            case INPUTTYPE.JOYPAD:
                JoyPadControls();
                break;
            case INPUTTYPE.TOUCHSCREEN:
                // Touchscreen controls are handled in other scripts (UIJoystick and UIButton)
                break;
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

            // Convert keyboard direction keys to x, y values (every frame)
            if (Input.GetKey(inputControl.key))
            {
                if (inputControl.Action == "Left") x = -1f;
                else if (inputControl.Action == "Right") x = 1f;
                else if (inputControl.Action == "Up") y = 1;
                else if (inputControl.Action == "Down") y = -1;
            }

            // Defend key exception (checks the defend state every frame)
            if (inputControl.Action == "Defend") defendKeyDown = Input.GetKey(inputControl.key);
        }

        // Send a direction event if movement keys are pressed
        if (x != 0 || y != 0) DirectionEvent(new Vector2(x, y), doubleTapState);
    }

    void JoyPadControls()
    {
        if (onInputEvent == null) return;

        // On joypad button press
        foreach (InputControl inputControl in joypadControls)
        {
            if (Input.GetKeyDown(inputControl.key)) onInputEvent(inputControl.Action, BUTTONSTATE.PRESS);

            // Defend key exception (checks the defend state every frame)
            if (inputControl.Action == "Defend") defendKeyDown = Input.GetKey(inputControl.key);
        }

        // Get joypad direction axis
        float x = Input.GetAxis("Joypad Left-Right");
        float y = Input.GetAxis("Joypad Up-Down");

        // Send a direction event
        DirectionEvent(new Vector2(x, y).normalized, false);
    }

    // Returns true if a key double tap is detected
    bool DetectDoubleTap(string action)
    {
        bool doubleTapDetected = ((Time.time - lastInputTime < doubleTapSpeed) && (lastInputAction == action));
        lastInputAction = action;
        lastInputTime = Time.time;
        return doubleTapDetected;
    }

    // This function is called when a touch screen button is pressed
    public void OnTouchScreenInputEvent(string action, BUTTONSTATE buttonState)
    {
        onInputEvent(action, buttonState);

        // Defend exception
        if (action == "Defend") defendKeyDown = (buttonState == BUTTONSTATE.PRESS);
    }

    // This function is used for the touch screen thumbstick
    public void OnTouchScreenJoystickEvent(Vector2 joystickDir)
    {
        DirectionEvent(joystickDir.normalized, false);
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

        // Input type
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Input Type", EditorStyles.boldLabel);
        inputManager.inputType = (INPUTTYPE)EditorGUILayout.EnumPopup("Input Type:", inputManager.inputType);
        GUILayout.Space(15);

        // Keyboard controls
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

        // Joypad controls	
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

        // Touchscreen controls are managed by the UI scripts

        GUILayout.Space(15);

        if (inputManager.inputType == INPUTTYPE.KEYBOARD || inputManager.inputType == INPUTTYPE.JOYPAD)
        {
            GUILayout.BeginHorizontal();

            // Button: Add a new action 
            if (GUILayout.Button("Add Input Action", GUILayout.Width(130), GUILayout.Height(25)))
            {
                if (inputManager.inputType == INPUTTYPE.KEYBOARD) inputManager.keyBoardControls.Add(new InputControl());
                if (inputManager.inputType == INPUTTYPE.JOYPAD) inputManager.joypadControls.Add(new InputControl());
            }

            // Button: Delete last action 
            bool showDeleteButton = (inputManager.inputType == INPUTTYPE.KEYBOARD && inputManager.keyBoardControls.Count > 0) || (inputManager.inputType == INPUTTYPE.JOYPAD && inputManager.joypadControls.Count > 0);
            if (showDeleteButton && GUILayout.Button("Delete Input Action", GUILayout.Width(130), GUILayout.Height(25)))
            {
                if (inputManager.inputType == INPUTTYPE.KEYBOARD && inputManager.keyBoardControls.Count > 0) inputManager.keyBoardControls.RemoveAt(inputManager.keyBoardControls.Count - 1);
                if (inputManager.inputType == INPUTTYPE.JOYPAD && inputManager.joypadControls.Count > 0) inputManager.joypadControls.RemoveAt(inputManager.joypadControls.Count - 1);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(15);
        }

        // Double tap settings
        EditorGUILayout.LabelField("Double Tap Settings", EditorStyles.boldLabel);
        inputManager.doubleTapSpeed = EditorGUILayout.FloatField("Double Tap Speed:", inputManager.doubleTapSpeed);
        EditorUtility.SetDirty(inputManager);
    }
}
#endif
