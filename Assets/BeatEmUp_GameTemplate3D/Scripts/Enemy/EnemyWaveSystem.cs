using UnityEngine;
using Photon.Pun;
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
                    wave.EnemyList[i].SetActive(false); // Deactivate enemy
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
        // Hide UI hand pointer
        HandPointer hp = GameObject.FindObjectOfType<HandPointer>();
        if (hp != null) hp.DeActivateHandPointer();

        // Activate enemies in the current wave after passing through the collider
        foreach (GameObject g in EnemyWaves[currentWave].EnemyList)
        {
            if (g != null) g.SetActive(true);
        }
        Invoke("SetEnemyTactics", .1f);
    }

    // Update Area Colliders
    void UpdateAreaColliders()
    {
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

        if (EnemyWaves[currentWave].AreaCollider != null)
        {
            EnemyWaves[currentWave].AreaCollider.gameObject.SetActive(true);
        }

        CameraFollow cf = GameObject.FindObjectOfType<CameraFollow>();
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

                // Only show hand pointer if there are more waves
                if (!allWavesCompleted())
                {
                    HandPointer hp = GameObject.FindObjectOfType<HandPointer>();
                    if (hp != null) hp.ActivateHandPointer();

                    UpdateAreaColliders();
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

        // Go to next level or show GAMEOVER screen
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
