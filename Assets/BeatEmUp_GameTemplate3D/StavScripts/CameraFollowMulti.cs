using UnityEngine;

public class CameraFollowMulti : MonoBehaviour
{

    public Vector3 MiddlePosition;

    [Header("Player Targets")]
    public GameObject[] targets;

    [Header("Follow Settings")]
    public float distanceToTarget = 10;
    public float heightOffset = -2;
    public float viewAngle = -6;
    public Vector3 AdditionalOffset;
    public bool FollowZAxis;

    [Header("Damp Settings")]
    public float DampX = 3f;
    public float DampY = 2f;
    public float DampZ = 3f;

    [Header("View Area")]
    public float MinLeft;
    public float MaxRight;

    [Header("Wave Area collider")]
    public bool UseWaveAreaCollider;
    public BoxCollider CurrentAreaCollider;
    public float AreaColliderViewOffset;
    private bool firstFrameActive;

    void Start()
    {
        firstFrameActive = true;
    }

    void Update()
    {
        UpdatePlayerTargets(); // Now this is in Update for dynamic target assignment

        if (targets.Length > 0)
        {
            MiddlePosition = Vector3.zero;

            if (targets.Length == 1)
            {
                // Follow a single target
                if (targets[0] != null)
                    MiddlePosition = targets[0].transform.position;
            }
            else
            {
                // Find center position between multiple targets
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
            if (CurrentAreaCollider == null) UseWaveAreaCollider = true;
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

    // Updates the targets to follow
    public void UpdatePlayerTargets()
    {
        targets = GameObject.FindGameObjectsWithTag("Player");
    }
}
