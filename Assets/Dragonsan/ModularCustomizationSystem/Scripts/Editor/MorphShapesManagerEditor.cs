using System.IO;
using UnityEngine;
using UnityEditor;
using HNGamers;

[CustomEditor(typeof(MorphShapesManager))]
public class MorphShapesManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draw the default inspector

        MorphShapesManager manager = (MorphShapesManager)target;

        // Button to initialize morph shapes
        if (GUILayout.Button("Initialize Morph Shapes"))
        {
            manager.InitializeMorphShapes();
            EditorUtility.SetDirty(manager); // Mark the manager as dirty to trigger a save
        }
        // Button to save morph shapes data to the Resources folder
        if (GUILayout.Button("Save Morph Shapes Data"))
        {
            SaveMorphShapesData(manager);
        }
        // Additional custom UI elements can be added here
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
        manager.limitRespect = EditorGUILayout.Toggle("Limit Respect", manager.limitRespect);
        manager.globalMultiplier = EditorGUILayout.Slider("Global Multiplier", manager.globalMultiplier, 0.0f, 1f);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Naming Conventions", EditorStyles.boldLabel);
        manager.blendShapePrimaryPrefix = EditorGUILayout.TextField("Primary Prefix", manager.blendShapePrimaryPrefix);
        manager.blendShapeMatchPrefix = EditorGUILayout.TextField("Match Prefix", manager.blendShapeMatchPrefix);
        manager.plusMinus = (EditorGUILayout.TextField("Plus Suffix", manager.plusMinus.Item1), EditorGUILayout.TextField("Minus Suffix", manager.plusMinus.Item2));
    }


    private void SaveMorphShapesData(MorphShapesManager manager)
    {
        // Generate the file name based on the GameObject's name
        string fileName = manager.gameObject.name + "_blendshapes.txt";
        string resourcesPath = "Assets/Resources";
        string fullPath = Path.Combine(resourcesPath, fileName);

        // Ensure the Resources directory exists
        if (!Directory.Exists(resourcesPath))
        {
            Directory.CreateDirectory(resourcesPath);
        }

        // Get the morph shapes data string
        string dataToSave = manager.ReturnMorphShapeDataString();

        // Write the data to a file in the Resources folder
        File.WriteAllText(fullPath, dataToSave);

        // Refresh the AssetDatabase to show the new file in the Unity Editor
        AssetDatabase.Refresh();

        Debug.Log("Morph shapes data saved to: " + fullPath);
    }
}