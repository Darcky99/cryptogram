using UnityEngine;
using System;

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
            if(splitResult[0] == "")
                return new char[0];
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
            if (splitResult[0] == "")
                return new char[0];
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
    public bool BossLevel => _BossLevel;

    [SerializeField] private string _Author;
    [SerializeField] private string _Phrase;
    [SerializeField] private string _PartiallyHidden;
    [SerializeField] private string _Hidden;
    [SerializeField] private bool _BossLevel;

    public class JSON
    {
        public JSON(LevelData[] levels)
        {
            Levels = levels;
        }

        public LevelData[] Levels;
    }
}