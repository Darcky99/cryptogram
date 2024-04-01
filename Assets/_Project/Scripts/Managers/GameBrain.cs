using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBrain : Singleton<GameBrain>
{
    private GameManager _GameManager => GameManager.Instance;

    #region Unity
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
    }
    private void onGameOver()
    {
        expendLife();
    }
    private void onNewDay()
    {
        earnLife();
    }
    #endregion

    public int HyperCasualLevelsCount => _LevelsToLoad.Levels.Length;

    public int Lifes => _Lifes;
    public int Hints => _Hints;

    [SerializeField] public LevelsAvailable delete;

    [SerializeField] private int _Lifes, _Hints;
    [SerializeField] private LevelsData_Scriptable _LevelsToLoad;

    public ILevelData GetCurrentHyperCasualLevel() => _LevelsToLoad.Levels[_GameManager.LevelIndex % HyperCasualLevelsCount];

    #region Lifes and Hints
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


}