using UnityEngine;
using UnityEditor;
using System.IO;
using Default;
using Newtonsoft.Json;

[CustomEditor(typeof(TestingManager))]
public class TestingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TestingManager testingManager = (TestingManager)target;

        if (testingManager.useEvaluationFile)
        {
            string path = Path.Combine(Application.dataPath, "Evaluation/", testingManager.evaluationFileName);

            if (File.Exists(path))
            {
                string jsonContent = File.ReadAllText(path);
                EvolutionEvaluationData data = JsonConvert.DeserializeObject<EvolutionEvaluationData>(jsonContent);

                EditorGUILayout.Space(30f);
                EditorGUILayout.LabelField("Evaluations:");
                for (int i = 0; i < data.Evaluations.Count; i++)
                {
                    if (data.Evaluations[i].fittestNetwork != null)
                    {
                        string layerStructure = string.Join(", ", data.Evaluations[i].LayerStructure);
                        EditorGUILayout.LabelField($"Index: {i}, Layers: {layerStructure}, Fitness: {data.Evaluations[i].fittestNetwork.fitness}");
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Evaluation file not found.", MessageType.Warning);
            }
        }
    }
}
