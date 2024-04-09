using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class FindLayersInUse : EditorWindow
{
    [MenuItem("Fun2Play/Tools/Find Layers In Use")]
    public static void CheckUsedLayers()
    {
        Debug.Log("Searching for used layers...");
        Dictionary<string, int> layerCount = new Dictionary<string, int>();
        List<GameObject> gameObjects = FindObjectsOfType<GameObject>().ToList();

        // iterate objects and save to dictionary
        foreach (var go in gameObjects)
        {
            string layerName = LayerMask.LayerToName(go.layer);
            if (layerCount.ContainsKey(layerName))
            {
                layerCount[layerName]++;
            }
            else
            {
                layerCount.Add(layerName, 1);
            }
        }

        // log to console
        foreach (KeyValuePair<string, int> entry in layerCount)
        {
            Debug.Log(entry.Key + ": " + entry.Value);
        }

        // unused layers
        List<string> layerNames = new List<string>();
        for (int i = 8; i <= 31; i++) // user defined layers start with layer 8 and unity supports 31 layers
        {
            var layerN = LayerMask.LayerToName(i); // get the name of the layer
            if (!string.IsNullOrEmpty(layerN)) // only add the layer if it has been named
                layerNames.Add(layerN);
        }

        List<string> listOfKeys = layerCount.Keys.ToList();
        List<string> unusedLayers = layerNames.Except(listOfKeys).ToList();
        string joined = string.Join(", ", unusedLayers);
        Scene scene = SceneManager.GetActiveScene();
        Debug.Log("Unused layers in " + scene.name + ": " + joined);

        Debug.Log("Find Used Layers finished to search the entire scene.");
    }
}
