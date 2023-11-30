using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Default;


[CustomEditor(typeof(NeuralNetworkData))]
public class NeuralNetworkDataEditor : Editor
{
    NeuralNetworkData neuralNetworkData;
    List<int> layerSizes = new() { 14, 12, 8, 5 }; // Example default values


    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        neuralNetworkData = (NeuralNetworkData)target;

        // Layer sizes input
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Layer Sizes:", GUILayout.MaxWidth(80));
        string layersInput = EditorGUILayout.TextField(string.Join(", ", layerSizes.ToArray()));
        EditorGUILayout.EndHorizontal();

        // Update layerSizes if input changes
        if (GUILayout.Button("Update Layers"))
        {
            layerSizes.Clear();
            foreach (var size in layersInput.Split(','))
            {
                if (int.TryParse(size.Trim(), out int layerSize))
                {
                    layerSizes.Add(layerSize);
                }
            }
        }

        // Create Neural Network button
        if (GUILayout.Button("Create Neural Network"))
        {
            neuralNetworkData.CreateNeuralNetwork(layerSizes.ToArray());
        }

        // Clear data button
        if (GUILayout.Button("Clear Training Data"))
        {
            neuralNetworkData.ClearTrainingData();
        }
    }
}
