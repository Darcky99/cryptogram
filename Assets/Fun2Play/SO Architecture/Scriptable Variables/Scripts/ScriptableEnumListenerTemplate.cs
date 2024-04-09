using UnityEngine;
using UnityEditor;
using HutongGames.PlayMaker;
using UnityEngine.Events;
using System;

public class ScriptableEnumListenerTemplate : MonoBehaviour
{
    [SerializeField]
    [UnityEngine.Tooltip("The Scriptable Variable object to listen to")]
    private ScriptableEnumTemplate scriptableEnum;
    [SerializeField]
    [UnityEngine.Tooltip("The Enum value of the scriptable variable")]
    private Enum value;
    [SerializeField]
    [UnityEngine.Tooltip("The type of subscription to listen to the Scriptable Bool at Runtime")]
    private Subscribe subscribe = Subscribe.UntilDestroy;
    [SerializeField]
    [UnityEngine.Tooltip("Enable to update a FSM Variable on every value changed event.")]
    private bool syncVariable;
    [SerializeField]
    [UnityEngine.Tooltip("The name of the FSM.")]
    private string targetFsmName;
    [SerializeField]
    [UnityEngine.Tooltip("The name of the variable to store the scriptable variable value in.")]
    private string targetVarName;
    [SerializeField]
    [UnityEngine.Tooltip("Alternatively, you can send an event every time the Scriptable Variable changes its value.")]
    private bool enableEvents;
    public UnityEvent<object> unityEvent;

    private FsmEnum _fsmEnum;

    public enum Subscribe
    {
        UntilDestroy,
        UntilDisable
    }


    #region Play Mode Initialization

    private void Awake()
    {
        if (scriptableEnum == null && Application.isPlaying)
        {
            Debug.LogError("The ScriptableBoolListener attached to " + gameObject + " has a missing 'ScriptableBool' scriptable object. Delete the ScriptableBoolListener component if you are not using it anymore.");
            return;
        }

        if (scriptableEnum != null && subscribe == Subscribe.UntilDestroy)
        {
            //[Placeholder]scriptableEnum.onValueChanged.AddListener(OnValueChanged);
        }
    }

    private void Start()
    {
        if (!Application.isPlaying)
            return;

        if (syncVariable)
        {
            if (string.IsNullOrEmpty(targetFsmName) || string.IsNullOrEmpty(targetVarName))
            {
                Debug.LogError($"The ScriptableBoolListener attached to the GameObject '{gameObject.name}' has missing string values and can not connect to the Playmaker FSM.", this);
                return;
            }

            var _fsm = PlayMakerFSM.FindFsmOnGameObject(gameObject, targetFsmName);
            if (_fsm == null)
            {
                Debug.LogError($"Playmaker FSM with the name '{targetFsmName}' was not found on GameObject '{gameObject.name}'. Verify the 'Target FSM Name' string on the ScriptableBoolListener component.", this);
                return;
            }
            else
            {
                _fsmEnum = _fsm.FsmVariables.GetFsmEnum(targetVarName);

                if (_fsmEnum == null)
                {
                    Debug.LogError($"A Playmaker FSM Bool variable with the name '{targetVarName}' was not found on Playmaker FSM '{targetFsmName}' attached to GameObject '{gameObject.name}'. Verify the 'Target Variable Name' string on the ScriptableBoolListener component.", this);
                    return;
                }
            }

            // Get Initial Value
            value = scriptableEnum.value;
            _fsmEnum.Value = value;
        }
    }

    #endregion


    #region Event Listeners

    private void OnEnable()
    {
        if (scriptableEnum != null && subscribe == Subscribe.UntilDisable)
        {
            //[Placeholder]scriptableEnum.onValueChanged.AddListener(OnValueChanged);
        }
    }

    private void OnDisable()
    {
        if (scriptableEnum != null && subscribe == Subscribe.UntilDisable)
        {
            //[Placeholder]scriptableEnum.onValueChanged.RemoveListener(OnValueChanged);
        }
    }

    private void OnDestroy()
    {
        if (scriptableEnum != null && subscribe == Subscribe.UntilDestroy)
        {
            //[Placeholder]scriptableEnum.onValueChanged.RemoveListener(OnValueChanged);
        }
    }

    #endregion


    #region Event Handlers

    public void OnValueChanged(Enum newValue)
    {
        if (syncVariable)
        {
            value = newValue;
            _fsmEnum.Value = newValue;
        }

        if (enableEvents)
        {
            unityEvent.Invoke(newValue);
        }
    }

    #endregion


    #region Editor Script

#if UNITY_EDITOR
    [CustomEditor(typeof(ScriptableEnumListenerTemplate))]
    public class ScriptableEnumListenerTemplateEditor : Editor
    {
        private SerializedProperty _scriptableEnum;
        private SerializedProperty _value;
        private SerializedProperty _subscribe;
        private SerializedProperty _syncVariable;
        private SerializedProperty _targetFsmName;
        private SerializedProperty _targetVarName;
        private SerializedProperty _enableEvents;
        private SerializedProperty _unityEvent;

        void OnEnable()
        {
            _scriptableEnum = serializedObject.FindProperty(nameof(scriptableEnum));
            _value = serializedObject.FindProperty(nameof(value));
            _subscribe = serializedObject.FindProperty(nameof(subscribe));
            _syncVariable = serializedObject.FindProperty(nameof(syncVariable));
            _targetFsmName = serializedObject.FindProperty(nameof(targetFsmName));
            _targetVarName = serializedObject.FindProperty(nameof(targetVarName));
            _enableEvents = serializedObject.FindProperty(nameof(enableEvents));
            _unityEvent = serializedObject.FindProperty(nameof(unityEvent));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Display Fun2Play logo
            Fun2PlayLogoDrawer.DrawLogo();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_scriptableEnum);

            if (_scriptableEnum.objectReferenceValue != null)
            {
                if (Application.isPlaying)
                {
                    //[Placeholder]EditorGUILayout.LabelField("Current Value", ((Enum)_value.enumValueIndex).ToString());
                }
                else
                {
                    EditorGUILayout.LabelField("Current Value", "[Runtime only]");
                }
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(_subscribe);

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(_syncVariable);
            if (_syncVariable.boolValue)
            {
                EditorGUILayout.PropertyField(_targetFsmName);
                EditorGUILayout.PropertyField(_targetVarName);
                EditorGUILayout.Space(5);
            }

            EditorGUILayout.PropertyField(_enableEvents);
            if (_enableEvents.boolValue)
            {
                EditorGUILayout.PropertyField(_unityEvent);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    #endregion
}
