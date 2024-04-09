using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class CSVToJsonConverterWindow : EditorWindow
{
    private string csvFilePath = "Assets/YourData.csv";
    private string jsonFilePath = "Assets/Output.json";

    [MenuItem("Fun2Play/Tools/CSV to JSON Converter")]
    public static void ShowWindow()
    {
        CSVToJsonConverterWindow window = GetWindow<CSVToJsonConverterWindow>("CSV to JSON Converter", true);
        window.minSize = new Vector2(400, 200);
    }

    private void OnGUI()
    {
        // Display Fun2Play logo
        Fun2PlayLogoDrawer.DrawLogo();

        // Input CSV file path
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("CSV File Path:", GUILayout.Width(100));
        csvFilePath = EditorGUILayout.TextField(csvFilePath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            csvFilePath = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(2);

        // Output JSON file path
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("JSON File Path:", GUILayout.Width(100));
        jsonFilePath = EditorGUILayout.TextField(jsonFilePath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            jsonFilePath = EditorUtility.SaveFilePanel("Save JSON File", "", "Output", "json");
        }
        EditorGUILayout.EndHorizontal();

        // Add a button to open FindScriptableObjectsInUse window
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedHeight = 50f;

        EditorGUILayout.Space(25);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        // Convert button
        if (GUILayout.Button("Convert", buttonStyle, GUILayout.Width(200)))
        {
            ConvertCSVToJson();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void ConvertCSVToJson()
    {
        if (string.IsNullOrEmpty(csvFilePath) || string.IsNullOrEmpty(jsonFilePath))
        {
            EditorUtility.DisplayDialog("Error", "CSV file path or JSON output path is empty!", "OK");
            return;
        }

        // Read CSV file
        string[] csvLines = File.ReadAllLines(csvFilePath);

        // Parse CSV data into JSON format
        List<Dictionary<string, string>> jsonDataList = new List<Dictionary<string, string>>();
        string[] headers = csvLines[0].Split(',');
        foreach (string line in csvLines.Skip(1))
        {
            string[] values = line.Split(',');
            Dictionary<string, string> jsonData = new Dictionary<string, string>();
            for (int i = 0; i < headers.Length; i++)
            {
                jsonData[headers[i]] = values[i];
            }
            jsonDataList.Add(jsonData);
        }

        // Convert JSON data to string
        string jsonDataString = JsonUtility.ToJson(jsonDataList, true);

        // Write JSON data to file
        File.WriteAllText(jsonFilePath, jsonDataString);

        Debug.Log("CSV to JSON conversion complete.");
        EditorUtility.DisplayDialog("Conversion Complete", "CSV to JSON conversion complete.", "OK");
    }
}
