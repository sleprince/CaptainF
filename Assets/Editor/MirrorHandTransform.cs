using UnityEngine;
using UnityEditor;

public class MirrorHandTransformEditor : EditorWindow
{
    public Transform leftHand;
    public Transform rightHand;
    public Transform body;  // The center of the body, typically hips or pelvis

    [MenuItem("Tools/Mirror Hand Transform")]
    public static void ShowWindow()
    {
        GetWindow<MirrorHandTransformEditor>("Mirror Hand Transform");
    }

    void OnGUI()
    {
        GUILayout.Label("Mirror Hand Transform Tool", EditorStyles.boldLabel);

        leftHand = (Transform)EditorGUILayout.ObjectField("Left Hand Transform", leftHand, typeof(Transform), true);
        rightHand = (Transform)EditorGUILayout.ObjectField("Right Hand Transform", rightHand, typeof(Transform), true);
        body = (Transform)EditorGUILayout.ObjectField("Body (Hips/Pelvis)", body, typeof(Transform), true);

        if (GUILayout.Button("Mirror Left Hand to Right Hand"))
        {
            if (leftHand != null && rightHand != null && body != null)
            {
                MirrorHandPositionAndRotation();
                Debug.Log("Hand transform mirrored successfully.");
            }
            else
            {
                Debug.LogError("Please assign all required transforms.");
            }
        }
    }

    void MirrorHandPositionAndRotation()
    {
        // Mirror the position
        Vector3 leftHandPosition = leftHand.position;
        Vector3 offset = leftHandPosition - body.position;
        Vector3 mirroredOffset = new Vector3(-offset.x, offset.y, offset.z);
        rightHand.position = body.position + mirroredOffset;

        // Mirror the rotation
        Quaternion leftHandRotation = leftHand.rotation;
        rightHand.rotation = new Quaternion(-leftHandRotation.x, leftHandRotation.y, -leftHandRotation.z, leftHandRotation.w);

        // Optionally mirror the scale if needed
        rightHand.localScale = leftHand.localScale;
    }
}
