namespace MobX.Utilities.Editor.Menu
{
    public static class Shortcuts
    {
        [UnityEditor.MenuItem("Edit/Clear All EditorPrefs", priority = 280)]
        private static void ClearEditorPrefs()
        {
            UnityEditor.EditorPrefs.DeleteAll();
        }
    }
}