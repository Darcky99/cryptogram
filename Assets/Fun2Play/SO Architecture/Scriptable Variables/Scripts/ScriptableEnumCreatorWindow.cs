// Fun2Play
// Requires the ScriptableEnumTemplate and ScriptableEnumListenerTemplate
// Any changes made to one of the 3 files requires most probably changes in the others as well

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class CreateScriptableEnumWindow : EditorWindow
{
    private string selectedEnumTypeName;
    private string[] enumTypeNames;
    private string searchFilter;
    private string assetPath = "Assets/";
    private bool userInteracted;


    #region Editor Window

    [MenuItem("Fun2Play/SO Architecture/Scriptable Enum Creator")]
    public static void ShowWindow()
    {
        CreateScriptableEnumWindow window = GetWindow<CreateScriptableEnumWindow>("Scriptable Enum Creator", true);
        window.minSize = new Vector2(400, 340);
    }

    private void OnEnable()
    {
        searchFilter = "";
        // Get all enum type names in the project
        enumTypeNames = GetAllEnumTypeNames();
    }

    private void OnGUI()
    {
        // Display Fun2Play logo
        Fun2PlayLogoDrawer.DrawLogo();

        // Select an Enum Type
        EditorGUILayout.LabelField("Select your Enum Type:");

        // Search filter field
        EditorGUI.BeginChangeCheck();
        EditorGUIUtility.labelWidth = 100f;
        searchFilter = EditorGUILayout.TextField("Search Filter:", searchFilter);
        if (EditorGUI.EndChangeCheck())
        {
            userInteracted = true;
        }

        // Filter enum type names based on search filter and exclude certain enum types
        string[] filteredEnumTypeNames = enumTypeNames
            .Where(enumTypeName =>
                enumTypeName.ToLower().Contains(searchFilter.ToLower()) &&
                !enumTypeName.StartsWith("System") &&
                !enumTypeName.StartsWith("UnityEditor") &&
                !enumTypeName.StartsWith("Unity") &&
                !enumTypeName.StartsWith("Intern") &&
                !enumTypeName.StartsWith("Microsoft") &&
                !enumTypeName.StartsWith("MS") &&
                !enumTypeName.StartsWith("Hutong") &&
                !enumTypeName.StartsWith("Mono"))
            .ToArray();

        // Combine filtered and original enum type names
        string[] combinedEnumTypeNames = filteredEnumTypeNames.Length > 0 ? filteredEnumTypeNames : enumTypeNames;

        // Get the index of the selected enum
        int selectedIndex = Array.IndexOf(combinedEnumTypeNames, selectedEnumTypeName);

        // If there's a new match in the filtered enum type names and the user has interacted, set selectedIndex to the index of the newest match
        if (userInteracted && selectedIndex == -1 && combinedEnumTypeNames.Length > 0)
        {
            selectedIndex = combinedEnumTypeNames.Length - 1;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Enum Type:", GUILayout.Width(100));

        // Dropdown to select enum type
        int newSelectedIndex = EditorGUILayout.Popup(selectedIndex, combinedEnumTypeNames);
        if (newSelectedIndex >= 0)
        {
            // Update the selected enum type name
            selectedEnumTypeName = combinedEnumTypeNames[newSelectedIndex];
            userInteracted = false;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // Select an Enum Type
        EditorGUILayout.LabelField("Select a Save Path for the new scripts:");

        EditorGUILayout.BeginHorizontal();
        // Display the selected folder path with a narrower label width
        EditorGUILayout.LabelField("Output Path:", GUILayout.Width(100));
        EditorGUILayout.TextField(assetPath);

        // Button to open file explorer and select folder with a specific width
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            string selectedFolder = EditorUtility.OpenFolderPanel("Select Save Path", "Assets", "");
            if (!string.IsNullOrEmpty(selectedFolder))
            {
                assetPath = "Assets" + selectedFolder.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(25);

        // Add a button to open FindScriptableObjectsInUse window
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedHeight = 50f;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create", buttonStyle, GUILayout.Width(200)))
        {
            if (!string.IsNullOrEmpty(selectedEnumTypeName))
            {
                // Get the enum type by name
                Type enumType = Type.GetType(selectedEnumTypeName);

                if (enumType != null && enumType.IsEnum)
                {
                    ClearConsole();

                    CreateScriptableEnum(enumType);
                    CreateScriptableEnumListener(enumType);
                    CreateScriptableEnumSetValue(enumType);
                    CreateScriptableEnumGetValue(enumType);

                    // Refresh the asset database to make the new scripts visible in the project
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError("Invalid enum type name. Make sure the type name is correct and represents an enum.");
                }
            }
            else
            {
                Debug.LogError("Please select an enum type before creating ScriptableEnum.");
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // Info box
        EditorGUILayout.HelpBox("\nCreates 4 new files: 'Scriptable Enum', 'Scriptable Enum Listener', 'Set and Get Enum' PlayMaker Actions.\n\n" +
            "The resulting Scriptable Enum will be available at:\nCreate/Fun2Play/SO Architecture/Scriptable Variables.\n", MessageType.Info);
    }

    #endregion


    #region Scriptable Enum

    private void CreateScriptableEnum(Type enumType)
    {
        // Find the template script
        string[] templateScriptPaths = AssetDatabase.FindAssets("ScriptableEnumTemplate t:Script", null);
        if (templateScriptPaths.Length == 0)
        {
            Debug.LogError("Scriptable Enum Template not found. Make sure the template is named 'ScriptableEnumTemplate' and located in the project.");
            return;
        }

        // Get the first template script path (assuming there's only one)
        string templateScriptPath = AssetDatabase.GUIDToAssetPath(templateScriptPaths[0]);

        // Load the template script as a TextAsset
        TextAsset templateScriptAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(templateScriptPath);
        if (templateScriptAsset == null)
        {
            Debug.LogError("Failed to load Scriptable Enum template.");
            return;
        }

        // Prepare Strings
        string templateScriptContent = templateScriptAsset.text;
        string className = "ScriptableEnum" + enumType.Name;
        string enumTypeName = enumType.Name;

        // Remove the enum definition lines using a regular expression
        templateScriptContent = Regex.Replace(templateScriptContent, @"(\r?\n\s*)\[Serializable\]\s*public\s*enum\s*EnumType\s*{\s*Option1,\s*Option2,\s*Option3\s*}", "");
        // Replace enum type declarations with fully qualified name
        templateScriptContent = templateScriptContent.Replace("public EnumType value;", $"public {enumType.FullName} value;");
        templateScriptContent = templateScriptContent.Replace("public EnumType initialValue;", $"public {enumType.FullName} initialValue;");
        // Replace Unity Event
        templateScriptContent = templateScriptContent.Replace("UnityEvent<EnumType>", $"UnityEvent<{enumType}>");
        // Replace methods
        templateScriptContent = templateScriptContent.Replace("public void SetValue(EnumType newValue)", $"public void SetValue({enumType.FullName} newValue)");
        templateScriptContent = templateScriptContent.Replace($"((ScriptableEnumTemplate)target).SetValue((EnumType)valueProperty.enumValueIndex);", $"((ScriptableEnum{enumTypeName})target).SetValue(({enumType.FullName})valueProperty.enumValueIndex);");
        // Class Name
        templateScriptContent = templateScriptContent.Replace("ScriptableEnumTemplate", className); //Replaces all instances in the entire script
        templateScriptContent = templateScriptContent.Replace("ScriptableEnumTemplateEditor", $"{className}Editor");

        //Asset Menu
        templateScriptContent = templateScriptContent.Replace("//[Placeholder] for Asset Menu Creation", $"[CreateAssetMenu(fileName = \"NewScriptableEnum{enumTypeName}\", menuName = \"Fun2Play/SO Architecture/Scriptable Variables/Enum {enumTypeName}\")]");


        // Save the modified scriptable object as a new instance
        string newAssetPath = $"{assetPath}/ScriptableEnum{enumTypeName}.cs";
        System.IO.File.WriteAllText(newAssetPath, templateScriptContent);

        Debug.Log($"Scriptable Enum created at: {newAssetPath}");
    }

    #endregion


    #region Listener

    private void CreateScriptableEnumListener(Type enumType)
    {
        // Find the Scriptable Enum Listener template script
        string[] templateScriptPaths = AssetDatabase.FindAssets("ScriptableEnumListenerTemplate t:Script", null);
        if (templateScriptPaths.Length == 0)
        {
            Debug.LogError("Scriptable Enum Listener Template not found. Make sure the template is named 'ScriptableEnumListenerTemplate' and located in the project.");
            return;
        }

        // Get the first template script path (assuming there's only one)
        string templateScriptPath = AssetDatabase.GUIDToAssetPath(templateScriptPaths[0]);

        // Load the template script as a TextAsset
        TextAsset templateScriptAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(templateScriptPath);
        if (templateScriptAsset == null)
        {
            Debug.LogError("Failed to load Scriptable Enum Listener template.");
            return;
        }

        // Prepare Strings
        string templateScriptContent = templateScriptAsset.text;
        string className = "ScriptableEnumListener" + enumType.Name;
        string enumTypeName = enumType.Name;

        // Class Name
        templateScriptContent = templateScriptContent.Replace("ScriptableEnumListenerTemplate", className);
        templateScriptContent = templateScriptContent.Replace("ScriptableEnumListenerTemplateEditor", $"{className}Editor");

        // Scriptable Enum
        templateScriptContent = templateScriptContent.Replace("ScriptableEnumTemplate", $"ScriptableEnum{enumType.Name}");

        // Enum Type
        templateScriptContent = templateScriptContent.Replace("Enum value", $"{enumType} value");
        templateScriptContent = templateScriptContent.Replace("Enum newValue", $"{enumType} newValue");
        templateScriptContent = templateScriptContent.Replace("//[Placeholder]EditorGUILayout.LabelField(\"Current Value\", ((Enum)_value.enumValueIndex).ToString());", $"EditorGUILayout.LabelField(\"Current Value\", (({enumType})_value.enumValueIndex).ToString());");

        // Unity Events Subscription
        templateScriptContent = templateScriptContent.Replace("//[Placeholder]scriptableEnum.onValueChanged.AddListener(OnValueChanged);", $"scriptableEnum.onValueChanged.AddListener(OnValueChanged);");
        templateScriptContent = templateScriptContent.Replace("//[Placeholder]scriptableEnum.onValueChanged.RemoveListener(OnValueChanged);", $"scriptableEnum.onValueChanged.RemoveListener(OnValueChanged);");

        // Save the modified scriptable object as a new instance
        string newAssetPath = $"{assetPath}/ScriptableEnumListener{enumTypeName}.cs";
        System.IO.File.WriteAllText(newAssetPath, templateScriptContent);

        Debug.Log($"Scriptable Enum Listener created at: {newAssetPath}");
    }

    #endregion


    #region Playmaker Action Set Enum

    private void CreateScriptableEnumSetValue(Type enumType)
    {
        // Find the Playmaker Action Set Value template script
        string[] templateScriptPaths = AssetDatabase.FindAssets("ScriptableEnumSetValueTemplate t:Script", null);
        if (templateScriptPaths.Length == 0)
        {
            Debug.LogError("Scriptable Enum Set Value Template not found. Make sure the playmaker action template is named 'ScriptableEnumSetValueTemplate' and located in the project.");
            return;
        }

        // Get the first template script path (assuming there's only one)
        string templateScriptPath = AssetDatabase.GUIDToAssetPath(templateScriptPaths[0]);

        // Load the template script as a TextAsset
        TextAsset templateScriptAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(templateScriptPath);
        if (templateScriptAsset == null)
        {
            Debug.LogError("Failed to load Scriptable Enum Set Value template.");
            return;
        }

        // Prepare Strings
        string templateScriptContent = templateScriptAsset.text;
        string className = "ScriptableEnum" + enumType.Name + "SetValue";
        string enumTypeName = enumType.Name;

        // Class Name
        templateScriptContent = templateScriptContent.Replace("ScriptableEnumTemplateSetValue", className);

        // Scriptable Enum
        templateScriptContent = templateScriptContent.Replace("ScriptableEnumTemplate", $"ScriptableEnum{enumType.Name}");

        // Activate Action and code for SetValue method
        templateScriptContent = templateScriptContent.Replace("//[Placeholder][ActionCategory(\"Fun2Play: SO Architecture\")]", $"[ActionCategory(\"Fun2Play: SO Architecture\")]");
        templateScriptContent = templateScriptContent.Replace("//[Placeholder]_scriptableEnum.SetValue(value.Value);", $"_scriptableEnum.SetValue(({enumType})value.Value);");

        // Save the modified scriptable object as a new instance
        string newAssetPath = $"{assetPath}/ScriptableEnum{enumTypeName}SetValue.cs";
        System.IO.File.WriteAllText(newAssetPath, templateScriptContent);

        Debug.Log($"Scriptable Enum Set Value PlayMaker action created at: {newAssetPath}");
    }

    #endregion


    #region Playmaker Action Get Enum

    private void CreateScriptableEnumGetValue(Type enumType)
    {
        // Find the Playmaker Action Get Value template script
        string[] templateScriptPaths = AssetDatabase.FindAssets("ScriptableEnumGetValueTemplate t:Script", null);
        if (templateScriptPaths.Length == 0)
        {
            Debug.LogError("Scriptable Enum Get Value Template not found. Make sure the playmaker action template is named 'ScriptableEnumGetValueTemplate' and located in the project.");
            return;
        }

        // Get the first template script path (assuming there's only one)
        string templateScriptPath = AssetDatabase.GUIDToAssetPath(templateScriptPaths[0]);

        // Load the template script as a TextAsset
        TextAsset templateScriptAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(templateScriptPath);
        if (templateScriptAsset == null)
        {
            Debug.LogError("Failed to load Scriptable Enum Get Value template.");
            return;
        }

        // Prepare Strings
        string templateScriptContent = templateScriptAsset.text;
        string className = "ScriptableEnum" + enumType.Name + "GetValue";
        string enumTypeName = enumType.Name;

        // Class Name
        templateScriptContent = templateScriptContent.Replace("ScriptableEnumTemplateGetValue", className);

        // Scriptable Enum
        templateScriptContent = templateScriptContent.Replace("ScriptableEnumTemplate", $"ScriptableEnum{enumType.Name}");

        // Activate Action
        templateScriptContent = templateScriptContent.Replace("//[Placeholder][ActionCategory(\"Fun2Play: SO Architecture\")]", $"[ActionCategory(\"Fun2Play: SO Architecture\")]");

        // Save the modified scriptable object as a new instance
        string newAssetPath = $"{assetPath}/ScriptableEnum{enumTypeName}GetValue.cs";
        System.IO.File.WriteAllText(newAssetPath, templateScriptContent);

        Debug.Log($"Scriptable Enum Set Value PlayMaker action created at: {newAssetPath}");
    }

    #endregion


    #region Find Enums in Project

    private string[] GetAllEnumTypeNames()
    {
        List<string> enumTypeNamesList = new List<string>();

        // Get all types in the assembly
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            foreach (Type type in assembly.GetTypes())
            {
                // Check if the type is an enum
                if (type.IsEnum)
                {
                    // Add the full type name to the list
                    enumTypeNamesList.Add(type.FullName);
                }
            }
        }

        return enumTypeNamesList.ToArray();
    }

    #endregion


    #region Clear Console

    public static void ClearConsole()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    #endregion
}
#endif