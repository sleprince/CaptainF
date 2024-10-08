using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun; // Required for Photon networking
using System.Collections;

public class UIManagerNetwork : MonoBehaviourPun
{
    public UIFader UI_fader;
    public UI_Screen[] UIMenus;

    void Awake()
    {
        DisableAllScreens();
        DontDestroyOnLoad(gameObject);

        // Show the Main Menu as we assume it's already past the title screen in multiplayer mode
        ShowMenu("HUD", false);
    }

    void Update()
    {

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

        // Fade in the UI
        if (UI_fader != null)
        {
            if (!UI_fader.gameObject.activeSelf)
            {
                UI_fader.gameObject.SetActive(true);
            }

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


    [System.Serializable]
    public class UI_Screen
    {
        public string UI_Name;
        public GameObject UI_Gameobject;
    }

}
