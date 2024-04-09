#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Linq;
#endif
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewScriptableBool", menuName = "Fun2Play/SO Architecture/Scriptable Variables/Bool")]
public class ScriptableBool : ScriptableObject
{
    public class ScriptableBoolEvent : UnityEvent<bool> { }
    public ScriptableBoolEvent onValueChanged = new ScriptableBoolEvent();

    public bool value;
    [Tooltip("Set an initial Value for the Scriptable Bool. When using auto-save or saving manually, this value is only used at the first app start.")]
    public bool initialValue;
    [Tooltip("Set to TRUE to save the Scriptable Bool and make it persistent throughout scenes and builds.\n\nAlternatively, you can use the method SaveScriptableVariable() to save manually.")]
    public bool autoSave;

    private string _key;
    [SerializeField] private List<ScriptableBoolListener> _currentListeners = new List<ScriptableBoolListener>();


    #region Event Registry

    private void OnValidate()
    {
        _currentListeners.RemoveAll(listener => listener == null);
    }

    public void RegisterListener(ScriptableBoolListener listener)
    {
        if (listener == null || _currentListeners.Contains(listener))
            return;

        _currentListeners.Add(listener);
    }

    public void UnregisterListener(ScriptableBoolListener listener)
    {
        _currentListeners.Remove(listener);
    }

    #endregion


    #region Scriptable Bool Methods

    private void OnEnable()
    {
        ConstructKey();
        LoadScriptableVariable();
    }

    public void SetValue(bool newValue)
    {
        value = newValue;
        onValueChanged.Invoke(value);

        if (autoSave) SaveScriptableVariable();
    }

    #endregion


    #region Load & Save

    private void ConstructKey()
    {
        _key = "ScriptableBool_" + this.name.Replace(" ", "");
    }

    public void SaveScriptableVariable()
    {
        ES3.Save(_key, value);
    }

    public void LoadScriptableVariable()
    {
        value = ES3.Load<bool>(_key, defaultValue: initialValue);
    }

    #endregion
}


#region Editor Script

#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableBool))]
public class ScriptableBoolEditor : Editor
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

        // Display Fun2Play logo
        Fun2PlayLogoDrawer.DrawLogo();

        EditorGUILayout.PropertyField(valueProperty);
        EditorGUILayout.PropertyField(initialValueProperty);
        EditorGUILayout.PropertyField(autoSaveProperty);

        ScriptableBool scriptableBool = (ScriptableBool)target;

        if (EditorGUI.EndChangeCheck())
        {
            // Call SetValue directly with the value from valueProperty so Inspector changes are recognized
            scriptableBool.SetValue(((ScriptableBool)target).value);
        }

        EditorGUILayout.Space(20);

        // Button to open FindScriptableObjectsInUse window
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedHeight = 35f;

        if (GUILayout.Button("Find All References in Scene and Prefabs", buttonStyle))
        {
            System.Reflection.Assembly editorAssembly = System.AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.StartsWith("Assembly-CSharp-Editor,"));
            Type utilityType = editorAssembly.GetTypes().FirstOrDefault(t => t.FullName.Contains("FindScriptableObjectsInUse"));

            if (utilityType != null)
            {
                // Open the Editor Window with the parameter
                object[] parameters = new object[] { scriptableBool };
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

