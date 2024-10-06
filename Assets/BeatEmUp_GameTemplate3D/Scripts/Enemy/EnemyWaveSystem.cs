using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyWaveSystem : MonoBehaviour
{
    public int MaxAttackers = 3; // The maximum number of enemies that can attack the player simultaneously 

    [Header("List of Enemy Waves")]
    public EnemyWave[] EnemyWaves;
    public int currentWave;

    [Header("Slow Motion Settings")]
    public bool activateSlowMotionOnLastHit;
    public float effectDuration = 1.5f;

    [Header("Load level On Finish")]
    public bool loadNewLevel;
    public string levelName;

    private GameObject[] playerPrefabs; // Store the player prefabs as targets

    void OnEnable()
    {
        EnemyActions.OnUnitDestroy += onUnitDestroy;
    }

    void OnDisable()
    {
        EnemyActions.OnUnitDestroy -= onUnitDestroy;
    }

    void Awake()
    {
        if (enabled) DisableAllEnemies();
    }

    void Start()
    {
        currentWave = 0;
        StartCoroutine(UpdatePlayerTargetsRepeatedly());
        UpdateAreaColliders();
        StartNewWave();
    }

    // Disable all the enemies
    void DisableAllEnemies()
    {
        foreach (EnemyWave wave in EnemyWaves)
        {
            for (int i = 0; i < wave.EnemyList.Count; i++)
            {
                if (wave.EnemyList[i] != null)
                {
                    wave.EnemyList[i].SetActive(false);
                }
                else
                {
                    wave.EnemyList.RemoveAt(i); // Remove empty fields from the list
                }
            }
        }
    }

    // Start a new enemy wave
    public void StartNewWave()
    {
        // Activate enemies in the current wave
        foreach (GameObject enemy in EnemyWaves[currentWave].EnemyList)
        {
            if (enemy != null) enemy.SetActive(true);
        }
        Invoke("SetEnemyTactics", .1f);
    }

    // Update the player targets for the enemies
    IEnumerator UpdatePlayerTargetsRepeatedly()
    {
        // Keep searching for player prefabs for the first few seconds of the game, ensuring that players are properly instantiated.
        for (int i = 0; i < 5; i++) // Retry for 5 frames
        {
            UpdatePlayerTargets();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void UpdatePlayerTargets()
    {
        // Continuously search for player prefabs (Photon Player prefabs)
        playerPrefabs = GameObject.FindGameObjectsWithTag("Player");

        // Ensure that all enemies in the current wave receive the player targets
        foreach (var wave in EnemyWaves)
        {
            foreach (var enemy in wave.EnemyList)
            {
                if (enemy != null)
                {
                    EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        enemyAI.playerPrefabs = playerPrefabs; // Assign all player prefabs to the enemies
                    }
                }
            }
        }
    }

    // Update the area colliders for wave restrictions
    void UpdateAreaColliders()
    {
        // Switch the current area collider to a trigger
        if (currentWave > 0)
        {
            BoxCollider areaCollider = EnemyWaves[currentWave - 1].AreaCollider;
            if (areaCollider != null)
            {
                areaCollider.enabled = true;
                areaCollider.isTrigger = true;
                AreaColliderTrigger act = areaCollider.gameObject.AddComponent<AreaColliderTrigger>();
                act.EnemyWaveSystem = this;
            }
        }

        // Set next collider as camera area restrictor
        if (EnemyWaves[currentWave].AreaCollider != null)
        {
            EnemyWaves[currentWave].AreaCollider.gameObject.SetActive(true);
        }

        CameraFollow cf = FindObjectOfType<CameraFollow>();
        if (cf != null) cf.CurrentAreaCollider = EnemyWaves[currentWave].AreaCollider;
    }

    // When an enemy is destroyed
    void onUnitDestroy(GameObject g)
    {
        if (EnemyWaves.Length > currentWave)
        {
            EnemyWaves[currentWave].RemoveEnemyFromWave(g);
            if (EnemyWaves[currentWave].waveComplete())
            {
                currentWave += 1;
                if (!allWavesCompleted())
                {
                    UpdateAreaColliders();
                    StartNewWave(); // Start the next wave if available
                }
                else
                {
                    StartCoroutine(LevelComplete());
                }
            }
        }
    }

    // True if all waves are completed
    bool allWavesCompleted()
    {
        int waveCount = EnemyWaves.Length;
        int waveFinished = 0;

        for (int i = 0; i < waveCount; i++)
        {
            if (EnemyWaves[i].waveComplete()) waveFinished += 1;
        }

        return waveCount == waveFinished;
    }

    // Add this method to EnemyWaveSystem
    public BoxCollider GetCurrentWaveCollider()
    {
        if (currentWave < EnemyWaves.Length && EnemyWaves[currentWave].AreaCollider != null)
        {
            return EnemyWaves[currentWave].AreaCollider;
        }
        return null; // Return null if no current collider is found
    }

    // Update enemy tactics
    void SetEnemyTactics()
    {
        EnemyManager.SetEnemyTactics();
    }

    // Level complete
    IEnumerator LevelComplete()
    {
        if (activateSlowMotionOnLastHit)
        {
            CamSlowMotionDelay cmd = Camera.main.GetComponent<CamSlowMotionDelay>();
            if (cmd != null)
            {
                cmd.StartSlowMotionDelay(effectDuration);
                yield return new WaitForSeconds(effectDuration);
            }
        }

        yield return new WaitForSeconds(1f);

        UIManager UI = FindObjectOfType<UIManager>();
        if (UI != null)
        {
            UI.UI_fader.Fade(UIFader.FADE.FadeOut, 2f, 0);
            yield return new WaitForSeconds(2f);
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            Destroy(p);
        }

        if (loadNewLevel)
        {
            if (levelName != "")
                SceneManager.LoadScene(levelName);
        }
        else
        {
            if (UI != null)
            {
                UI.DisableAllScreens();
                UI.ShowMenu("LevelComplete");
            }
        }
    }
}
