using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class LevelData_Scriptable : ScriptableObject, ILevelData
{
    public string Phrase => _Phrase;
    public string Author => _Author;
    public List<char> PartiallyHiddenLetters => _PartiallyHiddenLetters;
    public List<char> HidenLetters => _HidenLetters;

    [SerializeField] private string _Phrase;
    [SerializeField] private string _Author;
    [SerializeField] private List<char> _PartiallyHiddenLetters;
    [SerializeField] private List<char> _HidenLetters;
}