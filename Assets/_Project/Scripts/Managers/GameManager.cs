using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GameManager : Singleton<GameManager>
{
    private StorageManager _StorageManager => StorageManager.Instance;

    public static event Action<ILevelData> OnLoadLevel;
    public static event Action OnResetLevel;
    public static event Action OnLevelCompleted;
    public static event Action OnGameOver;

    #region Unity
    protected override void OnAwakeEvent()
    {
        base.OnAwakeEvent();

        //for now it's a good structure, I might move the data loading to their own methods / moments.
        _HC_Levels_Progress = new ContinousProgress();
        setMonth(DateTime.Today.Month);

        _GameMode = eGameMode.None;
    }
    public override void Start() 
    {
        load();

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
        LoadLevel();
    }
    private void onLoadLevel(ILevelData levelData)
    {
        _GameState = eGameState.Playing;
    }
    private void onLevelCompleted()
    {
        _GameState = eGameState.Win;
        registerProgress();
    }
    private void onGameOver()
    {
        _GameState = eGameState.GameOver;
        expendLife();
    }
    private void onNewDay()
    {
        for ( ; _Lifes < 5; )
            earnLife();
    }
    #endregion

    #region Game loop
    public eGameState GameState => _GameState;
    private eGameState _GameState;

    private void loadLevel(ILevelData levelData) => OnLoadLevel?.Invoke(levelData);

    public void ResetLevel()
    {
        OnResetLevel?.Invoke();
    }
    public void LevelCompleted() => OnLevelCompleted?.Invoke();
    public void LoadLevel()
    {
        switch (_GameMode)
        {
            default:
                Debug.LogError("Not valid gamemode");
                break;
            case eGameMode.HC:
                playHC();
                break;
        }
    }
    public void GameOver() => OnGameOver?.Invoke();
    #endregion

    #region Random generator
    public System.Random RandomGenerator => _RandomGenerator;
    private System.Random _RandomGenerator;

    private void createRandomizer(int seed) => _RandomGenerator = new System.Random(seed);

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

    #region Game modes

    private eGameMode _GameMode;

    private void registerProgress()
    {
        switch (_GameMode)
        {
            default:
            Debug.LogError("Not valid gamemode");
                break;
            case eGameMode.HC:
                _HC_Levels_Progress.LevelIndex++;
                break;
        }
        save();
    }

    #region HC LEVELS

    public int HC_Index => _HC_Levels_Progress.LevelIndex;

    private ContinousProgress _HC_Levels_Progress;

    private void playHC()
    {
        _GameMode = eGameMode.HC;
        string file;
        switch (Application.systemLanguage)
        {
            default:
                file = File.ReadAllText("Assets/_Project/Documents/JSON LEVELS/HC/HC Levels - English.json");
                break;
            case SystemLanguage.Spanish:
                file = File.ReadAllText("Assets/_Project/Documents/JSON LEVELS/HC/HC Levels - Spanish.json");
                break;
        }
        LevelData.JSON collection = JsonUtility.FromJson<LevelData.JSON>(file);
        LevelData levelToPlay = collection.Levels[_HC_Levels_Progress.LevelIndex % collection.Levels.Length];

        createRandomizer(_HC_Levels_Progress.LevelIndex);
        loadLevel(levelToPlay);
    }

    public void PlayHC() => playHC();

    #endregion

    #region DC LEVELS

    private CollectionProgress _DC_Levels_Progress;
    private int _Month;
    private int _LevelIndex;

    private void setMonth(int month)
    {
        _Month = month;
        // here it should check if there's any save.... so we load here..?


        _DC_Levels_Progress = new CollectionProgress(DateTime.DaysInMonth(DateTime.Today.Year, month));


    }
    public void playDH(int levelIndex)
    {
        _GameMode = eGameMode.DC;

        string file;
        _LevelIndex = levelIndex;

        switch (Application.systemLanguage)
        {
            default:
                //file = File.ReadAllText("Assets/_Project/Documents/JSON LEVELS/DC/DC Levels - English.json");
                //break;
            case SystemLanguage.Spanish:
                file = File.ReadAllText("Assets/_Project/Documents/JSON LEVELS/DC/April Levels - Spanish.json");
                break;
        }
        LevelData.JSON collection = JsonUtility.FromJson<LevelData.JSON>(file);
        LevelData levelToPlay = collection.Levels[levelIndex];

        createRandomizer(levelIndex + DateTime.Today.Year);
        loadLevel(levelToPlay);
    }

    public void PlayDH(int levelIndex) => playDH(levelIndex);

    #endregion

    #region THEME LEVELS
    //I'll need my data and progress here

    public void PlayThemeLevels()
    {
        //Get the the levels pack
        //Load the last one
        //Continue
    }
    #endregion

    #endregion

    #region Save and Load

    private const string HC_PROGRESS = "GP_HC-PROGRESS";
    private const string DC_PROGRESS = "GP_HC-PROGRESS";

    private void save()
    {
        _StorageManager.Save(HC_PROGRESS, _HC_Levels_Progress);
        _StorageManager.Save($"{DC_PROGRESS}-{_Month}", _DC_Levels_Progress);
    }
    private void load()
    {
        if(ES3.KeyExists(HC_PROGRESS))
            _HC_Levels_Progress = _StorageManager.Load<ContinousProgress>(HC_PROGRESS);
    }

    #endregion
}