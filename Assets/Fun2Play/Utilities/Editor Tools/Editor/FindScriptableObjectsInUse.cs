using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HutongGames.PlayMaker;
using System.Linq;
using System.Collections;

public class FindScriptableObjectsInUse : EditorWindow
{
    private ScriptableObject targetScriptableObject;
    private ScriptableObject previousScriptableObject;
    private Vector2 _scrollView;
    private List<GameObject> referencingGameObjects;
    private List<(PlayMakerFSM fsm, FsmObject variable)> referencingFSMs;
    private List<GameObject> referencingPrefabGameObjects;
    private List<(PlayMakerFSM fsm, FsmObject variable)> referencingPrefabFSMs;

    private bool includePrefabsInSearch = false;

    [MenuItem("Fun2Play/Tools/Find Scriptable Objects In Use")]
    public static void ShowWindow()
    {
        GetWindow<FindScriptableObjectsInUse>("Scriptable Object References");
    }

    public static void ShowWindowWithParameter(ScriptableObject scriptableObject)
    {
        var window = GetWindow<FindScriptableObjectsInUse>("Scriptable Object References");
        window.targetScriptableObject = scriptableObject;
    }

    private void OnEnable()
    {
        referencingGameObjects = new List<GameObject>();
        referencingFSMs = new List<(PlayMakerFSM, FsmObject)>();
        referencingPrefabGameObjects = new List<GameObject>();
        referencingPrefabFSMs = new List<(PlayMakerFSM, FsmObject)>();

        // Subscribe to the hierarchyWindowChanged event to avoid errors when deleting referenced GameObjects
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe from the hierarchyWindowChanged event
        EditorApplication.hierarchyChanged -= OnHierarchyChanged;
    }

    private void OnHierarchyChanged()
    {
        referencingGameObjects.RemoveAll(item => item == null);
        referencingFSMs = referencingFSMs.Where(tuple => tuple.fsm != null).ToList();
        referencingPrefabGameObjects.RemoveAll(item => item == null);
        referencingPrefabFSMs = referencingPrefabFSMs.Where(tuple => tuple.fsm != null).ToList();
    }

    private void ResetLists()
    {
        referencingGameObjects.Clear();
        referencingFSMs.Clear();
        referencingPrefabGameObjects.Clear();
        referencingPrefabFSMs.Clear();
    }


    #region Editor Window GUI

    private void OnGUI()
    {
        // Display Fun2Play logo
        Fun2PlayLogoDrawer.DrawLogo();

        targetScriptableObject = EditorGUILayout.ObjectField("Scriptable Object", targetScriptableObject, typeof(ScriptableObject), false) as ScriptableObject;

        if (targetScriptableObject != previousScriptableObject)
        {
            ResetLists();
            previousScriptableObject = targetScriptableObject;
            Repaint(); // Force a repaint to reflect changes immediately
        }



        if (targetScriptableObject == null)
        {
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("Drag a ScriptableObject into the field above.", MessageType.Info);
        }
        else
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            includePrefabsInSearch = EditorGUILayout.Toggle("Include Prefabs in Search", includePrefabsInSearch);

            if (includePrefabsInSearch)
            {
                GUILayout.Label(" "); // Add an empty label to create space between the toggle and the help box
                EditorGUILayout.HelpBox("Enabling prefabs in the search can significantly increase search time, especially in larger projects.", MessageType.Info);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fixedHeight = 44f;

            if (GUILayout.Button("Find References", buttonStyle))
            {
                FindReferences();
            }

            if (GUILayout.Button("Reset", buttonStyle))
            {
                ResetLists();
            }

            GUILayout.EndHorizontal();

            DisplayReferences();
        }
    }

    #endregion



    private void FindReferences()
    {
        // Reset All Lists
        ResetLists();

        FindReferencesInGameObjects();
        FindReferencesInPlaymakerFSMs();

        if (includePrefabsInSearch)
        {
            FindReferencesInPrefabGameObjects();
            FindReferencesInPrefabPlaymakerFSMs();
        }
        // Check if any positive results are found in any of the lists
        if (referencingGameObjects.Count == 0 && referencingFSMs.Count == 0 && referencingPrefabGameObjects.Count == 0 && referencingPrefabFSMs.Count == 0)
        {
            ShowNotification(new GUIContent("No references found!"));
        }
    }

    private void DisplayReferences()
    {
        _scrollView = EditorGUILayout.BeginScrollView(_scrollView);

        GUILayout.Space(10);
        DisplayGameObjectReferences();
        GUILayout.Space(10);
        DisplayPlaymakerFSMReferences();
        GUILayout.Space(10);
        DisplayPrefabGameObjectReferences();
        GUILayout.Space(10);
        DisplayPrefabPlaymakerFSMReferences();

        EditorGUILayout.EndScrollView();
    }


    #region GameObjects References in Active Scene

    private void FindReferencesInGameObjects()
    {
        GameObject[] allGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject gameObject in allGameObjects)
        {
            Component[] components = gameObject.GetComponentsInChildren<Component>(true);

            foreach (Component component in components)
            {
                if (component != null && !(component is PlayMakerFSM))
                {
                    var fields = component.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                    foreach (var field in fields)
                    {
                        if (typeof(ScriptableObject).IsAssignableFrom(field.FieldType))
                        {
                            ScriptableObject fieldValue = field.GetValue(component) as ScriptableObject;
                            if (fieldValue == targetScriptableObject)
                            {
                                referencingGameObjects.Add(component.gameObject);
                            }
                        }
                    }
                }
            }
        }
    }

    private void DisplayGameObjectReferences()
    {
        if (referencingGameObjects == null || referencingGameObjects.Count == 0)
            return;

        GUILayout.Label("C# References in acvtive Scene: " + referencingGameObjects.Count, EditorStyles.boldLabel);

        // Define button style with fixed height
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedHeight = 28f;
        buttonStyle.alignment = TextAnchor.MiddleLeft;
        buttonStyle.padding.left = 10;

        // Define icon style to align vertically in the center
        GUIStyle iconStyle = new GUIStyle();
        iconStyle.margin.top = 6;

        foreach (GameObject obj in referencingGameObjects)
        {
            foreach (Component component in obj.GetComponents<Component>())
            {
                GUILayout.BeginHorizontal();

                var fields = component.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                foreach (var field in fields)
                {
                    if (typeof(ScriptableObject).IsAssignableFrom(field.FieldType) && field.Name != "fsmTemplate")
                    {
                        ScriptableObject fieldValue = field.GetValue(component) as ScriptableObject;
                        if (fieldValue == targetScriptableObject)
                        {
                            // Display icons
                            Texture2D iconGO = EditorGUIUtility.IconContent("d_GameObject Icon").image as Texture2D;
                            GUILayout.Label(iconGO, iconStyle, GUILayout.Width(20), GUILayout.Height(20));
                            Texture2D iconScript = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
                            GUILayout.Label(iconScript, iconStyle, GUILayout.Width(20), GUILayout.Height(20));

                            // Construct button text with GameObject name, Component name, and Field name
                            string buttonText = $"{obj.name} \u27A1 {component.GetType().Name} \u27A1 {field.Name}";

                            // Create a new GUIContent object for the button
                            GUIContent buttonContent = new GUIContent(buttonText);

                            // Display button with constructed text
                            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.ExpandWidth(true)))
                            {
                                EditorGUIUtility.PingObject(obj);
                            }
                        }   
                    }
                }

                GUILayout.EndHorizontal();
            }
        }
    }

    #endregion


    #region GameObjects References in Prefabs

    private void FindReferencesInPrefabGameObjects()
    {
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        foreach (string prefabGUID in prefabGUIDs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null)
            {
                Component[] components = prefab.GetComponentsInChildren<Component>(true);
                foreach (Component component in components)
                {
                    if (component != null && !(component is PlayMakerFSM))
                    {
                        var fields = component.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                        foreach (var field in fields)
                        {
                            if (typeof(ScriptableObject).IsAssignableFrom(field.FieldType))
                            {
                                ScriptableObject fieldValue = field.GetValue(component) as ScriptableObject;
                                if (fieldValue == targetScriptableObject)
                                {
                                    referencingPrefabGameObjects.Add(component.gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void DisplayPrefabGameObjectReferences()
    {
        if (referencingPrefabGameObjects == null || referencingPrefabGameObjects.Count == 0)
            return;

        GUILayout.Label("C# References in Prefabs: " + referencingPrefabGameObjects.Count, EditorStyles.boldLabel);

        // Define button style with fixed height
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedHeight = 28f;
        buttonStyle.alignment = TextAnchor.MiddleLeft;
        buttonStyle.padding.left = 10;

        // Define icon style to align vertically in the center
        GUIStyle iconStyle = new GUIStyle();
        iconStyle.margin.top = 6;

        foreach (GameObject prefab in referencingPrefabGameObjects)
        {
            foreach (Component component in prefab.GetComponents<Component>())
            {
                GUILayout.BeginHorizontal();

                var fields = component.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                foreach (var field in fields)
                {
                    if (typeof(ScriptableObject).IsAssignableFrom(field.FieldType) && field.Name != "fsmTemplate")
                    {
                        ScriptableObject fieldValue = field.GetValue(component) as ScriptableObject;
                        if (fieldValue == targetScriptableObject)
                        {
                            // Display icons
                            Texture2D iconGO = EditorGUIUtility.IconContent("d_Prefab Icon").image as Texture2D;
                            GUILayout.Label(iconGO, iconStyle, GUILayout.Width(20), GUILayout.Height(20));
                            Texture2D iconScript = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
                            GUILayout.Label(iconScript, iconStyle, GUILayout.Width(20), GUILayout.Height(20));

                            // Construct button text with GameObject name, Component name, and Field name
                            string buttonText = $"{component.gameObject.transform.name} \u27A1 {component.GetType().Name} \u27A1 {field.Name}";

                            // Create a new GUIContent object for the button
                            GUIContent buttonContent = new GUIContent(buttonText);

                            // Display button with constructed text
                            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.ExpandWidth(true)))
                            {
                                EditorGUIUtility.PingObject(prefab);
                            }
                        }  
                    }
                }

                GUILayout.EndHorizontal();
            }
        }
    }

    #endregion


    #region Playmaker References from Scene

    private void FindReferencesInPlaymakerFSMs()
    {
        GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject rootGameObject in rootGameObjects)
        {
            PlayMakerFSM[] fsms = rootGameObject.GetComponentsInChildren<PlayMakerFSM>(true);
            foreach (PlayMakerFSM fsm in fsms)
            {
                // Access object variables
                foreach (FsmObject objectVariable in fsm.FsmVariables.ObjectVariables)
                {
                    if ((Object)objectVariable.RawValue == targetScriptableObject)
                    {
                        referencingFSMs.Add((fsm, objectVariable));
                    }
                }
            }
        }
    }

    private void DisplayPlaymakerFSMReferences()
    {
        if (referencingFSMs == null || referencingFSMs.Count == 0)
            return;

        GUILayout.Label("Playmaker References in active Scene: " + referencingFSMs.Count, EditorStyles.boldLabel);

        // Define button style with fixed height
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedHeight = 28f;
        buttonStyle.alignment = TextAnchor.MiddleLeft;
        buttonStyle.padding.left = 10;

        // Define icon style to align vertically in the center
        GUIStyle iconStyle = new GUIStyle();
        iconStyle.margin.top = 6;

        foreach ((PlayMakerFSM fsm, FsmObject variable) in referencingFSMs)
        {
            if (((Object)variable.RawValue == targetScriptableObject))
            {
                GUIContent buttonContent = new GUIContent();
                buttonContent.text = $"{fsm.gameObject.name} \u27A1 {fsm.FsmName} \u27A1 {variable.Name}";

                GUILayout.BeginHorizontal();

                // Display icons
                Texture2D iconGO = EditorGUIUtility.IconContent("d_GameObject Icon").image as Texture2D;
                GUILayout.Label(iconGO, iconStyle, GUILayout.Width(20), GUILayout.Height(20));

                Texture2D iconFSM = EditorGUIUtility.IconContent("Assets/_Project/Fun2Play/Utilities/Editor Icons/PlaymakerIcon.png").image as Texture2D;
                GUILayout.Label(iconFSM, iconStyle, GUILayout.Width(20), GUILayout.Height(20));

                // Display button with all information
                if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.ExpandWidth(true)))
                {
                    EditorGUIUtility.PingObject(fsm.gameObject);
                }

                GUILayout.EndHorizontal();
            }   
        }
    }

    #endregion


    #region Playmaker References in Prefabs

    private void FindReferencesInPrefabPlaymakerFSMs()
    {
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        foreach (string prefabGUID in prefabGUIDs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null)
            {
                PlayMakerFSM[] fsms = prefab.GetComponentsInChildren<PlayMakerFSM>(true);
                foreach (PlayMakerFSM fsm in fsms)
                {
                    // Access object variables
                    foreach (FsmObject objectVariable in fsm.FsmVariables.ObjectVariables)
                    {
                        if ((Object)objectVariable.RawValue == targetScriptableObject)
                        //if (objectVariable.RawValue == targetObject)
                        {
                            referencingPrefabFSMs.Add((fsm, objectVariable));
                        }
                    }
                }
            }
        }
    }

    private void DisplayPrefabPlaymakerFSMReferences()
    {
        if (referencingPrefabFSMs == null || referencingPrefabFSMs.Count == 0)
            return;

        GUILayout.Label("Playmaker References in Prefabs: " + referencingPrefabFSMs.Count, EditorStyles.boldLabel);

        // Define button style with fixed height
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedHeight = 28f;
        buttonStyle.alignment = TextAnchor.MiddleLeft;
        buttonStyle.padding.left = 10;

        // Define icon style to align vertically in the center
        GUIStyle iconStyle = new GUIStyle();
        iconStyle.margin.top = 6;

        foreach ((PlayMakerFSM fsm, FsmObject variable) in referencingPrefabFSMs)
        {
            if (((Object)variable.RawValue == targetScriptableObject))
            {
                GUIContent buttonContent = new GUIContent();
                buttonContent.text = $"{fsm.gameObject.name} \u27A1 {fsm.FsmName} \u27A1 {variable.Name}";

                GUILayout.BeginHorizontal();

                // Display icons
                Texture2D iconGO = EditorGUIUtility.IconContent("d_Prefab Icon").image as Texture2D;
                GUILayout.Label(iconGO, iconStyle, GUILayout.Width(20), GUILayout.Height(20));

                Texture2D iconFSM = EditorGUIUtility.IconContent("Assets/_Project/Fun2Play/Utilities/Editor Icons/PlaymakerIcon.png").image as Texture2D;
                GUILayout.Label(iconFSM, iconStyle, GUILayout.Width(20), GUILayout.Height(20));

                // Display button with all information
                if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.ExpandWidth(true)))
                {
                    EditorGUIUtility.PingObject(fsm.gameObject);
                }

                GUILayout.EndHorizontal();
            } 
        }
    }

    #endregion    
}
