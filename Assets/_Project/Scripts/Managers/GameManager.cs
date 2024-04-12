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
        _GameMode = eGameMode.None;
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
    public override void Start() 
    {
        load();
        //continueLevel();
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
            case eGameMode.HC:
                playHC();
                break;
            case eGameMode.TH:
                playThemeLevels(_CurrentTheme);
                break;
            default:
                Debug.LogError("Not valid gamemode");
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

    #region Gamemode & Progress

    public eGameMode GameMode => _GameMode;

    private eGameMode _GameMode;

    private const string _ENGLISH = "English";
    private const string _SPANISH = "Spanish";

    private void registerLevelProgress(bool[] levelProgress, int mistakeCount)
    {
        LevelContinue levelContinue = new LevelContinue(levelProgress, mistakeCount);

        switch (_GameMode)
        {
            case eGameMode.HC:
                _HC_Levels_Progress.LevelContinue = levelContinue;
                _StorageManager.Save(_StorageManager.HC_PROGRESS, _HC_Levels_Progress);
                break;
            case eGameMode.DC:
                _DC_CurrentDay.LevelContinue = levelContinue;
                _StorageManager.Save(_StorageManager.DC_PROGRESS, _DC_Levels_Progress);
                break;
            case eGameMode.TH:
                _TH_Current.LevelContinue = levelContinue;
                _StorageManager.Save(_StorageManager.TH_PROGRESS, _TH_Levels_Progress);
                break;
        }
    }
    private void registerProgress()
    {
        switch (_GameMode)
        {
            case eGameMode.HC:
                _HC_Levels_Progress.IncreaseLevelIndex();
                _StorageManager.Save(_StorageManager.HC_PROGRESS, _HC_Levels_Progress);
                break;
            case eGameMode.DC:
                _DC_Levels_Progress[_Month].RegisterProgress(_DC_Index, new ItemProgress(true));
                _StorageManager.Save(_StorageManager.DC_PROGRESS, _DC_Levels_Progress);
                break;
            case eGameMode.TH:
                _TH_Current.IncreaseLevelIndex();
                _StorageManager.Save(_StorageManager.TH_PROGRESS, _TH_Levels_Progress);
                break;

            default:
                Debug.LogError("Not valid gamemode");
                break;
        }
    }
    private string filePath(string path, string prefix, string languaje, string specific = "")
    {
        return specific == "" ? 
            $"{path}{prefix}-{languaje}.json" : 
            $"{path}{specific}/{prefix}-{languaje}.json";
    }
    private LevelContinue levelContinue()
    {
        LevelContinue levelContinue = null;

        switch (_GameMode)
        {
            case eGameMode.HC:
                levelContinue = HC_Levels_Progress.LevelContinue;
                break;
            case eGameMode.DC:
                levelContinue = DC_Levels_Progress[_Month].Items[_DC_Index].LevelContinue;
                break;
            case eGameMode.TH:
                levelContinue = TH_Levels_Progress[_CurrentTheme].LevelContinue;
                break;
        }

        return levelContinue;
    }

    public void RegisterLevelProgress(bool[] levelProgress, int mistakeCount) => registerLevelProgress(levelProgress, mistakeCount);
    public LevelContinue LevelContinue() => levelContinue();

    #region HC LEVELS

    public ContinousProgress HC_Levels_Progress => _HC_Levels_Progress;
    public int HC_Index => _HC_Levels_Progress.LevelIndex;

    private ContinousProgress _HC_Levels_Progress;

    private const string _HC_PREFIX = "HC";
    private const string _HC_PATH = "Assets/_Project/Documents/JSON LEVELS/HC/";

    private void playHC()
    {
        _GameMode = eGameMode.HC;
        string file;
        string path;
        switch (Application.systemLanguage)
        {
            default:
                path = filePath(_HC_PATH, _HC_PREFIX, _ENGLISH);
                file = File.ReadAllText(path);
                break;
            case SystemLanguage.Spanish:
                path = filePath(_HC_PATH, _HC_PREFIX, _ENGLISH);
                file = File.ReadAllText(path);
                break;
        }
        LevelData[] collection = JsonUtility.FromJson<JSONWrapper<LevelData>>(file).Array;
        LevelData levelToPlay = collection[_HC_Levels_Progress.LevelIndex % collection.Length];

        createRandomizer(_HC_Levels_Progress.LevelIndex);
        loadLevel(levelToPlay);

        if (levelToPlay.BossLevel)
            BossWarning.Instance.TriggerBossWarning();
    }

    public void PlayHC() => playHC();

    #endregion

    #region DC LEVELS

    public Dictionary<int, CollectionProgress> DC_Levels_Progress => _DC_Levels_Progress;

    private ItemProgress _DC_CurrentDay => _DC_Levels_Progress[_Month].Items[_DC_Index];

    private int _DC_Index;
    private int _Month;
    private Dictionary<int, CollectionProgress> _DC_Levels_Progress;

    private const string _DC_PREFIX = "DC";
    private const string _DC_PATH = "Assets/_Project/Documents/JSON LEVELS/DC/";

    private void setMonth(int month)
    {
        _Month = month;
        loadDCProgress();
    }
    public void playDH(int levelIndex)
    {
        _GameMode = eGameMode.DC;

        string file;
        string path;
        _DC_Index = levelIndex;

        switch (Application.systemLanguage)
        {
            default:
                //file = File.ReadAllText("Assets/_Project/Documents/JSON LEVELS/DC/DC Levels - English.json");
                //break;
            case SystemLanguage.Spanish:
                path = filePath(_DC_PATH, _DC_PREFIX, _SPANISH, $"{_Month}");
                file = File.ReadAllText(path);
                break;
        }
        LevelData[] collection = JsonUtility.FromJson<JSONWrapper<LevelData>>(file).Array;
        LevelData levelToPlay = collection[levelIndex];

        createRandomizer(levelIndex + DateTime.Today.Year);
        loadLevel(levelToPlay);
    }

    public void PlayDH(int levelIndex) => playDH(levelIndex);
    public void SetMonth(int month) => setMonth(month);

    #endregion

    #region THEME LEVELS

    public string[] Themes 
    {
        get
        {
            int i = 0;
            string[] themes = new string[_TH_Levels_Progress.Keys.Count];

            foreach(string value in _TH_Levels_Progress.Keys)
            {
                themes[i] = value;
                i++;
            }
            return themes;
        } 
    }
    public Dictionary<string, ContinousProgress> TH_Levels_Progress => _TH_Levels_Progress;

    private ContinousProgress _TH_Current => _TH_Levels_Progress[_CurrentTheme];

    //private int _TH_Index;
    private string _CurrentTheme;

    private Dictionary<string, ContinousProgress> _TH_Levels_Progress;

    private const string _THEMES_PATH = "Assets/_Project/Documents/JSON LEVELS/TH/";
    private const string _THEMES_PREFIX = "TH";

    private void playThemeLevels(string theme)
    {
        _GameMode = eGameMode.TH;
        _CurrentTheme = theme;
        //_TH_Index = _TH_Levels_Progress[_CurrentTheme].LevelIndex;

        LevelData[] collection = getThemeLevels(theme);
        int levelIndex = _TH_Levels_Progress[theme].LevelIndex;
        LevelData levelToPlay = collection[levelIndex];

        createRandomizer(levelIndex + levelToPlay.Phrase.Length + theme.Length);
        loadLevel(levelToPlay);
    }

    private bool isThemeCompleted(string theme) => _TH_Levels_Progress[theme].LevelIndex >= getThemeLevelsCount(theme) ;
    private int getThemeLevelsCount(string theme) => getThemeLevels(theme).Length;
    private LevelData[] getThemeLevels(string theme)
    {
        if (_TH_Levels_Progress.ContainsKey(theme) == false)
            Debug.LogError("Theme does not exist!");

        string path = filePath(_THEMES_PATH, _THEMES_PREFIX, _SPANISH, theme);
        string file = File.ReadAllText(path);
        LevelData[] levels = JsonUtility.FromJson<JSONWrapper<LevelData>>(file).Array;

        return levels;
    }
    private string[] getAllThemeTitles()
    {
        string[] directories = Directory.GetDirectories(_THEMES_PATH);
        string[] directoryNames = new string[directories.Length];

        for (int i = 0; i < directories.Length; i++)
        {
            string[] folderDirectories = directories[i].Split('/');
            string name = folderDirectories[folderDirectories.Length - 1];

            directoryNames[i] = name;

            //I'll need this to get the languajes
            //foreach (string file in Directory.GetFiles(directories[i]))
            //{
            //    string[] fileDirectoryNames = file.Split('\\');
            //    debug += $" {fileDirectoryNames[fileDirectoryNames.Length - 1]}, ";
            //}
        }
        return directoryNames;
    }

    public bool IsThemeCompleted() => isThemeCompleted(_CurrentTheme);
    public int GetThemeLevelsCount(string theme) => getThemeLevelsCount(theme);
    public void PlayThemeLevels(string theme) => playThemeLevels(theme);

    #endregion

    #endregion

    #region Save and Load

    private void load()
    {
        loadHCProgress();
        loadTHProgress();
    }

    private void loadHCProgress()
    {
        if (ES3.KeyExists(_StorageManager.HC_PROGRESS))
        {
            _HC_Levels_Progress = _StorageManager.Load<ContinousProgress>(_StorageManager.HC_PROGRESS);
            return;
        }
        _HC_Levels_Progress = new ContinousProgress(0, null);
    }
    private void loadDCProgress()
    {
        if (ES3.KeyExists(_StorageManager.DC_PROGRESS))
        {
            _DC_Levels_Progress = _StorageManager.Load<Dictionary<int, CollectionProgress>>(_StorageManager.DC_PROGRESS);
            return;
        }
        _DC_Levels_Progress = new Dictionary<int, CollectionProgress>();

        for(int i = 1; i <= 12; i++)
            _DC_Levels_Progress.Add(i, new CollectionProgress(DateTime.DaysInMonth(DateTime.Today.Year, i)));
    }
    private void loadTHProgress()
    {
        if (ES3.KeyExists(_StorageManager.TH_PROGRESS))
            _TH_Levels_Progress = _StorageManager.Load<Dictionary<string, ContinousProgress>>(_StorageManager.TH_PROGRESS);
        else
            _TH_Levels_Progress = new Dictionary<string, ContinousProgress>();
        
        string[] themes = getAllThemeTitles();

        for(int i = 0; i < themes.Length; i++)
            if(!_TH_Levels_Progress.ContainsKey(themes[i]))
                _TH_Levels_Progress.Add(themes[i], new ContinousProgress(0, null));
    }
    #endregion
}