using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyWaveSystem : MonoBehaviour
{
    public int MaxAttackers = 3; // The maximum number of enemies that can attack the player simultaneously 

    [Header("List of enemy Waves")]
    public EnemyWave[] EnemyWaves;
    public int currentWave;

    [Header("Slow Motion Settings")]
    public bool activateSlowMotionOnLastHit;
    public float effectDuration = 1.5f;

    [Header("Load level On Finish")]
    public bool loadNewLevel;
    public string levelName;

    private CameraFollow cameraFollow;

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

        // Dynamically find the CameraFollow component, accounting for network play where it might be a child of the player prefab
        StartCoroutine(FindAndAssignCameraFollow());

        UpdateAreaColliders();
        StartNewWave();
    }

    IEnumerator FindAndAssignCameraFollow()
    {
        // Continuously try to find CameraFollow until it is located
        while (cameraFollow == null)
        {
            cameraFollow = FindCameraFollow();
            if (cameraFollow != null)
            {
                Debug.Log("CameraFollow found and assigned.");
                yield break; // Exit when the camera is found
            }
            yield return new WaitForSeconds(0.5f); // Wait a little before trying again
        }
    }

    CameraFollow FindCameraFollow()
    {
        // This will find the CameraFollow component even if it's a child of another object (like the player prefab in network play)
        return GameObject.FindObjectOfType<CameraFollow>();
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
                    // Deactivate enemy
                    wave.EnemyList[i].SetActive(false);
                }
                else
                {
                    // Remove empty fields from the list
                    wave.EnemyList.RemoveAt(i);
                }
            }
            foreach (GameObject g in wave.EnemyList)
            {
                if (g != null) g.SetActive(false);
            }
        }
    }

    // Start a new enemy wave
    public void StartNewWave()
    {
        // Hide UI hand pointer
        HandPointer hp = GameObject.FindObjectOfType<HandPointer>();
        if (hp != null) hp.DeActivateHandPointer();

        // Activate enemies
        foreach (GameObject g in EnemyWaves[currentWave].EnemyList)
        {
            if (g != null) g.SetActive(true);
        }

        Invoke("SetEnemyTactics", 0.1f);
    }

    // Update Area Colliders
    void UpdateAreaColliders()
    {
        // Ensure CameraFollow is found before continuing
        if (cameraFollow == null)
        {
            Debug.LogError("CameraFollow component not found.");
            return;
        }

        // Switch current area collider to a trigger
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

        // Assign the collider to CameraFollow
        cameraFollow.CurrentAreaCollider = EnemyWaves[currentWave].AreaCollider;
    }

    // An enemy has been destroyed
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
                }
                else
                {
                    StartCoroutine(LevelComplete());
                }
            }
        }
    }

    // True if all the waves are completed
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

    // Update enemy tactics
    void SetEnemyTactics()
    {
        EnemyManager.SetEnemyTactics();
    }

    // Level complete
    IEnumerator LevelComplete()
    {
        // Activate slow motion effect
        if (activateSlowMotionOnLastHit)
        {
            CamSlowMotionDelay cmd = Camera.main.GetComponent<CamSlowMotionDelay>();
            if (cmd != null)
            {
                cmd.StartSlowMotionDelay(effectDuration);
                yield return new WaitForSeconds(effectDuration);
            }
        }

        // Timeout before continuing
        yield return new WaitForSeconds(1f);

        // Fade to black
        UIManager UI = GameObject.FindObjectOfType<UIManager>();
        if (UI != null)
        {
            UI.UI_fader.Fade(UIFader.FADE.FadeOut, 2f, 0);
            yield return new WaitForSeconds(2f);
        }

        // Disable players
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            Destroy(p);
        }

        // Go to the next level or show GAMEOVER screen
        if (loadNewLevel)
        {
            if (levelName != "")
                SceneManager.LoadScene(levelName);
        }
        else
        {
            // Show game over screen
            if (UI != null)
            {
                UI.DisableAllScreens();
                UI.ShowMenu("LevelComplete");
            }
        }
    }
}
