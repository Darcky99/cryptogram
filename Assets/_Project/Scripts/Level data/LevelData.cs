using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LevelData : ILevelData
{
    public LevelData(string author, string phrase, string partiallyHiddenLetters, string hidenLetters)
    {
        _Author = author;
        _Phrase = phrase;
        _PartiallyHiddenLetters = partiallyHiddenLetters;
        _HiddenLetters = hidenLetters;
    }

    public string Author => _Author;
    public string Phrase => _Phrase;
    public char[] PartiallyHiddenLetters 
    { 
        get
        {
            string[] splitResult = _PartiallyHiddenLetters.Split(' ');
            char[] characters = new char[splitResult.Length];
            for(int i = 0; i < splitResult.Length; i++)
            {
                if(splitResult[i].Length > 1)
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
            string[] splitResult = _HiddenLetters.Split(' ');
            char[] characters = new char[splitResult.Length];
            for (int i = 0; i < splitResult.Length; i++)
            {
                if (splitResult[i].Length > 1)
                    Debug.LogError($"{_HiddenLetters} must be separated by ' '");
                characters[i] = splitResult[i][0];
            }
            return characters;
        }
    }

    [SerializeField] private string _Author;
    [SerializeField] private string _Phrase;
    [SerializeField] private string _PartiallyHiddenLetters;
    [SerializeField] private string _HiddenLetters;
}