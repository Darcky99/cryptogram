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
    }
    private void OnDisable()
    {
        GameManager.OnLoadLevel -= onLoadLevel;
    }
    #endregion

    #region Callbacks
    private void onLoadLevel(int levelIndex)
    {
        LifePanel.Instance.SetLifeCount(_Lifes);
        HintPanel.Instance.SetHintCount(_Hints);
    }
    #endregion

    public int HyperCasualLevelsCount => _HyperCasualLevels.Count;

    [SerializeField] private int _Lifes, _Hints;
    [SerializeField] private List<LevelData_Scriptable> _HyperCasualLevels;

    public ILevelData GetCurrentHyperCasualLevel() => _HyperCasualLevels[_GameManager.LevelIndex % HyperCasualLevelsCount];
}