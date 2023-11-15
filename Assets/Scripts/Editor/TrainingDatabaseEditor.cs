using UnityEngine;
using UnityEditor;
using Default;

[CustomEditor(typeof(TrainingDatabase))]
public class TrainingDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        base.OnInspectorGUI();

        // Cast the target to TrainingDatabase
        TrainingDatabase database = (TrainingDatabase)target;

        if (database.Maps == null) return;

        for (int i = 0; i < database.Maps.Count; i++)
        {
            var mapData = database.Maps[i];

            if (mapData.mapTexture != null)
            {
                GUILayout.Label($"Preview for map {i}:");
                Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(50));
                EditorGUI.DrawPreviewTexture(rect, mapData.mapTexture, null, ScaleMode.ScaleToFit);
            }
        }
    }
}
