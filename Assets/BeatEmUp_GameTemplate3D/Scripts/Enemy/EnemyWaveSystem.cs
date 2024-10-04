using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class EnemyWaveSystem : MonoBehaviour
{
    public int MaxAttackers = 3; // Maximum number of enemies that can attack the player simultaneously 

    [Header("List of enemy Waves")]
    public EnemyWave[] EnemyWaves;
    public int currentWave;

    [Header("Slow Motion Settings")]
    public bool activateSlowMotionOnLastHit;
    public float effectDuration = 1.5f;

    [Header("Load level On Finish")]
    public bool loadNewLevel;
    public string levelName;

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
                    wave.EnemyList[i].SetActive(false);
                }
                else
                {
                    wave.EnemyList.RemoveAt(i);
                }
            }
        }
    }

    // Start a new enemy wave
    public void StartNewWave()
    {
        HandPointer hp = GameObject.FindObjectOfType<HandPointer>();
        if (hp != null) hp.DeActivateHandPointer();

        foreach (GameObject g in EnemyWaves[currentWave].EnemyList)
        {
            if (g != null) g.SetActive(true);
        }
        Invoke("SetEnemyTactics", .1f);
    }

    // Update Area Colliders
    void UpdateAreaColliders()
    {
        // Ensure wave area collider is handled correctly across network
        CameraFollow cf = GameObject.FindObjectOfType<CameraFollow>();
        if (PhotonNetwork.InRoom)
        {
            // Network mode - only update local player's camera
            if (cf != null && cf.GetComponent<PhotonView>().IsMine)
            {
                SetCurrentWaveCollider(cf);
            }
        }
        else
        {
            // Local play
            SetCurrentWaveCollider(cf);
        }
    }

    // Helper function to set current wave collider
    void SetCurrentWaveCollider(CameraFollow cf)
    {
        if (cf != null)
        {
            // Switch previous area collider to trigger
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

            // Update the camera's area collider for restriction
            cf.CurrentAreaCollider = EnemyWaves[currentWave].AreaCollider;
            cf.UseWaveAreaCollider = true;
            cf.AreaColliderViewOffset = 4.5f; // Ensure the proper offset is set
        }

        // Activate UI hand pointer if needed
        HandPointer hp = GameObject.FindObjectOfType<HandPointer>();
        if (hp != null) hp.ActivateHandPointer();
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

    public BoxCollider GetCurrentWaveCollider()
    {
        if (EnemyWaves.Length > 0 && EnemyWaves[currentWave].AreaCollider != null)
        {
            return EnemyWaves[currentWave].AreaCollider;
        }
        return null;
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

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            Destroy(p);
        }

        if (loadNewLevel && levelName != "")
        {
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
