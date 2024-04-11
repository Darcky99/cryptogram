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
        //_HC_Levels_Progress = new ContinousProgress();
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
                _HC_Levels_Progress.IncreaseLevelIndex();
                _StorageManager.Save(HC_PROGRESS, _HC_Levels_Progress);
                break;
            case eGameMode.DC:
                _DC_Levels_Progress.RegisterProgress(_DC_Index, new ItemProgress(true));
                _StorageManager.Save(dcKey(_Month), _DC_Levels_Progress);
                break;
        }
    }

    #region HC LEVELS

    public int HC_Index => _HC_Levels_Progress.LevelIndex;

    private ContinousProgress _HC_Levels_Progress;

    private const string _HC_PATH = "Assets/_Project/Documents/JSON LEVELS/HC/";

    private void playHC()
    {
        _GameMode = eGameMode.HC;
        string file;
        switch (Application.systemLanguage)
        {
            default:
                file = File.ReadAllText(_HC_PATH + "HC Levels - English.json");
                break;
            case SystemLanguage.Spanish:
                file = File.ReadAllText(_HC_PATH + "HC Levels - Spanish.json");
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

    private int _DC_Index;
    private int _Month;
    private CollectionProgress _DC_Levels_Progress;

    private const string _DC_PATH = "Assets/_Project/Documents/JSON LEVELS/DC/";

    private void setMonth(int month)
    {
        _Month = month;
        if (!ES3.KeyExists(dcKey(month)))
        {
            _DC_Levels_Progress = new CollectionProgress(DateTime.DaysInMonth(DateTime.Today.Year, month));
            return;
        }
        _DC_Levels_Progress = _StorageManager.Load<CollectionProgress>(dcKey(month));
    }
    public void playDH(int levelIndex)
    {
        _GameMode = eGameMode.DC;

        string file;
        _DC_Index = levelIndex;

        switch (Application.systemLanguage)
        {
            default:
                //file = File.ReadAllText("Assets/_Project/Documents/JSON LEVELS/DC/DC Levels - English.json");
                //break;
            case SystemLanguage.Spanish:
                file = File.ReadAllText(_DC_PATH + "April Levels - Spanish.json");
                break;
        }
        LevelData.JSON collection = JsonUtility.FromJson<LevelData.JSON>(file);
        LevelData levelToPlay = collection.Levels[levelIndex];

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
            string[] themes = new string[_ThemesProgress.Keys.Count];

            foreach(string value in _ThemesProgress.Keys)
            {
                themes[i] = value;
                i++;
            }
            return themes;
        } 
    }
    public Dictionary<string, ContinousProgress> ThemesProgress => _ThemesProgress;

    private int _TH_Index;
    private string _CurrentTheme;
    //MAKE THE SCREEN THEMES GET THE STRING FROM HERE
    private Dictionary<string, ContinousProgress> _ThemesProgress;

    private const string _THEMES_PREFIX = "TH";
    private const string _THEMES_PATH = "Assets/_Project/Documents/JSON LEVELS/TH/";

    //WE SHOULD HAVE THE SAME PREFIX THING FOR ALL OF THESE AND STANDARISE LANGUAJE NAMING AS WELL
    //ALSO MAYBE MAKE A METHOD TO CRATE PATH STRINGS, USING THE CATEGORY PATH + SPECIFIC + PREFIX + LANGUAJE

    public void playThemeLevels(string theme)
    {
        //here, we need to somehow indicate the THEME folder
        //then, somehow select a languaje

        //using the theme string load the exact level required
    }

    private int getThemeLevelsCount(string theme)
    {
        return getThemeLevels(theme).Length;
    }
    private LevelData[] getThemeLevels(string theme)
    {
        if (_ThemesProgress.ContainsKey(theme) == false)
            Debug.LogError("Theme does not exist!");

        string file = File.ReadAllText($"{_THEMES_PATH}{theme}/{_THEMES_PREFIX}-Spanish.json");
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

    public int GetThemeLevelsCount(string theme) => getThemeLevelsCount(theme);

    #endregion

    #endregion

    #region Save and Load

    private const string HC_PROGRESS = "GP_HC-PROGRESS";
    private const string DC_PROGRESS = "GP_DC-PROGRESS";
    private const string TH_PROGRESS = "GP_TH-PROGRESS";

    private string dcKey(int month) => $"{DC_PROGRESS}-{_Month}";

    private void load()
    {
        loadHCProgress();
        loadTHProgress();
    }

    private void loadHCProgress()
    {
        if (ES3.KeyExists(HC_PROGRESS))
        {
            _HC_Levels_Progress = _StorageManager.Load<ContinousProgress>(HC_PROGRESS);
            return;
        }
        _HC_Levels_Progress = new ContinousProgress(0, null);
    }
    private void loadTHProgress()
    {
        if (ES3.KeyExists(TH_PROGRESS))
            _ThemesProgress = _StorageManager.Load<Dictionary<string, ContinousProgress>>(TH_PROGRESS);
        else
            _ThemesProgress = new Dictionary<string, ContinousProgress>();
        
        string[] themes = getAllThemeTitles();

        for(int i = 0; i < themes.Length; i++)
            if(!_ThemesProgress.ContainsKey(themes[i]))
                _ThemesProgress.Add(themes[i], new ContinousProgress(0, null));
    }

    #endregion
}