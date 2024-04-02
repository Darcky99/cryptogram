using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBrain : Singleton<GameBrain>
{
    //private GameManager _GameManager => GameManager.Instance;
    //private StorageManager _StorageManager => StorageManager.Instance;

    //#region Unity
    //public override void Start()
    //{
    //    base.Start();
    //    initialize();
    //}
    //private void OnEnable()
    //{
    //    GameManager.OnResetLevel += onResetLevel;
    //    GameManager.OnLoadLevel += onLoadLevel;
    //    GameManager.OnGameOver += onGameOver;
    //    TimeManager.OnNewDay += onNewDay;
    //}
    //private void OnDisable()
    //{
    //    GameManager.OnResetLevel -= onResetLevel;
    //    GameManager.OnLoadLevel -= onLoadLevel;
    //    GameManager.OnGameOver -= onGameOver;
    //    TimeManager.OnNewDay -= onNewDay;
    //}
    //#endregion

    //#region Callbacks
    //private void onResetLevel()
    //{
    //    _StorageManager.DeleteLevelContinue();
    //}
    //private void onLoadLevel(ILevelData levelData)
    //{
    //    LifePanel.Instance.SetLifeCount(_Lifes);
    //    HintPanel.Instance.SetHintCount(_Hints);

    //    _RandomGenerator = new System.Random(_GameManager.LevelIndex);
    //}
    //private void onGameOver()
    //{
    //    expendLife();
    //}
    //private void onNewDay()
    //{
    //    for( ; _Lifes < 5; )
    //        earnLife();
    //}
    //#endregion

    //#region Random generator
    //public System.Random RandomGenerator => _RandomGenerator;

    //private System.Random _RandomGenerator;
    //#endregion

    //#region Lifes and Hints
    //public int Lifes => _Lifes;
    //public int Hints => _Hints;

    //[SerializeField] private int _Lifes, _Hints;

    //private void earnLife()
    //{
    //    _Lifes++;
    //    LifePanel.Instance.SetLifeCount(_Lifes);
    //}
    //private void expendLife()
    //{
    //    _Lifes--;
    //    LifePanel.Instance.SetLifeCount(_Lifes);
    //}

    //public void EarnHint()
    //{
    //    _Hints++;
    //    HintPanel.Instance.SetHintCount(_Hints);
    //}
    //public void ExpendHint()
    //{
    //    _Hints--;
    //    HintPanel.Instance.SetHintCount(_Hints);
    //}
    //#endregion

    //#region Level load
    //public int LevelsCount => _LevelsToLoad.Length;
    //public eLevelsCollection LevelsCollection => _LevelsCollection;

    //public ILevelData LevelByIndex => _LevelsToLoad[_GameManager.LevelIndex % LevelsCount];
    //public LevelProgress LevelProgress => _LevelProgress;

    //private ILevelData[] _LevelsToLoad;
    //private eLevelsCollection _LevelsCollection;
    //private LevelProgress _LevelProgress;

    //private void initialize()
    //{
    //    _StorageManager.LoadGameProgress();
    //    bool exist = _StorageManager.TryLoadLevelContinue(out _LevelProgress);

    //    if (exist)
    //        SelectLevels(_LevelProgress.ContinueLevelType);
    //    else
    //        SelectLevels(eLevelsCollection.HC);
    //}

    //public void SelectLevels(eLevelsCollection toLoad)
    //{
    //    _LevelsCollection = toLoad;
    //    switch (toLoad, Application.systemLanguage)
    //    {
    //        case (eLevelsCollection.HC, SystemLanguage.English):
    //            _LevelsToLoad = Resources.Load<LevelsData_Scriptable>("HC Levels/HC Levels - English.asset").Levels;
    //            break;
    //        case (eLevelsCollection.HC, SystemLanguage.Spanish):
    //            _LevelsToLoad = Resources.Load<LevelsData_Scriptable>("HC Levels/HC Levels - Spanish").Levels;
    //            break;

    //        case (eLevelsCollection.DailyChallenge, SystemLanguage.English):
    //            break;

    //        case (eLevelsCollection.OnlineTest, SystemLanguage.English):
    //            _LevelsToLoad = RemoteLoad.GetTestLevels();
    //            break;
    //        case (eLevelsCollection.OnlineTest, SystemLanguage.Spanish):
    //            _LevelsToLoad = RemoteLoad.GetTestLevels();
    //            break;
    //    }
    //    _GameManager.SetLevelIndex(_StorageManager.GetLevelIndex(LevelsCollection));
    //    _GameManager.LoadLevel();
    //}
    //#endregion
}