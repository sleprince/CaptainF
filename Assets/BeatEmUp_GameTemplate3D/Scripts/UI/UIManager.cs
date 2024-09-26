using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public UIFader UI_fader;
    public UI_Screen[] UIMenus;

    private InputManager inputManager;

    void Awake()
    {
        DisableAllScreens();

        // Don't destroy this object on load
        DontDestroyOnLoad(gameObject);

        // Find InputManager
        inputManager = FindObjectOfType<InputManager>();
    }

    // Shows a menu by name
    public void ShowMenu(string name, bool disableAllScreens)
    {
        if (disableAllScreens) DisableAllScreens();

        foreach (UI_Screen UI in UIMenus)
        {
            if (UI.UI_Name == name)
            {
                if (UI.UI_Gameobject != null)
                {
                    UI.UI_Gameobject.SetActive(true);

                    // Only show touch screen controls if inputType is TOUCHSCREEN and it's not the main menu
                    if (IsGameplayScene() && inputManager != null && inputManager.inputType == INPUTTYPE.TOUCHSCREEN)
                    {
                        Debug.Log("Showing TouchScreenControls");
                        ShowTouchScreenControls();
                    }
                    else
                    {
                        Debug.Log("Hiding TouchScreenControls");
                        CloseMenu("TouchScreenControls");
                    }
                }
                else
                {
                    Debug.Log("No menu found with name: " + name);
                }
            }
        }

        // Fade in the UI
        if (UI_fader != null) UI_fader.gameObject.SetActive(true);
        UI_fader.Fade(UIFader.FADE.FadeIn, .5f, .3f);
    }

    public void ShowMenu(string name)
    {
        ShowMenu(name, true);
    }

    // Closes a menu by name
    public void CloseMenu(string name)
    {
        foreach (UI_Screen UI in UIMenus)
        {
            if (UI.UI_Name == name)
            {
                UI.UI_Gameobject.SetActive(false);
            }
        }
    }

    // Disables all screens
    public void DisableAllScreens()
    {
        foreach (UI_Screen UI in UIMenus)
        {
            if (UI.UI_Gameobject != null)
            {
                UI.UI_Gameobject.SetActive(false);
            }
            else
            {
                Debug.Log("Null reference found in UI with name: " + UI.UI_Name);
            }
        }
    }

    // Show touch screen controls if inputType is TOUCHSCREEN
    void ShowTouchScreenControls()
    {
        foreach (UI_Screen UI in UIMenus)
        {
            if (UI.UI_Name == "TouchScreenControls")
            {
                if (inputManager != null && inputManager.inputType == INPUTTYPE.TOUCHSCREEN)
                {
                    Debug.Log("Activating TouchScreenControls");
                    UI.UI_Gameobject.SetActive(true);
                }
                else
                {
                    Debug.Log("Deactivating TouchScreenControls");
                    UI.UI_Gameobject.SetActive(false);
                }
                return;
            }
        }
        Debug.LogError("TouchScreenControls not found in UIMenus.");
    }

    // Check if the current scene is gameplay or menu
    bool IsGameplayScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName != "00_MainMenu"; // Replace with your actual main menu scene name
    }
}

[System.Serializable]
public class UI_Screen
{
    public string UI_Name;
    public GameObject UI_Gameobject;
    public bool showTouchControls;
}
