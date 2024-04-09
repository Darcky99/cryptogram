#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Linq;
#endif
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewScriptableFloat", menuName = "Fun2Play/SO Architecture/Scriptable Variables/Float")]
public class ScriptableFloat : ScriptableObject
{
    public class ScriptableFloatEvent : UnityEvent<float> { }
    public ScriptableFloatEvent onValueChanged = new ScriptableFloatEvent();

    public float value;
    [Tooltip("Set an initial Value for the Scriptable Float. When using auto-save or saving manually, this value is only used for the first app start.")]
    public float initialValue;
    [Tooltip("Set to TRUE to save the Scriptable Float and make it persistent throughout scenes and builds.\n\nAlternatively, you can use the method SaveScriptableVariable() to save manually.")]
    public bool autoSave;
    [Tooltip("Set to TRUE if you need the scriptable float to use a min/max value.")]
    public bool clamp = false;
    public float minValue = 0f;
    public float maxValue = 100f;

    private string _key;
    [SerializeField] private List<ScriptableFloatListener> _currentListeners = new List<ScriptableFloatListener>();


    #region Scriptable Float Methods

    private void OnEnable()
    {
        ConstructKey();
        LoadScriptableVariable();
    }

    public void SetValue(float newValue)
    {
        value = clamp ? Mathf.Clamp(newValue, minValue, maxValue) : newValue;
        onValueChanged.Invoke(value);

        if (autoSave) SaveScriptableVariable();
    }

    public void AddValue(float amount)
    {
        value = clamp ? Mathf.Clamp(value + amount, minValue, maxValue ) : value + amount;
        onValueChanged.Invoke(value);

        if (autoSave) SaveScriptableVariable();
    }

    #endregion


    #region Load & Save

    private void ConstructKey()
    {
        _key = "ScriptableFloat_" + this.name.Replace(" ", "");
    }

    public void SaveScriptableVariable()
    {
        ES3.Save(_key, value);
    }

    public void LoadScriptableVariable()
    {
        value = ES3.Load<float>(_key, defaultValue: initialValue);
    }

    #endregion
}


#region Editor Script

#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableFloat))]
public class ScriptableFloatEditor : Editor
{
    private SerializedProperty valueProperty;
    private SerializedProperty initialValueProperty;
    private SerializedProperty autoSaveProperty;
    private SerializedProperty clampProperty;
    private SerializedProperty minValueProperty;
    private SerializedProperty maxValueProperty;

    private void OnEnable()
    {
        valueProperty = serializedObject.FindProperty("value");
        initialValueProperty = serializedObject.FindProperty("initialValue");
        autoSaveProperty = serializedObject.FindProperty("autoSave");
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

        ScriptableFloat scriptableFloat = (ScriptableFloat)target;
        if (scriptableFloat.clamp)
        {
            EditorGUILayout.PropertyField(minValueProperty);
            EditorGUILayout.PropertyField(maxValueProperty);
            scriptableFloat.value = Mathf.Clamp(scriptableFloat.value, scriptableFloat.minValue, scriptableFloat.maxValue);
        }
        
        if (EditorGUI.EndChangeCheck())
        {
            // Call SetValue directly with the value from valueProperty so Inspector changes are recognized
            scriptableFloat.SetValue(((ScriptableFloat)target).value);
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
                object[] parameters = new object[] { scriptableFloat };
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