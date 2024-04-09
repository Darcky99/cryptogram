using UnityEngine;
using UnityEngine.Events;
using HutongGames.PlayMaker;
using UnityEditor;

public class ScriptableEventListener : MonoBehaviour
{
    [UnityEngine.Tooltip("The Scriptable Event object to listen to")]
    public ScriptableEvent scriptableEvent;
    [UnityEngine.Tooltip("The type of subscribtion to listen to the Scriptable Event at Runtime")]
    public Subscribe subscribe = Subscribe.UntilDestroy;
    [UnityEngine.Tooltip("Playmaker FSMs can't receive parameters using Unity Events. Set this to TRUE to pass it manually just before the Unity event is sent.\n\nNote: can also be used without the event to only pass a variable.")]
    public bool enableFSMParameter;
    [UnityEngine.Tooltip("The name of the FSM.")]
    public string targetFsmName;
    [UnityEngine.Tooltip("The name of the variable to store parameter 1 in.")]
    public string targetVariableName;
    [UnityEngine.Tooltip("The name of the variable to store parameter 2 in.")]
    public string targetVariableName2;
    [UnityEngine.Tooltip("The name of the variable to store parameter 3 in.")]
    public string targetVariableName3;

    public bool enableEvents;
    public UnityEvent<object, object, object> unityEvent;

    public enum Subscribe
    {
        UntilDestroy,
        UntilDisable
    }

    private PlayMakerFSM _targetFSM;

    private void Awake()
    {
        if (scriptableEvent == null)
            return;
        if (subscribe == Subscribe.UntilDestroy)
            SubscribeToEvent();
    }

    public void Start()
    {
        if (scriptableEvent == null)
            Debug.LogError("The ScriptableEventListener attached to " + gameObject + " has a missing 'ScriptableEvent ScriptableObject'. Delete the ScriptableEventListener component if you are not using it anymore.");

        if (enableFSMParameter && !string.IsNullOrEmpty(targetFsmName))
        {
            _targetFSM = PlayMakerFSM.FindFsmOnGameObject(gameObject, targetFsmName);
            if (_targetFSM == null)
            {
                Debug.LogError("Failed to find FSM '" + targetFsmName + "' on GameObject '" + gameObject.name + "'.");
                return;
            }
        }
    }

    private void OnEnable()
    {
        if (scriptableEvent == null)
            return;

        if (subscribe == Subscribe.UntilDisable)
            SubscribeToEvent();
    }

    private void OnDisable()
    {
        if (scriptableEvent != null && subscribe == Subscribe.UntilDisable)
            UnsubscribeFromEvent();
    }

    private void OnDestroy()
    {
        if (scriptableEvent != null && subscribe == Subscribe.UntilDestroy)
            UnsubscribeFromEvent();
    }

    private void SubscribeToEvent()
    {
        if (scriptableEvent != null)
            scriptableEvent.unityEvent.AddListener(OnEventRaised);
    }

    private void UnsubscribeFromEvent()
    {
        if (scriptableEvent != null)
            scriptableEvent.unityEvent.RemoveListener(OnEventRaised);
    }


    public void OnEventRaised(object parameter1, object parameter2, object parameter3)
    {
        if (enableFSMParameter)
        {
            // Set FSM Variable
            if (!string.IsNullOrEmpty(targetVariableName) && (parameter1 != null) && (_targetFSM != null)) SetFsmVariable(targetVariableName, parameter1);
            if (!string.IsNullOrEmpty(targetVariableName2) && (parameter2 != null) && (_targetFSM != null)) SetFsmVariable(targetVariableName2, parameter2);
            if (!string.IsNullOrEmpty(targetVariableName3) && (parameter3 != null) && (_targetFSM != null)) SetFsmVariable(targetVariableName3, parameter3);
        }

        // Send Unity Event
        if (enableEvents)
        {
            unityEvent.Invoke(parameter1, parameter2, parameter3);
        }
    }

    private void SetFsmVariable(string variableName, object value)
    {
        if (_targetFSM == null)
        {
            Debug.LogWarning("Target FSM is null. Aborting SetFsmVariable in Scriptable Event Listener '" + scriptableEvent.name + "' on game object '" + gameObject.name + "'.", gameObject);
            return;
        }

        if (value == null)
        {
                Debug.LogWarning("Value is null. Aborting SetFsmVariable in Scriptable Event Listener '" + scriptableEvent.name + "' on game object '" + gameObject.name + "'.", gameObject);
            return;
        }

        if (value is GameObject)
        {
            FsmGameObject fsmGameObject = _targetFSM.FsmVariables.GetFsmGameObject(variableName);
            if (fsmGameObject != null)
            {
                fsmGameObject.Value = (GameObject)value;
            }
        }
        else if (value is int)
        {
            FsmInt fsmInt = _targetFSM.FsmVariables.GetFsmInt(variableName);
            if (fsmInt != null)
            {
                fsmInt.Value = (int)value;
            }
        }
        else if (value is float)
        {
            FsmFloat fsmFloat = _targetFSM.FsmVariables.GetFsmFloat(variableName);
            if (fsmFloat != null)
            {
                fsmFloat.Value = (float)value;
            }
        }
        else if (value is string)
        {
            FsmString fsmString = _targetFSM.FsmVariables.GetFsmString(variableName);
            if (fsmString != null)
            {
                fsmString.Value = (string)value;
            }
        }
        else if (value is bool)
        {
            FsmBool fsmBool = _targetFSM.FsmVariables.GetFsmBool(variableName);
            if (fsmBool != null)
            {
                fsmBool.Value = (bool)value;
            }
        }
        else if (value is Vector2)
        {
            FsmVector2 fsmVector2 = _targetFSM.FsmVariables.GetFsmVector2(variableName);
            if (fsmVector2 != null)
            {
                fsmVector2.Value = (Vector2)value;
            }
        }
        else if (value is Vector3)
        {
            FsmVector3 fsmVector3 = _targetFSM.FsmVariables.GetFsmVector3(variableName);
            if (fsmVector3 != null)
            {
                fsmVector3.Value = (Vector3)value;
            }
        }
        else if (value is Sprite)
        {
            FsmObject fsmObject = _targetFSM.FsmVariables.GetFsmObject(variableName);
            if (fsmObject != null)
            {
                fsmObject.Value = (Sprite)value;
            }
        }
    }
}

//[ExecuteAlways]
#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableEventListener))]
public class ScriptableEventListenerEditor : Editor
{
    private SerializedProperty _scriptableEvent;
    private SerializedProperty _subscribe;
    private SerializedProperty _enableFSMParameter;
    private SerializedProperty _targetFsmName;
    private SerializedProperty _targetVariableName;
    private SerializedProperty _targetVariableName2;
    private SerializedProperty _targetVariableName3;
    private SerializedProperty _enableEvents;
    private SerializedProperty _unityEvent;

    void OnEnable()
    {
        _scriptableEvent = serializedObject.FindProperty("scriptableEvent");

        _subscribe = serializedObject.FindProperty("subscribe");
        _enableFSMParameter = serializedObject.FindProperty("enableFSMParameter");
        _targetFsmName = serializedObject.FindProperty("targetFsmName");
        _targetVariableName = serializedObject.FindProperty("targetVariableName");
        _targetVariableName2 = serializedObject.FindProperty("targetVariableName2");
        _targetVariableName3 = serializedObject.FindProperty("targetVariableName3");
        _enableEvents = serializedObject.FindProperty("enableEvents");
        _unityEvent = serializedObject.FindProperty("unityEvent");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Display Fun2Play logo
        Fun2PlayLogoDrawer.DrawLogo();

        EditorGUILayout.PropertyField(_scriptableEvent);

        EditorGUILayout.PropertyField(_subscribe);

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Playmaker", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_enableFSMParameter, new GUIContent("Use Parameters"));
        if (_enableFSMParameter.boolValue)
        {
            EditorGUILayout.PropertyField(_targetFsmName);
            EditorGUILayout.PropertyField(_targetVariableName, new GUIContent("Variable 1"));
            EditorGUILayout.PropertyField(_targetVariableName2, new GUIContent("Variable 2"));
            EditorGUILayout.PropertyField(_targetVariableName3, new GUIContent("Variable 3"));

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