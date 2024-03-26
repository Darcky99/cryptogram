using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBrain : Singleton<GameBrain>
{
    private GameManager _GameManager => GameManager.Instance;

    private int _HyperCasualLevelsCount => _HyperCasualLevels.Count;
    [SerializeField] private List<LevelData_Scriptable> _HyperCasualLevels;

    public ILevelData GetCurrentHyperCasualLevel() => _HyperCasualLevels[_GameManager.LevelIndex % _HyperCasualLevelsCount];
}