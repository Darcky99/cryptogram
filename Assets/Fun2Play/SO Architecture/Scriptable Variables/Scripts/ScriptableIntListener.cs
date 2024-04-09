using UnityEngine;
using UnityEditor;
using HutongGames.PlayMaker;
using UnityEngine.Events;

public class ScriptableIntListener : MonoBehaviour
{
    [SerializeField]
    [UnityEngine.Tooltip("The Scriptable Variable object to listen to")]
    private ScriptableInt _scriptableIntObject;
    [SerializeField]
    [UnityEngine.Tooltip("The Int value of the scriptable variable")]
    private int _value;
    [SerializeField]
    [UnityEngine.Tooltip("The type of subscription to listen to the Scriptable Int at Runtime")]
    private Subscribe _subscribe = Subscribe.UntilDestroy;
    [SerializeField]
    [UnityEngine.Tooltip("Enable to update a FSM Variable on every value changed event.")]
    private bool _syncVariable;
    [SerializeField]
    [UnityEngine.Tooltip("The name of the FSM.")]
    private string _targetFsmName;
    [SerializeField]
    [UnityEngine.Tooltip("The name of the variable to store the scriptable variable value in.")]
    private string _targetVarName;
    [SerializeField]
    [UnityEngine.Tooltip("Alternatively, you can send a Unity Event every time the Scriptable Variable changes its value.")]
    private bool _enableEvents;
    [SerializeField]
    private UnityEvent<object> unityEvent;

    private FsmInt _fsmInt;

    public enum Subscribe
    {
        UntilDestroy,
        UntilDisable
    }


    #region Play Mode Initialization

    private void Awake()
    {
        if (_scriptableIntObject == null && Application.isPlaying)
        {
            Debug.LogError("The ScriptableIntListener attached to " + gameObject + " has a missing 'ScriptableInt' scriptable object. Delete the ScriptableIntListener component if you are not using it anymore.");
            return;
        }

        if (_scriptableIntObject != null && _subscribe == Subscribe.UntilDestroy)
        {
            _scriptableIntObject.onValueChanged.AddListener(OnValueChanged);
        }
    }

    private void Start()
    {
        if (!Application.isPlaying)
            return;

        if (_syncVariable)
        {
            if (string.IsNullOrEmpty(_targetFsmName) || string.IsNullOrEmpty(_targetVarName))
            {
                Debug.LogError($"The ScriptableIntListener attached to the GameObject '{gameObject.name}' has missing string values and can not connect to the Playmaker FSM.", this);
                return;
            }

            var _fsm = PlayMakerFSM.FindFsmOnGameObject(gameObject, _targetFsmName);
            if (_fsm == null)
            {
                Debug.LogError($"Playmaker FSM with the name '{_targetFsmName}' was not found on GameObject '{gameObject.name}'. Verify the 'Target FSM Name' string on the ScriptableIntListener component.", this);
                return;
            }
            else
            {
                _fsmInt = _fsm.FsmVariables.GetFsmInt(_targetVarName);

                if (_fsmInt == null)
                {
                    Debug.LogError($"A Playmaker FSM Int variable with the name '{_targetVarName}' was not found on Playmaker FSM '{_targetFsmName}' attached to GameObject '{gameObject.name}'. Verify the 'Target Variable Name' string on the ScriptableIntListener component.", this);
                    return;
                }
            }

            //Get Initial Value
            _value = _scriptableIntObject.value;
            _fsmInt.Value = _value;
        }
    }

    #endregion


    #region Event Listeners

    private void OnEnable()
    {
        if (_scriptableIntObject != null && _subscribe == Subscribe.UntilDisable)
        {
            _scriptableIntObject.onValueChanged.AddListener(OnValueChanged);
        }
    }

    private void OnDisable()
    {
        if (_scriptableIntObject != null && _subscribe == Subscribe.UntilDisable)
        {
            _scriptableIntObject.onValueChanged.RemoveListener(OnValueChanged);
        }
    }

    private void OnDestroy()
    {
        if (_scriptableIntObject != null && _subscribe == Subscribe.UntilDestroy)
        {
            _scriptableIntObject.onValueChanged.RemoveListener(OnValueChanged);
        }
    }

    #endregion


    #region Event Handlers

    public void OnValueChanged(int newValue)
    {
        if (_syncVariable)
        {
            _value = newValue;
            _fsmInt.Value = newValue;
        }

        if (_enableEvents)
        {
            unityEvent.Invoke(newValue);
        }
    }

    #endregion
}


#region Editor Script

#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableIntListener))]
public class ScriptableIntListenerEditor : Editor
{
    private SerializedProperty scriptableIntObjectProperty;
    private SerializedProperty valueProperty;
    private SerializedProperty subscribeProperty;
    private SerializedProperty syncVariableProperty;
    private SerializedProperty targetFsmNameProperty;
    private SerializedProperty targetVarNameProperty;
    private SerializedProperty enableEventsProperty;
    private SerializedProperty unityEventProperty;

    void OnEnable()
    {
        scriptableIntObjectProperty = serializedObject.FindProperty("_scriptableIntObject");
        valueProperty = serializedObject.FindProperty("_value");
        subscribeProperty = serializedObject.FindProperty("_subscribe");
        syncVariableProperty = serializedObject.FindProperty("_syncVariable");
        targetFsmNameProperty = serializedObject.FindProperty("_targetFsmName");
        targetVarNameProperty = serializedObject.FindProperty("_targetVarName");
        enableEventsProperty = serializedObject.FindProperty("_enableEvents");
        unityEventProperty = serializedObject.FindProperty("unityEvent");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Display Fun2Play logo
        Fun2PlayLogoDrawer.DrawLogo();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(scriptableIntObjectProperty);

        if (scriptableIntObjectProperty.objectReferenceValue != null)
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField("Current Value", valueProperty.intValue.ToString());
            }
            else
            {
                EditorGUILayout.LabelField("Current Value", "[Runtime only]");
            }
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(subscribeProperty);

        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(syncVariableProperty);
        if (syncVariableProperty.boolValue)
        {
            EditorGUILayout.PropertyField(targetFsmNameProperty);
            EditorGUILayout.PropertyField(targetVarNameProperty);
            EditorGUILayout.Space(5);
        }

        EditorGUILayout.PropertyField(enableEventsProperty);
        if (enableEventsProperty.boolValue)
        {
            EditorGUILayout.PropertyField(unityEventProperty);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif

#endregion