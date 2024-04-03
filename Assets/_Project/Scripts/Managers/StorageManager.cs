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

    private bool _SaveExists => ES3.KeyExists(_GAME_PROGRESS_DICTIONARY) || ES3.KeyExists(_LEVEL_CONTINUE) || ES3.KeyExists(_DAY_LIFES) || ES3.KeyExists(_DAY_HINTS);

    private Dictionary<eLevelsCollection, int> _LevelsIndex;
    private LevelProgress _LevelProgress;

    private const string _GAME_PROGRESS_DICTIONARY = "GP_GAME_DICTIONARY";
    private const string _LEVEL_CONTINUE = "GP_LVLCONT";
    private const string _DAY_LIFES = "GP_DAYLIFES";
    private const string _DAY_HINTS = "GP_HINTS";

    private int getLevelIndex(eLevelsCollection levelsCollection)
    {
        if (_LevelsIndex.ContainsKey(levelsCollection))
            return _LevelsIndex[levelsCollection];
        return 0;
    }

    private void saveGameProgress()
    {
        eLevelsCollection currentCollection = _GameManager.LevelsCollection;
        int currentIndex = _GameManager.LevelIndex;
        if (!_LevelsIndex.ContainsKey(currentCollection))
            _LevelsIndex.Add(currentCollection, currentIndex);
        _LevelsIndex[currentCollection] = currentIndex;
        ES3.Save(_GAME_PROGRESS_DICTIONARY, _LevelsIndex);
        ES3.Save(_DAY_LIFES, _GameManager.Lifes);
        ES3.Save(_DAY_HINTS, _GameManager.Hints);
        ES3.DeleteKey(_LEVEL_CONTINUE);
    }
    private void saveLevelContinue(bool[] progress, int mistakeCount)
    {
        eLevelsCollection currentCollection = _GameManager.LevelsCollection;
        int currentIndex = _GameManager.LevelIndex;
        _LevelProgress = new LevelProgress(currentCollection, currentIndex, progress, mistakeCount);
        ES3.Save(_LEVEL_CONTINUE, _LevelProgress);
    }

    private void loadGameProgress()
    {
        bool keyExist = _SaveExists;

        if (keyExist)
        {
            _LevelsIndex = ES3.Load<Dictionary<eLevelsCollection, int>>(_GAME_PROGRESS_DICTIONARY);
            _GameManager.LoadLifeAndHints(ES3.Load<int>(_DAY_LIFES), ES3.Load<int>(_DAY_HINTS));
        }
        else
            _LevelsIndex = new Dictionary<eLevelsCollection, int>();
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
    }

    public int GetLevelIndex(eLevelsCollection levelsCollection) => getLevelIndex(levelsCollection);

    public void SaveGameProgress() => saveGameProgress();
    public void SaveLevelContinue(bool[] progress, int mistakeCount) => saveLevelContinue(progress, mistakeCount);

    //public void LoadGameProgress() => loadGameProgress();
    public bool TryLoadLevelContinue(out LevelProgress levelProgress) => tryLoadLevelContinue(out levelProgress);

    public void DeleteLevelContinue() => deleteLevelContinue();
    public void DeleteAll() => deleteAll();
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