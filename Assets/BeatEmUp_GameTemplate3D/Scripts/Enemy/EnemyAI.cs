using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : EnemyActions, IDamagable<DamageObject>
{
    [Space(10)]
    public bool enableAI;

    // A list of player prefabs to target, manually assigned via the inspector
    public GameObject[] playerPrefabs;

    private GameObject currentTarget; // The current player the enemy is targeting

    // A list of states where the AI is executed
    private List<UNITSTATE> ActiveAIStates = new List<UNITSTATE>
    {
        UNITSTATE.IDLE,
        UNITSTATE.WALK
    };

    void Start()
    {
        // Add this enemy to the enemy list
        EnemyManager.enemyList.Add(gameObject);

        // Set z spread (used to keep space between enemies)
        ZSpread = (EnemyManager.enemyList.Count - 1);
        Invoke("SetZSpread", .1f);

        // Randomize values to avoid synchronous movement
        if (randomizeValues) SetRandomValues();

        OnStart();

        // Ensure the player targets array is not empty
        if (playerPrefabs.Length == 0)
        {
            Debug.LogError("No player prefabs assigned in EnemyAI!");
        }
        else
        {
            // Set the default target as the first player (e.g., Player1)
            currentTarget = playerPrefabs[0];
        }
    }

    void FixedUpdate()
    {
        OnFixedUpdate();
    }

    void LateUpdate()
    {
        OnLateUpdate();
    }

    void Update()
    {
        // Do nothing when there are no targets or AI is disabled
        if (playerPrefabs.Length == 0 || !enableAI)
        {
            Ready();
            return;
        }

        // Update the target to the closest player dynamically
        UpdateClosestPlayerTarget();

        if (currentTarget == null)
        {
            Ready();
            return;
        }

        // Get range to target
        range = GetDistanceToTarget(currentTarget);

        if (!isDead && enableAI)
        {
            if (ActiveAIStates.Contains(enemyState) && targetSpotted)
            {
                // AI active 
                AI();
            }
            else
            {
                // Try to spot the player
                if (distanceToTarget.magnitude < sightDistance) targetSpotted = true;
            }
        }
    }

    void AI()
    {
        if (currentTarget == null)
        {
            Debug.LogError("AI method error: Current target is null.");
            return;
        }

        LookAtTarget(currentTarget.transform);
        if (range == RANGE.ATTACKRANGE)
        {
            // Attack the target
            if (!cliffSpotted)
            {
                if (Time.time - lastAttackTime > attackInterval)
                {
                    ATTACK();
                }
                else
                {
                    Ready();
                }
                return;
            }

            // Actions for ATTACKRANGE distance
            if (enemyTactic == ENEMYTACTIC.KEEPCLOSEDISTANCE) WalkTo(closeRangeDistance, 0f);
            if (enemyTactic == ENEMYTACTIC.KEEPMEDIUMDISTANCE) WalkTo(midRangeDistance, RangeMarging);
            if (enemyTactic == ENEMYTACTIC.KEEPFARDISTANCE) WalkTo(farRangeDistance, RangeMarging);
            if (enemyTactic == ENEMYTACTIC.STANDSTILL) Ready();
        }
        else
        {
            // Actions for CLOSERANGE, MIDRANGE & FARRANGE distances
            if (enemyTactic == ENEMYTACTIC.ENGAGE) WalkTo(attackRangeDistance, 0f);
            if (enemyTactic == ENEMYTACTIC.KEEPCLOSEDISTANCE) WalkTo(closeRangeDistance, RangeMarging);
            if (enemyTactic == ENEMYTACTIC.KEEPMEDIUMDISTANCE) WalkTo(midRangeDistance, RangeMarging);
            if (enemyTactic == ENEMYTACTIC.KEEPFARDISTANCE) WalkTo(farRangeDistance, RangeMarging);
            if (enemyTactic == ENEMYTACTIC.STANDSTILL) Ready();
        }
    }

    // Update the current range
    private RANGE GetDistanceToTarget(GameObject target)
    {
        if (target != null)
        {
            // Get distance from the target
            distanceToTarget = target.transform.position - transform.position;
            distance = Vector3.Distance(target.transform.position, transform.position);

            float distX = Mathf.Abs(distanceToTarget.x);
            float distZ = Mathf.Abs(distanceToTarget.z);

            // AttackRange
            if (distX <= attackRangeDistance)
            {
                if (distZ < (hitZRange / 2f))
                    return RANGE.ATTACKRANGE;
                else
                    return RANGE.CLOSERANGE;
            }

            // Close Range
            if (distX > attackRangeDistance && distX < midRangeDistance) return RANGE.CLOSERANGE;

            // Mid range
            if (distX > closeRangeDistance && distance < farRangeDistance) return RANGE.MIDRANGE;

            // Far range
            if (distX > farRangeDistance) return RANGE.FARRANGE;
        }
        return RANGE.FARRANGE;
    }

    // Find the closest player and update the current target
    private void UpdateClosestPlayerTarget()
    {
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in playerPrefabs)
        {
            if (player != null) // Check for null player references
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    closestPlayer = player;
                }
            }
        }

        // Update the target only if a closer player is found
        if (closestPlayer != null)
        {
            currentTarget = closestPlayer;
        }
    }

    // Set an enemy tactic
    public void SetEnemyTactic(ENEMYTACTIC tactic)
    {
        enemyTactic = tactic;
    }

    // Spread enemies out in z distance
    void SetZSpread()
    {
        ZSpread = (ZSpread - (float)(EnemyManager.enemyList.Count - 1) / 2f) * (capsule.radius * 2) * zSpreadMultiplier;
        if (ZSpread > attackRangeDistance) ZSpread = attackRangeDistance - 0.1f;
    }

    // Unit has died
    void Death()
    {
        StopAllCoroutines();
        CancelInvoke();

        enableAI = false;
        isDead = true;
        animator.SetAnimatorBool("isDead", true);
        Move(Vector3.zero, 0);
        EnemyManager.RemoveEnemyFromList(gameObject);
        gameObject.layer = LayerMask.NameToLayer("Default");

        // Ground death
        if (enemyState == UNITSTATE.KNOCKDOWNGROUNDED)
        {
            StartCoroutine(GroundHit());
        }
        else
        {
            // Normal death
            animator.SetAnimatorTrigger("Death");
        }

        GlobalAudioPlayer.PlaySFXAtPosition("EnemyDeath", transform.position);
        StartCoroutine(animator.FlickerCoroutine(2));
        enemyState = UNITSTATE.DEATH;
        DestroyUnit();
    }
}








public enum ENEMYTACTIC {
	ENGAGE = 0,
	KEEPCLOSEDISTANCE = 1,
	KEEPMEDIUMDISTANCE = 2,
	KEEPFARDISTANCE = 3,
	STANDSTILL = 4,
}

public enum RANGE {
	ATTACKRANGE,
	CLOSERANGE,
	MIDRANGE,
	FARRANGE,
}