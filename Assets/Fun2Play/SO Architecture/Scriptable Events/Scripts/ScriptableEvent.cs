#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "New Scriptable Event", menuName = "Fun2Play/SO Architecture/Scriptable Event")]
public class ScriptableEvent : ScriptableObject
{
    [Serializable]
    public class UnityEventWithParams : UnityEvent<object, object, object> { }

    [Tooltip("Unity event to invoke when the Scriptable Event is raised")]
    public UnityEventWithParams unityEvent;

    public void Raise(object parameter1, object parameter2, object parameter3)
    {
        unityEvent.Invoke(parameter1, parameter2, parameter3);
    }

}

#region Unity Editor

#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableEvent))]
public class ScriptableEventEditor : Editor
{
    private string eventInfo;
    private bool instructionsDismissed;
    private bool editable;
    private string uniqueKey;
    private readonly string fileNameEventInfoDB = "_EventInfos";
    private readonly string defaultInfo = "Describe your event here...";

    private void OnEnable()
    {
        ScriptableEvent scriptableEvent = (ScriptableEvent)target;
        uniqueKey = $"{scriptableEvent.name}_EventInfo";

        if (!string.IsNullOrEmpty(scriptableEvent.name))
        {
            LoadEventInfo();
        }
        if (eventInfo != defaultInfo) instructionsDismissed = true;
    }

    public override void OnInspectorGUI()
    {
        ScriptableEvent scriptableEvent = (ScriptableEvent)target;

        // Display Fun2Play logo
        Fun2PlayLogoDrawer.DrawLogo();

        serializedObject.Update();

        // Instructions
        if (!instructionsDismissed)
        {
            EditorGUILayout.HelpBox("\nA Scriptable Event can have up to 3 parameters. Currently supported types are: GameObject, Int, Float, String, Bool, Vector2, Vector3, Sprite.\n\n" +
                "To raise an event use the Raise() method in your C# script or the Playmaker action 'Raise Scriptable Event'.\n\n" +
                "To register a new listener, attach a 'Scriptable Event Listener' component to a GameObject in your scene and drag this Scriptable Event into the corresponding field.\n", MessageType.Info);
            EditorGUILayout.Space(10);
        }

        // Define the rect for the HelpBox
        Rect helpBoxRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // Show Event Information Field
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        float buttonWidth = 50f;

        EditorGUIUtility.labelWidth = 70f;

        // Display the label with a custom layout
        GUILayout.Label("Event Info:", EditorStyles.label, GUILayout.Width(EditorGUIUtility.labelWidth));

        // Calculate the available width for the TextArea
        float availableWidth = EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - (buttonWidth + 35f);

        // If in editing mode, show text field and "Done" button
        if (editable)
        {
            // Limit the width of the text area dynamically
            eventInfo = EditorGUILayout.TextArea(eventInfo, GUILayout.MaxWidth(availableWidth));

            // Save the event information to the JSON file
            if (GUILayout.Button("Save", GUILayout.Width(buttonWidth)) || (Event.current.isKey && Event.current.keyCode == KeyCode.Tab))
            {
                editable = false;
                GUI.FocusControl(null);
                if (string.IsNullOrEmpty(eventInfo)) eventInfo = defaultInfo;
                instructionsDismissed = eventInfo == defaultInfo ? false : true;
                SaveEventInfo();
            }
        }
        else
        {
            // Display the text field
            EditorGUILayout.LabelField(eventInfo, EditorStyles.wordWrappedLabel, GUILayout.ExpandWidth(true));

            // Flexible space to push the button to the right
            GUILayout.FlexibleSpace();

            // If the user clicks the edit button or hits enter
            if (GUILayout.Button("Edit...", GUILayout.Width(buttonWidth)))
            {
                editable = true;
                GUI.FocusControl(null);
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5);

        // End HelpBox
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(20);
        GUIContent raiseButtonContent = new GUIContent("Raise Event", "Raise the event without sending any parameter.");
        if (GUILayout.Button(raiseButtonContent, GUILayout.Height(35)))
        {
            // Manually raise the event
            ((ScriptableEvent)target).Raise(null, null, null);
        }

        // Add a button to open FindScriptableObjectsInUse window
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedHeight = 35f;

        if (GUILayout.Button("Find All References in Scene and Prefabs", buttonStyle))
        {
            System.Reflection.Assembly editorAssembly = System.AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.StartsWith("Assembly-CSharp-Editor,"));
            Type utilityType = editorAssembly.GetTypes().FirstOrDefault(t => t.FullName.Contains("FindScriptableObjectsInUse"));

            if (utilityType != null)
            {
                // Open the Editor Window with the parameter
                object[] parameters = new object[] { scriptableEvent };
                utilityType.GetMethod("ShowWindowWithParameter", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Invoke(obj: null, parameters);
            }
            else
            {
                Debug.LogError("FindScriptableObjectsInUse class not found. Add the Fun2Play FindScriptableObjectsInUse script to your project.");
            }
        }
    }

    private string GetFilePath()
    {
        // Get the directory path for storing EventInfo files
        string directoryPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(target));
        // Get the file path for the text file with .txt extension
        return Path.Combine(directoryPath, $"{fileNameEventInfoDB}.txt");
    }

    private void LoadEventInfo()
    {
        string filePath = GetFilePath();

        // Set eventInfo to defaultInfo by default
        eventInfo = defaultInfo;

        if (File.Exists(filePath))
        {
            try
            {
                string allEventInfo = File.ReadAllText(filePath);
                string[] entries = allEventInfo.Split(new string[] { "~/~" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string entry in entries)
                {
                    // Remove leading and trailing whitespaces and newlines from the entry
                    string trimmedEntry = entry.Trim();

                    string[] parts = trimmedEntry.Split(new string[] { "|~|" }, StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        if (parts[0] == uniqueKey)
                        {
                            eventInfo = parts[1];
                            if (string.IsNullOrEmpty(eventInfo))
                            {
                                eventInfo = defaultInfo;
                            }
                            // Exit the loop if key is found
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load event information: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Event information file not found at path: " + filePath);
        }
    }

    private void SaveEventInfo()
    {
        string filePath = GetFilePath();
        try
        {
            // Build the new entry
            string newEntry = $"{uniqueKey}|~|{eventInfo}";

            // Read existing lines from the file, if it exists
            string[] existingEntries = File.Exists(filePath) ? File.ReadAllLines(filePath) : new string[0];

            // Remove existing entry if uniqueKey is found
            List<string> updatedEntries = new List<string>();
            bool withinEntry = false;

            foreach (string entry in existingEntries)
            {
                // Check if within the entry to be deleted
                if (withinEntry)
                {
                    // Check if entry is a delimiter, indicating end of entry
                    if (entry == "~/~")
                    {
                        // Found end of entry, reset withinEntry flag
                        withinEntry = false;
                        continue;
                    }
                    // Skip adding lines within the entry to be deleted
                    continue;
                }

                string[] parts = entry.Split(new string[] { "|~|" }, StringSplitOptions.None);
                if (parts.Length == 2 && parts[0] == uniqueKey)
                {
                    // Set flag to indicate currently within the entry to be deleted
                    withinEntry = true;
                    continue; // Skip adding existing entry
                }
                updatedEntries.Add(entry);
            }

            // Add new entry unconditionally
            updatedEntries.Add(newEntry);

            // Add delimiter after adding the new entry
            updatedEntries.Add("~/~");

            // Write the updated entries back to the file
            File.WriteAllLines(filePath, updatedEntries);

            Debug.Log("Event information saved successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save event information: " + e.Message);
        }

        // Refresh the AssetDatabase to ensure the changes are immediately visible in the Unity Editor
        AssetDatabase.Refresh();
    }

}
#endif

#endregion