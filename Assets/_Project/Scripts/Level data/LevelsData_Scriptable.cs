using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Levels", menuName = "Levels")]
public class LevelsData_Scriptable : ScriptableObject
{
    public LevelData[] Levels => _Levels;

    [SerializeField] private LevelData[] _Levels;

    public void SetLevels(LevelData[] levels)
    {
        _Levels = levels;
    }
}