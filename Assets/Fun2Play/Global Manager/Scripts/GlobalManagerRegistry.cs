using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GlobalManagerRegistry : MonoBehaviour
{
    // Public access allows direct interaction from other classes
    public static Dictionary<string, GameObject> managerDictionary = new Dictionary<string, GameObject>();

    [System.Serializable]
    private struct ManagerEntry
    {
        public string key;
        public GameObject manager;
    }

    [SerializeField]
    private List<ManagerEntry> managerEntries = new List<ManagerEntry>();

    private static object dictionaryLock = new object();

    private void Awake()
    {
        RegisterRegistry(this, gameObject);
    }

    private void OnDestroy()
    {
        UnregisterRegistry(this);
    }

    private static void RegisterRegistry(GlobalManagerRegistry registry, GameObject gameObject)
    {
        lock (dictionaryLock)
        {
            foreach (var entry in registry.managerEntries)
            {
                if (!managerDictionary.ContainsKey(entry.key))
                {
                    managerDictionary.Add(entry.key, entry.manager);
                }
                else
                {
                    Debug.LogError($"The name '{entry.key}' is already registered as a GlobalManager Key in the dictionary and you can't use it a second time.\n" +
                        $"Make sure all registries have unique keys. To fix the duplicated key, change its name in the GlobalManagerRegistry component attached to the GameObject '{gameObject.name}'.", gameObject);
                }
            }
        }
    }

    private static void UnregisterRegistry(GlobalManagerRegistry registry)
    {
        lock (dictionaryLock)
        {
            foreach (var entry in registry.managerEntries)
            {
                managerDictionary.Remove(entry.key);
            }
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(GlobalManagerRegistry))]
public class GlobalManagerRegistryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Fun2PlayLogoDrawer.DrawLogo();

        // Draw the default inspector GUI
        DrawDefaultInspector();
    }
}
#endif