using UnityEngine;
using UnityEditor;

public class RenameSubObjectsWindow : EditorWindow
{
    public Transform sourceRoot; // Assign the source root GameObject in the Inspector
    public Transform destinationRoot; // Assign the destination root GameObject in the Inspector

    [MenuItem("Tools/Rename Sub-Objects")]
    public static void ShowWindow()
    {
        GetWindow<RenameSubObjectsWindow>("Rename Sub-Objects");
    }

    void OnGUI()
    {
        GUILayout.Label("Prefab Assignments", EditorStyles.boldLabel);

        sourceRoot = (Transform)EditorGUILayout.ObjectField("Source Root", sourceRoot, typeof(Transform), true);
        destinationRoot = (Transform)EditorGUILayout.ObjectField("Destination Root", destinationRoot, typeof(Transform), true);

        if (GUILayout.Button("Rename Sub-Objects"))
        {
            RenameChildrenBasedOnStructure(sourceRoot, destinationRoot);
        }
    }

    void RenameChildrenBasedOnStructure(Transform source, Transform target)
    {
        // Check if the target has the same number of children as the source
        if (source.childCount != target.childCount)
        {
            Debug.LogWarning("The source and destination do not have the same number of children.");
            return;
        }

        // Iterate through all children of the source
        for (int i = 0; i < source.childCount; i++)
        {
            Transform sourceChild = source.GetChild(i);
            Transform targetChild = target.GetChild(i);

            // Rename the target child to match the source child
            targetChild.name = sourceChild.name;

            // Recursively rename children of both the source and target
            RenameChildrenBasedOnStructure(sourceChild, targetChild);
        }
    }
}
