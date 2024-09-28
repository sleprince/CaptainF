using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public UIFader UI_fader;
    public UI_Screen[] UIMenus;

    private bool isTitleScreenActive = true; // Track if TitleScreen is active
    private bool isTransitioning = false;    // Prevent double fades or transitions

    void Awake()
    {
        DisableAllScreens();
        DontDestroyOnLoad(gameObject);

        // Show TitleScreen without a fade if that's how it worked before
        ShowMenu("TitleScreen", false);
    }

    void Update()
    {
        // If we're still on TitleScreen and any key is pressed, initiate transition
        if (isTitleScreenActive && !isTransitioning && Input.anyKeyDown)
        {
            // Directly show the main menu without a coroutine
            ShowMenu("MainMenu");
            isTransitioning = true;  // Prevent further input during transition
            isTitleScreenActive = false;
        }
    }

    // Show a menu and handle fading (UI fader is managed inside this function)
    public void ShowMenu(string name, bool disableAllScreens = true)
    {
        if (disableAllScreens) DisableAllScreens();

        foreach (UI_Screen UI in UIMenus)
        {
            if (UI.UI_Name == name)
            {
                if (UI.UI_Gameobject != null)
                {
                    UI.UI_Gameobject.SetActive(true);
                }
                else
                {
                    Debug.Log("No menu found with name: " + name);
                }
            }
        }

        // Fade in the UI (if it's not the TitleScreen)
        if (UI_fader != null && name != "TitleScreen")
        {
            // Ensure the fader is active before fading in the new menu
            if (!UI_fader.gameObject.activeSelf)
            {
                UI_fader.gameObject.SetActive(true);
            }

            // Fade in the menu
            UI_fader.Fade(UIFader.FADE.FadeIn, .5f, .3f);
        }
    }

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
}
