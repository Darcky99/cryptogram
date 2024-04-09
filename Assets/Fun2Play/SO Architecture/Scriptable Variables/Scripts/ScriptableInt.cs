#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using System;
#endif
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewScriptableInt", menuName = "Fun2Play/SO Architecture/Scriptable Variables/Int")]
public class ScriptableInt : ScriptableObject
{
    public class ScriptableIntEvent : UnityEvent<int> { }
    public ScriptableIntEvent onValueChanged = new ScriptableIntEvent();

    public int value;
    [Tooltip("Set an initial Value for the Scriptable Int. When using auto-save or saving manually, this value is only used for the first app start.")]
    public int initialValue;
    [Tooltip("Set to TRUE to save the Scriptable Int and make it persistent throughout scenes and builds.\n\nAlternatively, you can use the method SaveScriptableVariable() to save manually.")]
    public bool autoSave;
    [Tooltip("Set to TRUE if you need the Scriptable Int to use a min/max value.")]
    public bool clamp = false;
    public int minValue = 0;
    public int maxValue = 100;

    private string _key;
    [SerializeField] private List<ScriptableIntListener> _currentListeners = new List<ScriptableIntListener>();


    #region Scriptable Float Methods

    private void OnEnable()
    {
        ConstructKey();
        LoadScriptableVariable();
    }

    public void SetValue(int newValue)
    {
        value = clamp ? Mathf.Clamp(newValue, minValue, maxValue) : newValue;
        onValueChanged.Invoke(value);

        if (autoSave) SaveScriptableVariable();
    }

    public void AddValue(int amount)
    {
        value = clamp ? Mathf.Clamp(value + amount, minValue, maxValue) : value + amount;
        onValueChanged.Invoke(value);

        if (autoSave) SaveScriptableVariable();
    }

    #endregion


    #region Load & Save

    private void ConstructKey()
    {
        _key = "ScriptableInt_" + this.name.Replace(" ", "");
    }

    public void SaveScriptableVariable()
    {
        ES3.Save(_key, value);
    }

    public void LoadScriptableVariable()
    {
        value = ES3.Load(_key, defaultValue: initialValue);
    }

    #endregion
}


#region Editor Script

#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableInt))]
public class ScriptableIntEditor : Editor
{
    private SerializedProperty valueProperty;
    private SerializedProperty autoSaveProperty;
    private SerializedProperty initialValueProperty;
    private SerializedProperty clampProperty;
    private SerializedProperty minValueProperty;
    private SerializedProperty maxValueProperty;

    private void OnEnable()
    {
        valueProperty = serializedObject.FindProperty("value");
        autoSaveProperty = serializedObject.FindProperty("autoSave");
        initialValueProperty = serializedObject.FindProperty("initialValue");
        clampProperty = serializedObject.FindProperty("clamp");
        minValueProperty = serializedObject.FindProperty("minValue");
        maxValueProperty = serializedObject.FindProperty("maxValue");
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
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(clampProperty);

        ScriptableInt scriptableInt = (ScriptableInt)target;
        if (scriptableInt.clamp)
        {
            EditorGUILayout.PropertyField(minValueProperty);
            EditorGUILayout.PropertyField(maxValueProperty);
            scriptableInt.value = Mathf.Clamp(scriptableInt.value, scriptableInt.minValue, scriptableInt.maxValue);
        }

        if (EditorGUI.EndChangeCheck())
        {
            // Call SetValue directly with the value from valueProperty so Inspector changes are recognized
            scriptableInt.SetValue(((ScriptableInt)target).value);
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
                object[] parameters = new object[] { scriptableInt };
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