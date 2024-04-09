#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class Fun2PlayLogoDrawer
{
    public static void DrawLogo()
    {
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        Texture icon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Fun2Play/Utilities/Editor Icons/F2PIcon.png", typeof(Texture));
        GUILayout.Label(icon, GUILayout.Width(74), GUILayout.Height(24));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5);
    }
}
#endif