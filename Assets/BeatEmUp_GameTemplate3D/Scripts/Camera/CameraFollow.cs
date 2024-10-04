using UnityEngine;
using Photon.Pun;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Vector3 MiddlePosition;

    [Header("Player Targets")]
    public GameObject[] targets;

    [Header("Follow Settings")]
    public float distanceToTarget = 10f; // The distance to the target
    public float heightOffset = -2f; // The height offset of the camera relative to its target
    public float viewAngle = -6f; // Downwards rotation
    public Vector3 AdditionalOffset; // Any additional offset
    public bool FollowZAxis; // Enable or disable the camera following the Z-axis

    [Header("Damp Settings")]
    public float DampX = 3f;
    public float DampY = 2f;
    public float DampZ = 3f;

    [Header("View Area")]
    public float MinLeft;
    public float MaxRight;

    [Header("Wave Area Collider")]
    public bool UseWaveAreaCollider;
    public BoxCollider CurrentAreaCollider; // This will be set from the EnemyWaveSystem
    public float AreaColliderViewOffset;
    private bool firstFrameActive;

    private PhotonView photonView; // For identifying the local player in a networked game

    void Start()
    {
        StartCoroutine(WaitForPlayersAndInitializeCamera());

        firstFrameActive = true;
        UpdatePlayerTargets();  // Initially assign targets
        InvokeRepeating("UpdatePlayerTargets", 0f, 0.5f); // Keep checking for players every 0.5 seconds

        // Check if this is a networked game
        if (PhotonNetwork.InRoom)
        {
            photonView = GetComponentInParent<PhotonView>();
        }
    }

    IEnumerator WaitForPlayersAndInitializeCamera()
    {
        // Wait until networked player objects are instantiated
        while (targets == null || targets.Length == 0)
        {
            UpdatePlayerTargets(); // Continuously check for players
            yield return new WaitForSeconds(0.1f); // Wait for a short time before checking again
        }

        firstFrameActive = true;
    }

    void Update()
    {
        if (targets.Length > 0)
        {
            MiddlePosition = Vector3.zero;

            if (targets.Length == 1)
            {
                // Follow a single target
                if (targets[0] != null) MiddlePosition = targets[0].transform.position;
            }
            else
            {
                // Find the center position between multiple targets
                int count = 0;
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i])
                    {
                        MiddlePosition += targets[i].transform.position;
                        count++;
                    }
                }
                MiddlePosition = MiddlePosition / count;
            }

            // Initial values
            float currentX = transform.position.x;
            float currentY = transform.position.y;
            float currentZ = transform.position.z;

            // Damp X
            currentX = Mathf.Lerp(currentX, MiddlePosition.x, DampX * Time.deltaTime);

            // Damp Y
            currentY = Mathf.Lerp(currentY, MiddlePosition.y - heightOffset, DampY * Time.deltaTime);

            // Damp Z
            if (FollowZAxis)
            {
                currentZ = Mathf.Lerp(currentZ, MiddlePosition.z + distanceToTarget, DampZ * Time.deltaTime);
            }
            else
            {
                currentZ = distanceToTarget;
            }

            // Set values for 1st frame (No damping)
            if (firstFrameActive)
            {
                firstFrameActive = false;
                currentX = MiddlePosition.x;
                currentY = MiddlePosition.y - heightOffset;
                currentZ = FollowZAxis ? (MiddlePosition.z + distanceToTarget) : distanceToTarget;
            }

            // Set camera position
            if (CurrentAreaCollider == null) UseWaveAreaCollider = false;
            if (!UseWaveAreaCollider)
            {
                transform.position = new Vector3(Mathf.Clamp(currentX, MaxRight, MinLeft), currentY, currentZ) + AdditionalOffset;
            }
            else
            {
                transform.position = new Vector3(Mathf.Clamp(currentX, CurrentAreaCollider.transform.position.x + AreaColliderViewOffset, MinLeft), currentY, currentZ) + AdditionalOffset;
            }

            // Set camera rotation
            transform.rotation = new Quaternion(0, 180f, viewAngle, 0);
        }
    }

    // Updates the targets to follow, differentiating local and network play
    public void UpdatePlayerTargets()
    {
        if (PhotonNetwork.InRoom)
        {
            GameObject localPlayer = FindLocalPlayer();  // Prioritize finding the local player
            if (localPlayer != null)
            {
                targets = new GameObject[] { localPlayer };  // Set the local player as the target
            }
            else
            {
                // Fallback to find players by tag, but this is less reliable in network mode
                targets = GameObject.FindGameObjectsWithTag("Player");
            }
        }
        else
        {
            // If not in network mode, just find players by tag
            targets = GameObject.FindGameObjectsWithTag("Player");
        }
    }

    private GameObject FindLocalPlayer()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView photonView = player.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                return player;
            }
        }
        return null;
    }
}
