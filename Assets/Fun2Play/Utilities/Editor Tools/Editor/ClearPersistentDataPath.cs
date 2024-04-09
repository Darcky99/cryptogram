using UnityEditor;
using UnityEngine;

public class PersistentDataPathClearer
{
    [MenuItem("Fun2Play/Clear Persistent Data Path", priority = 0)]
    private static void ClearDataPath()
    {
        bool userConfirmed = EditorUtility.DisplayDialog("Clear Persistent Data Path", "Are you sure you want to clear the persistent data path? This action cannot be undone.", "Yes", "No");

        if (userConfirmed)
        {
            ClearPersistentDataPath();
        }
    }

    private static void ClearPersistentDataPath()
    {
        try
        {
            string path = Application.persistentDataPath;

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);

            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (var dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            Debug.Log("Persistent Data Path cleared successfully.");
        }
        catch (System.Exception e)
        {
            Debug.Log("Error clearing Persistent Data Path: " + e.Message);
        }
    }
}
