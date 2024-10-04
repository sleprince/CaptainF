using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScrn : UISceneLoader
{
    public Text text;
    public Text subtext;
    public string TextRestart = "Press any key to restart";
    public string TextNextLevel = "Press any key to continue";
    public Gradient ColorTransition;
    public float speed = 3.5f;
    private bool restartInProgress = false;

    private InputManager inputManager; // Reference to InputManager

    private void OnEnable()
    {
        FindInputManager();  // Find the InputManager at the start

        // Subscribe to the static event using the class name instead of an instance
        InputManager.onInputEvent += OnInputEvent;

        if (subtext != null)
        {
            subtext.text = (GlobalGameSettings.LevelData.Count > 0 && !lastLevelReached()) ? TextNextLevel : TextRestart;
        }
        else
        {
            Debug.Log("No subtext assigned.");
        }

        restartInProgress = false;
    }

    private void OnDisable()
    {
        // Unsubscribe from the static event using the class name instead of an instance
        InputManager.onInputEvent -= OnInputEvent;
    }

    private void OnInputEvent(string action, BUTTONSTATE buttonState)
    {
        if (buttonState != BUTTONSTATE.PRESS) return;

        if (GlobalGameSettings.LevelData.Count == 0 || lastLevelReached())
        {
            LoadLevel(SceneManager.GetActiveScene().name, GlobalGameSettings.currentLevelId);
        }
        else
        {
            if (GlobalGameSettings.LevelData.Count > 0)
            {
                LoadLevel(GetNextSceneName(), GlobalGameSettings.currentLevelId + 1);
            }
        }
    }

    void Update()
    {
        if (text != null && text.gameObject.activeSelf)
        {
            float t = Mathf.PingPong(Time.time * speed, 1f);
            text.color = ColorTransition.Evaluate(t);
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
        {
            OnInputEvent("AnyKey", BUTTONSTATE.PRESS);
        }
    }

    void LoadLevel(string sceneName, int levelId)
    {
        if (!restartInProgress)
        {
            restartInProgress = true;

            if (inputManager != null)
            {
                inputManager.SetRetry(true);  // Use the found input manager
            }

            GlobalAudioPlayer.PlaySFX("ButtonStart");
            ButtonFlicker bf = GetComponentInChildren<ButtonFlicker>();
            if (bf != null) bf.StartButtonFlicker();

            GlobalGameSettings.currentLevelId = levelId;
            LoadScene(sceneName);
        }
    }

    string GetNextSceneName()
    {
        return GlobalGameSettings.LevelData[GlobalGameSettings.currentLevelId + 1].sceneToLoad;
    }

    bool lastLevelReached()
    {
        int totalNumberOfLevels = Mathf.Clamp(GlobalGameSettings.LevelData.Count - 1, 0, GlobalGameSettings.LevelData.Count);
        return GlobalGameSettings.currentLevelId == totalNumberOfLevels;
    }

    // Find the InputManager in the scene (networked or local)
    private void FindInputManager()
    {
        // Check if it's a networked game and find the correct InputManager (network mode)
        if (Photon.Pun.PhotonNetwork.InRoom)
        {
            foreach (var input in GameObject.FindObjectsOfType<InputManager>())
            {
                if (input.GetComponent<Photon.Pun.PhotonView>()?.IsMine == true)
                {
                    inputManager = input;
                    return;
                }
            }
        }
        else
        {
            // Local play - there should be only one InputManager in the scene
            inputManager = GameObject.FindObjectOfType<InputManager>();
        }
    }
}
