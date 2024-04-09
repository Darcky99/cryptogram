#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;

//[Placeholder] for Asset Menu Creation
public class ScriptableEnumTemplate : ScriptableObject
{
    public class ScriptableEnumEvent : UnityEvent<EnumType> { }
    public ScriptableEnumEvent onValueChanged = new ScriptableEnumEvent();

    [Serializable]public enum EnumType { Option1, Option2, Option3 }

    [Tooltip("The current value set")]
    public EnumType value;
    [Tooltip("Set an initial Value for the Scriptable Enum. When using auto-save or saving manually, this value is only used for the first app start. Otherwise it's used on every app start or scene change.")]
    public EnumType initialValue;
    [Tooltip("Set to TRUE to save the Scriptable Enum and make it persistent throughout scenes and builds.\n\nAlternatively, you can call the method SaveScriptableVariable() to save manually.")]
    public bool autoSave;

    private string _key;


    #region Scriptable Enum Methods

    private void OnEnable()
    {
        ConstructKey();
        LoadScriptableVariable();
    }

    public void SetValue(EnumType newValue)
    {
        value = newValue;
        onValueChanged.Invoke(value);

        if (autoSave) SaveScriptableVariable();
    }

    private void ConstructKey()
    {
        _key = "ScriptableEnum_" + name.Replace(" ", "");
    }

    #endregion


    #region Load & Save

    public void SaveScriptableVariable()
    {
        ES3.Save(_key, value);
    }

    public void LoadScriptableVariable()
    {
        value = ES3.Load(_key, defaultValue: initialValue);
    }

    #endregion


    #region Editor Script

#if UNITY_EDITOR
    [CustomEditor(typeof(ScriptableEnumTemplate))]
    public class ScriptableEnumTemplateEditor : Editor
    {
        private SerializedProperty valueProperty;
        private SerializedProperty initialValueProperty;
        private SerializedProperty autoSaveProperty;

        private void OnEnable()
        {
            valueProperty = serializedObject.FindProperty("value");
            initialValueProperty = serializedObject.FindProperty("initialValue");
            autoSaveProperty = serializedObject.FindProperty("autoSave");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            ScriptableEnumTemplate scriptableEnumTarget = (ScriptableEnumTemplate)target;

            GUILayout.BeginHorizontal();
            // Get the type of the enum value
            Type enumType = scriptableEnumTarget.value.GetType();

            // Create a GUIStyle with a larger font size
            GUIStyle infoBoxStyle = new GUIStyle(EditorStyles.helpBox);
            infoBoxStyle.fontSize = 12;
            infoBoxStyle.padding = new RectOffset(10, 10, 5, 5);

            // Display the info message with the specified GUIStyle
            GUILayout.Label($"Enum Type: {enumType}", infoBoxStyle);

            // Display Fun2Play logo
            Fun2PlayLogoDrawer.DrawLogo();
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(20);

            EditorGUILayout.PropertyField(valueProperty);
            EditorGUILayout.PropertyField(initialValueProperty);
            EditorGUILayout.PropertyField(autoSaveProperty);

            if (EditorGUI.EndChangeCheck())
            {
                // Call SetValue directly with the value from valueProperty so Inspector changes are recognized
                ((ScriptableEnumTemplate)target).SetValue((EnumType)valueProperty.enumValueIndex);
            }

            EditorGUILayout.Space(20);

            // Button to open FindScriptableObjectsInUse window
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fixedHeight = 35f;

            if (GUILayout.Button("Find All References in Scene and Prefabs", buttonStyle))
            {
                System.Reflection.Assembly editorAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.StartsWith("Assembly-CSharp-Editor,"));
                Type utilityType = editorAssembly.GetTypes().FirstOrDefault(t => t.FullName.Contains("FindScriptableObjectsInUse"));

                if (utilityType != null)
                {
                    // Open the Editor Window with the parameter
                    object[] parameters = new object[] { scriptableEnumTarget };
                    utilityType.GetMethod("ShowWindowWithParameter", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Invoke(obj: null, parameters);
                }
                else
                {
                    Debug.LogError("FindScriptableObjectsInUse class not found. Add the Fun2Play FindScriptableObjectsInUse script to your project.");
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    #endregion
}