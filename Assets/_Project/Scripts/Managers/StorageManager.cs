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
        loadGameProgress();
    }
    #endregion

    

    private bool _GameSaveExists => ES3.KeyExists(_GAME_PROGRESS_DICTIONARY) || ES3.KeyExists(_DAY_LIFES) || ES3.KeyExists(_DAY_HINTS);
    private bool _LevelContinueSaveExists => ES3.KeyExists(_LEVEL_CONTINUE);

    
    //private CollectionProgress _DC_Levels_Progress;

    private const string _GAME_PROGRESS_DICTIONARY = "GP_GAME_DICTIONARY";
    private const string _LEVEL_CONTINUE = "GP_LVLCONT";
    private const string _DAY_LIFES = "GP_DAYLIFES";
    private const string _DAY_HINTS = "GP_HINTS";


    private void save(string key, object value) => ES3.Save(key, value);
    private T load<T>(string key) => ES3.Load<T>(key);

    public void Save(string key, object value) => save(key, value);
    public T Load<T>(string key) => load<T>(key);



    //private void saveHCProgress()
    //{
    //    if(_HC_Levels_Progress == null)
    //    {
    //        _HC_Levels_Progress = new ContinousProgress();
    //        return;
    //    }

    //}

    //public void SaveHCProgress() => saveHCProgress();

    //private int getLevelIndex(eLevelsCollection levelsCollection)
    //{
    //    if (_LevelsIndex.ContainsKey(levelsCollection))
    //        return _LevelsIndex[levelsCollection];
    //    return 0;
    //}

    //private void saveGameProgress()
    //{
    //    eLevelsCollection currentCollection = _GameManager.LevelsCollection;
    //    int currentIndex = _GameManager.LevelIndex;
    //    if (!_LevelsIndex.ContainsKey(currentCollection))
    //        _LevelsIndex.Add(currentCollection, currentIndex);
    //    else if(_LevelsIndex[currentCollection] < currentIndex)
    //        _LevelsIndex[currentCollection] = currentIndex;

    //    ES3.Save(_GAME_PROGRESS_DICTIONARY, _LevelsIndex);
    //    ES3.Save(_DAY_LIFES, _GameManager.Lifes);
    //    ES3.Save(_DAY_HINTS, _GameManager.Hints);
    //    ES3.DeleteKey(_LEVEL_CONTINUE);
    //}
    //private void saveLevelContinue(bool[] progress, int mistakeCount)
    //{
    //    eLevelsCollection currentCollection = _GameManager.LevelsCollection;
    //    int currentIndex = _GameManager.LevelIndex;
    //    _LevelProgress = new LevelProgress(currentCollection, currentIndex, progress, mistakeCount);
    //    ES3.Save(_LEVEL_CONTINUE, _LevelProgress);
    //}

    private void loadGameProgress()
    {
        bool keyExist = _GameSaveExists;
        if (keyExist)
        {
            //_LevelsIndex = ES3.Load<Dictionary<eLevelsCollection, int>>(_GAME_PROGRESS_DICTIONARY);
            _GameManager.LoadLifeAndHints(ES3.Load<int>(_DAY_LIFES), ES3.Load<int>(_DAY_HINTS));
        }
        else
        {
            //_LevelsIndex = new Dictionary<eLevelsCollection, int>();
            //_GameManager.LoadLifeAndHints() NOT required. Default values are: 5, 3
        }
    }
    private bool tryLoadLevelContinue(out LevelProgress levelProgress)
    {
        levelProgress = null;

        bool keyExist = ES3.KeyExists(_LEVEL_CONTINUE);
        if(keyExist)
            levelProgress = ES3.Load<LevelProgress>(_LEVEL_CONTINUE);
        return keyExist;
    }

    private void deleteLevelContinue() => ES3.DeleteKey(_LEVEL_CONTINUE);
    private void deleteAll()
    {
        ES3.DeleteKey(_GAME_PROGRESS_DICTIONARY);
        ES3.DeleteKey(_LEVEL_CONTINUE);
        ES3.DeleteKey(_DAY_LIFES);
        ES3.DeleteKey(_DAY_HINTS);
    }

    //public int GetLevelIndex(eLevelsCollection levelsCollection) => getLevelIndex(levelsCollection);

    //public void SaveGameProgress() => saveGameProgress();
    //public void SaveLevelContinue(bool[] progress, int mistakeCount) => saveLevelContinue(progress, mistakeCount);

    //public void LoadGameProgress() => loadGameProgress();
    public bool TryLoadLevelContinue(out LevelProgress levelProgress) => tryLoadLevelContinue(out levelProgress);

    public void DeleteLevelContinue() => deleteLevelContinue();
    public void DeleteAll() => deleteAll();
}



public class ContinousProgress
{
    public int LevelIndex;
    public LevelContinue LevelContinue;
}
public class CollectionProgress
{
    public CollectionProgress(int size)
    {
        Items = new ItemProgress[size];
    }

    public ItemProgress[] Items;

    public void RegisterProgress(int index, ItemProgress itemProgress)
    {
        Items[index] = itemProgress;
    }
}

public class ItemProgress
{
    public ItemProgress(LevelContinue levelContinue, bool isComplete)
    {

    }

    public LevelContinue LevelContinue;
    public bool IsCompleted = false;
}
public class LevelContinue
{
    public bool[] Progress;
    public int MistakeCount;
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