using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBrain : Singleton<GameBrain>
{
    private GameManager _GameManager => GameManager.Instance;

    #region Unity
    public override void Start()
    {
        base.Start();
        SelectLevels(eLevels.OnlineTest);
    }
    private void OnEnable()
    {
        GameManager.OnLoadLevel += onLoadLevel;
        GameManager.OnGameOver += onGameOver;
        TimeManager.OnNewDay += onNewDay;
    }
    private void OnDisable()
    {
        GameManager.OnLoadLevel -= onLoadLevel;
        GameManager.OnGameOver -= onGameOver;
        TimeManager.OnNewDay -= onNewDay;
    }
    #endregion

    #region Callbacks
    private void onLoadLevel(int levelIndex)
    {
        LifePanel.Instance.SetLifeCount(_Lifes);
        HintPanel.Instance.SetHintCount(_Hints);

        _RandomGenerator = new System.Random(levelIndex);
    }
    private void onGameOver()
    {
        expendLife();
    }
    private void onNewDay()
    {
        for( ; _Lifes < 5; )
            earnLife();
    }
    #endregion

    #region Random generator
    public System.Random RandomGenerator => _RandomGenerator;

    private System.Random _RandomGenerator;
    #endregion

    #region Lifes and Hints
    public int Lifes => _Lifes;
    public int Hints => _Hints;

    [SerializeField] private int _Lifes, _Hints;

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
    #endregion

    #region Level load

    public int LevelsCount => _LevelsToLoad.Length;

    private ILevelData[] _LevelsToLoad;

    public ILevelData GetCurrentLevel() => _LevelsToLoad[_GameManager.LevelIndex % LevelsCount];

    public void SelectLevels(eLevels toLoad)
    {
        switch (toLoad, Application.systemLanguage)
        {
            case (eLevels.HC, SystemLanguage.English):
                _LevelsToLoad = Resources.Load<LevelsData_Scriptable>("HC Levels/HC Levels - English.asset").Levels;
                break;
            case (eLevels.HC, SystemLanguage.Spanish):
                _LevelsToLoad = Resources.Load<LevelsData_Scriptable>("HC Levels/HC Levels - Spanish").Levels;
                break;

            case (eLevels.DailyChallenge, SystemLanguage.English):
                break;
            case (eLevels.OnlineTest, SystemLanguage.English):
                _LevelsToLoad = RemoteLoad.GetTestLevels();
                break;
            case (eLevels.OnlineTest, SystemLanguage.Spanish):
                _LevelsToLoad = RemoteLoad.GetTestLevels();
                break;
        }
        _GameManager.LoadLevel();
    }

    #endregion
}