using System.Collections.Generic;
using UnityEngine;
using System;
using MongoDB.Bson;
using MongoDB.Driver;

[Serializable]
public class LevelData : ILevelData
{
    public LevelData(string author, string phrase, string partiallyHidden, string hidden)
    {
        _Author = author;
        _Phrase = phrase;
        _PartiallyHidden = partiallyHidden;
        _Hidden = hidden;
    }

    public string Author => _Author;
    public string Phrase => _Phrase;
    public char[] PartiallyHidden
    {
        get
        {
            string[] splitResult = _PartiallyHidden.Split(' ');
            char[] characters = new char[splitResult.Length];
            for (int i = 0; i < splitResult.Length; i++)
            {
                if (splitResult[i].Length > 1)
                    Debug.LogError($"{_PartiallyHidden} must be separated by ' '");
                characters[i] = splitResult[i][0];
            }
            return characters;
        }
    }
    public char[] Hidden
    {
        get
        {
            string[] splitResult = _Hidden.Split(' ');
            char[] characters = new char[splitResult.Length];
            for (int i = 0; i < splitResult.Length; i++)
            {
                if (splitResult[i].Length > 1)
                    Debug.LogError($"{_Hidden} must be separated by ' '");
                else if (splitResult[i].Length < 1)
                    Debug.LogWarning($"{_Hidden} '' empty string not valid");
                characters[i] = splitResult[i][0];
            }
            return characters;
        }
    }

    [SerializeField] private string _Author;
    [SerializeField] private string _Phrase;
    [SerializeField] private string _PartiallyHidden;
    [SerializeField] private string _Hidden;

    public class JSON
    {
        public JSON(LevelData[] levels)
        {
            Levels = levels;
        }

        public LevelData[] Levels;
    }
}

