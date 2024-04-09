using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

public class FindGOsWithMissingMonoScript : EditorWindow
{
    [MenuItem("Fun2Play/Tools/Find Missing Mono Scripts")]
    public static void FindMissingMonoScripts()
    {
        List<GameObject> objectsWithMissingScripts = new List<GameObject>();

        // Find all GameObjects in the scene
        GameObject[] allGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        // Check all GameObjects and their children for missing scripts
        foreach (GameObject obj in allGameObjects)
        {
            CheckForMissingScriptsRecursive(obj, objectsWithMissingScripts);
        }

        // Display the results
        DisplayResults(objectsWithMissingScripts);
    }

    private static void CheckForMissingScriptsRecursive(GameObject gameObject, List<GameObject> objectsWithMissingScripts)
    {
        // Check if the GameObject itself has any missing MonoBehaviours
        Component[] components = gameObject.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component == null)
            {
                objectsWithMissingScripts.Add(gameObject);
                break;
            }
        }

        // Recursively check children
        foreach (Transform child in gameObject.transform)
        {
            CheckForMissingScriptsRecursive(child.gameObject, objectsWithMissingScripts);
        }
    }

    private static void DisplayResults(List<GameObject> objectsWithMissingScripts)
    {
        // Display the results in the console
        if (objectsWithMissingScripts.Count > 0)
        {
            Debug.Log("There is a total of " + objectsWithMissingScripts.Count + " missing Mono Scripts in your active Scene.");

            for (int i = 0; i < objectsWithMissingScripts.Count; i++)
            {
                GameObject obj = objectsWithMissingScripts[i];
                Debug.Log("Script " + (i + 1) + ": missing on GameObject '" + obj.name + "'!", obj);
            }
        }
        else
        {
            Debug.Log("No GameObjects with missing Mono Scripts found in the current scene.");
        }
    }
}
