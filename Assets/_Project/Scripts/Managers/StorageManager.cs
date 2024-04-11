using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class StorageManager : Singleton<StorageManager>
{
    private GameManager _GameManager => GameManager.Instance;

    #region Unity
    protected override void OnAwakeEvent()
    {
        base.OnAwakeEvent();
    }
    #endregion

    public string HC_PROGRESS => _HC_PROGRESS;
    public string DC_PROGRESS => _DC_PROGRESS;
    public string TH_PROGRESS => _TH_PROGRESS;

    private const string _DAY_LIFES = "GP_DAYLIFES";
    private const string _DAY_HINTS = "GP_HINTS";

    private const string _HC_PROGRESS = "GP_HC-PROGRESS";
    private const string _DC_PROGRESS = "GP_DC-PROGRESS";
    private const string _TH_PROGRESS = "GP_TH-PROGRESS";

    private void save(string key, object value) => ES3.Save(key, value);
    private T load<T>(string key) => ES3.Load<T>(key);

    public void Save(string key, object value) => save(key, value);
    public T Load<T>(string key) => load<T>(key);

    private void deleteAll()
    {
        ES3.DeleteKey(_DAY_LIFES);
        ES3.DeleteKey(_DAY_HINTS);
        ES3.DeleteKey(_HC_PROGRESS);
        ES3.DeleteKey(_DC_PROGRESS);
        ES3.DeleteKey(_TH_PROGRESS);
    }
    public void DeleteAll() => deleteAll();
}

public class ItemProgress
{
    public ItemProgress(bool isComplete, LevelContinue levelContinue = null)
    {
        _LevelContinue = levelContinue;
        _IsCompleted = isComplete;
    }
    public LevelContinue LevelContinue => _LevelContinue;
    public bool IsCompleted => _IsCompleted;

    private LevelContinue _LevelContinue;
    private bool _IsCompleted = false;
}
public class LevelContinue
{
    public LevelContinue(bool[] progress, int mistakeCount)
    {
        _Progress = progress;
        _MistakeCount = mistakeCount;
    }
    public bool[] Progress => _Progress;
    public int MistakeCount => _MistakeCount;

    private bool[] _Progress;
    private int _MistakeCount;
}

[CustomEditor(typeof(StorageManager))]
public class StorageManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        StorageManager storageManager = (StorageManager)target;
        GUILayout.Space(10);
        if (GUILayout.Button("Delete data"))
            storageManager.DeleteAll();
    }
}