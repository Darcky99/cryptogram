using UnityEditor;
using UnityEngine;

public class SwitchAppNameAndIcon : EditorWindow
{
    string iOSAppName;
    Texture2D iOSAppIcon;

    string androidAppName;
    Texture2D androidAppIcon;

    const string iOSAppNameKey = "IOS_APP_NAME";
    const string iOSAppIconKey = "IOS_APP_ICON";

    const string androidAppNameKey = "ANDROID_APP_NAME";
    const string androidAppIconKey = "ANDROID_APP_ICON";

    [MenuItem("Fun2Play/Tools/Switch App Name & Icon")]
    public static void ShowWindow()
    {
        GetWindow<SwitchAppNameAndIcon>("Switch App Name & Icon");
    }

    void OnEnable()
    {
        // Load settings from EditorPrefs
        iOSAppName = EditorPrefs.GetString(iOSAppNameKey, "iOSApp");
        iOSAppIcon = LoadTextureFromPrefs(iOSAppIconKey);

        androidAppName = EditorPrefs.GetString(androidAppNameKey, "AndroidApp");
        androidAppIcon = LoadTextureFromPrefs(androidAppIconKey);
    }

    void OnDisable()
    {
        // Save settings to EditorPrefs when the window is closed
        EditorPrefs.SetString(iOSAppNameKey, iOSAppName);
        SaveTextureToPrefs(iOSAppIconKey, iOSAppIcon);

        EditorPrefs.SetString(androidAppNameKey, androidAppName);
        SaveTextureToPrefs(androidAppIconKey, androidAppIcon);
    }

    void OnGUI()
    {
        // GUILayout.Label("Switch Options", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("iOS Settings", EditorStyles.boldLabel);
        iOSAppName = EditorGUILayout.TextField("App Name iOS", iOSAppName);
        iOSAppIcon = (Texture2D)EditorGUILayout.ObjectField("Icon iOS", iOSAppIcon, typeof(Texture2D), false);

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Android Settings", EditorStyles.boldLabel);
        androidAppName = EditorGUILayout.TextField("App Name Android", androidAppName);
        androidAppIcon = (Texture2D)EditorGUILayout.ObjectField("Icon Android", androidAppIcon, typeof(Texture2D), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Switch to iOS"))
        {
            SwitchToiOS();
        }

        if (GUILayout.Button("Switch to Android"))
        {
            SwitchToAndroid();
        }
    }

    void SwitchToiOS()
    {
        SetPlatformSettings(iOSAppName, iOSAppIcon);
        Debug.Log("Switched to iOS");
    }

    void SwitchToAndroid()
    {
        SetPlatformSettings(androidAppName, androidAppIcon);
        Debug.Log("Switched to Android");
    }

    void SetPlatformSettings(string appName, Texture2D icon)
    {
        // Set icons for both platforms (unknown platform here)
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { icon });
        // Set product name
        PlayerSettings.productName = appName;
    }

    Texture2D LoadTextureFromPrefs(string key)
    {
        string texturePath = EditorPrefs.GetString(key, string.Empty);
        if (!string.IsNullOrEmpty(texturePath))
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        }
        return null;
    }

    void SaveTextureToPrefs(string key, Texture2D texture)
    {
        if (texture != null)
        {
            string texturePath = AssetDatabase.GetAssetPath(texture);
            EditorPrefs.SetString(key, texturePath);
        }
        else
        {
            EditorPrefs.SetString(key, string.Empty);
        }
    }
}
