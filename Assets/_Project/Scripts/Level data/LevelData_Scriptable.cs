using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class LevelData_Scriptable : ScriptableObject, ILevelData
{
    public string Phrase => _Phrase;
    public string Author => _Author;
    public char[] PartiallyHiddenLetters
    {
        get
        {
            string[] splitResult = _PartiallyHiddenLetters.Split(' ');
            char[] characters = new char[splitResult.Length];
            for (int i = 0; i < splitResult.Length; i++)
            {
                if (splitResult[i].Length > 1)
                    Debug.LogError($"{_PartiallyHiddenLetters} must be separated by ' '");
                characters[i] = splitResult[i][0];
            }
            return characters;
        }
    }
    public char[] HiddenLetters
    {
        get
        {
            string[] splitResult = _HidenLetters.Split(' ');
            char[] characters = new char[splitResult.Length];
            for (int i = 0; i < splitResult.Length; i++)
            {
                if (splitResult[i].Length > 1)
                    Debug.LogError($"{_HidenLetters} must be separated by ' '");
                characters[i] = splitResult[i][0];
            }
            return characters;
        }
    }

    [SerializeField] private string _Phrase;
    [SerializeField] private string _Author;
    [SerializeField] private string _PartiallyHiddenLetters;
    [SerializeField] private string _HidenLetters;
}