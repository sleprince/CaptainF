﻿using UnityEngine;
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

    private void OnEnable()
    {
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

            if (InputManager.instance != null)
            {
                InputManager.instance.SetRetry(true);
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
}
