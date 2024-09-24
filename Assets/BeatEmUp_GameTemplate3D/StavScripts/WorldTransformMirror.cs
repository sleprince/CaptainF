using UnityEngine;

public class MirrorWorldTransform : MonoBehaviour
{
    public Transform leftHand;  // Left hand transform
    public Transform rightHand; // Right hand transform
    public Transform body;      // Body center transform

    void Update()
    {
        if (leftHand != null && rightHand != null && body != null)
        {
            // Mirror position in world space
            rightHand.position = MirrorPosition(leftHand.position, body);

            // Mirror rotation in world space
            rightHand.rotation = MirrorRotation(leftHand.rotation);

            // Keep the scale the same (assuming uniform scale)
            rightHand.localScale = leftHand.localScale;
        }
    }

    Vector3 MirrorPosition(Vector3 leftHandPosition, Transform body)
    {
        // Calculate the offset of the left hand relative to the body's center
        Vector3 offset = leftHandPosition - body.position;

        // Mirror across the body's X-axis (assuming symmetry along the X-axis)
        Vector3 mirroredOffset = new Vector3(-offset.x, offset.y, offset.z);

        // Apply the mirrored offset to the body's position to get the right hand position
        return body.position + mirroredOffset;
    }

    Quaternion MirrorRotation(Quaternion leftHandRotation)
    {
        // In world space, mirror the rotation by flipping the X and Z components
        return new Quaternion(-leftHandRotation.x, leftHandRotation.y, -leftHandRotation.z, leftHandRotation.w);
    }
}
