using UnityEngine;

public class AreaColliderTrigger : MonoBehaviour
{
    public EnemyWaveSystem EnemyWaveSystem;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            if (EnemyWaveSystem != null)
                EnemyWaveSystem.StartNewWave();

            // Destroy the collider so the player can move to the next area
            Destroy(gameObject);
        }
    }
}
