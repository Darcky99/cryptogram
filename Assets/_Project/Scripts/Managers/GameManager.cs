using System;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    private StorageManager _StorageManager => StorageManager.Instance;
    private RemoteDataManager _RemoteDataManager => RemoteDataManager.Instance;

    public static event Action<ILevelData> OnLoadLevel;
    public static event Action OnResetLevel;
    public static event Action OnLevelCompleted;
    public static event Action OnGameOver;

    #region Unity
    protected override void OnAwakeEvent()
    {
        base.OnAwakeEvent();
    }
    public override void Start() 
    {
        //continueLevel();
    }
    private void OnEnable()
    {
        InputManager.OnKeyDown += onKeyDown;

        OnResetLevel += onResetLevel;
        OnLoadLevel += onLoadLevel;
        OnGameOver += onGameOver;
        OnLevelCompleted += onLevelCompleted;

        TimeManager.OnNewDay += onNewDay;
    }
    private void OnDisable()
    {
        InputManager.OnKeyDown -= onKeyDown;

        OnResetLevel += onResetLevel;
        OnLoadLevel += onLoadLevel;
        OnGameOver += onGameOver;
        OnLevelCompleted += onLevelCompleted;

        TimeManager.OnNewDay -= onNewDay;
    }
    #endregion

    #region Callbacks
    private void onKeyDown(KeyCode keyCode)
    {
        //This behaviour changes depending on the gameState.

        //If we are on playing, _LevelIndex++
        //If we  are on levelCompleted, go _LevelIndex -= 2;

        switch (keyCode)
        {
            case KeyCode.R:
                ResetLevel();
                break;
            case KeyCode.Q:
                Debug.Log("Not implemented!");
                //eGameState required
                //LoadLevel();
                break;
            case KeyCode.E:
                Debug.Log("Not implemented!");
                //eGameState required
                //LoadLevel();
                break;
        }
    }

    private void onResetLevel()
    {
        _StorageManager.DeleteLevelContinue();
    }
    private void onLoadLevel(ILevelData levelData)
    {
        _RandomGenerator = new System.Random(LevelIndex);
    }
    private void onLevelCompleted()
    {
        _LevelIndex++;
        _StorageManager.SaveGameProgress();
    }
    private void onGameOver()
    {
        expendLife();
    }
    private void onNewDay()
    {
        for (; _Lifes < 5;)
            earnLife();
    }
    #endregion

    #region Pause & Resume
    private void pause() => Time.timeScale = 0;
    private void resume() => Time.timeScale = 1;
    #endregion

    #region Random generator
    public System.Random RandomGenerator => _RandomGenerator;

    private System.Random _RandomGenerator;
    #endregion

    #region Lifes and Hints
    public int Lifes => _Lifes;
    public int Hints => _Hints;

    private int _Lifes = 5, _Hints = 3;

    private void earnLife()
    {
        _Lifes++;
        LifePanel.Instance.SetLifeCount(_Lifes);
    }
    private void expendLife()
    {
        _Lifes--;
        LifePanel.Instance.SetLifeCount(_Lifes);
    }

    public void EarnHint()
    {
        _Hints++;
        HintPanel.Instance.SetHintCount(_Hints);
    }
    public void ExpendHint()
    {
        _Hints--;
        HintPanel.Instance.SetHintCount(_Hints);
    }

    public void LoadLifeAndHints(int lifes, int hints)
    {
        _Lifes = lifes;
        _Hints = hints;
    }
    #endregion

    #region Game loop
    public int LevelIndex => _LevelIndex;

    private int _LevelIndex = 0;

    public void ResetLevel()
    {
        OnResetLevel?.Invoke();
        LoadLevel();
    }
    public void LoadLevel() => LoadLevel(LevelByIndex);
    public void LoadLevel(ILevelData levelData) => OnLoadLevel?.Invoke(levelData);
    public void LevelCompleted() => OnLevelCompleted?.Invoke();
    public void GameOver() => OnGameOver?.Invoke();

    public void SetLevelIndex(int levelIndex)
    {
        _LevelIndex = levelIndex;
    }
    #endregion

    #region Load levels
    public int LevelsCount => _LevelsToLoad.Length;
    public eLevelsCollection LevelsCollection => _LevelsCollection;

    public ILevelData LevelByIndex => _LevelsToLoad[LevelIndex % LevelsCount];
    public LevelProgress LevelProgress => _LevelProgress;

    private ILevelData[] _LevelsToLoad;
    private eLevelsCollection _LevelsCollection;
    private LevelProgress _LevelProgress;

    //private void continueLevel()
    //{
    //    //THIS SHOULD PROBABLY BE CONTROLLED FROM THE MAIN MENU.
    //    //IN A FUTURE, WE WILL SKIP THE MAIN_MENU, AND GO STRAIGH INTO THE LEVEL THAT WE LEFT UNCOMPLETED...

    //    bool exist = _StorageManager.TryLoadLevelContinue(out _LevelProgress);
    //    if (exist)
    //        SelectLevels(_LevelProgress.ContinueLevelType);
    //    else
    //        SelectLevels(eLevelsCollection.HC);
    //}

    public void PlayHCLevels()
    {
        switch (Application.systemLanguage)
        {
            default:
                _LevelsToLoad = Resources.Load<LevelsData_Scriptable>("HC Levels/HC Levels - English.asset").Levels;
                break;
            case SystemLanguage.Spanish:
                _LevelsToLoad = Resources.Load<LevelsData_Scriptable>("HC Levels/HC Levels - Spanish").Levels;
                break;
        }
        SetLevelIndex(_StorageManager.GetLevelIndex(LevelsCollection));
        LoadLevel();
    }
    public async void PLayDailyChallenge(int month, int levelIndex)
    {
        _LevelsToLoad = await _RemoteDataManager.GetDailyChallengeLevels(month);
        SetLevelIndex(levelIndex);
        LoadLevel();
    }
    #endregion
}