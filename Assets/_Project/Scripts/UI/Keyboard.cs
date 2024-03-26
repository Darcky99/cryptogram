using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    private PhraseManager _PhraseManager => PhraseManager.Instance;

    #region Unity
    private void Awake()
    {
        _EsensialKeys = new List<char>();
        //_Keys = GetComponentsInChildren<Key>(true);
        _TopKeys = _TopLine.GetComponentsInChildren<Key>(true);
        _MidKeys = _MidLine.GetComponentsInChildren<Key>(true);
        _BotKeys = _BotLine.GetComponentsInChildren<Key>(true);
    }
    private void OnEnable()
    {
        //GameManager.OnLoadLevel += onLoadLevel;
    }
    private void OnDisable()
    {
        //GameManager.OnLoadLevel -= onLoadLevel;
    }
    #endregion

    #region Callbacks
    //private void onLoadLevel(int index)
    //{
    //    initializeKeyboard();
    //}
    #endregion

    private int KeysInTopLine
    {
        get
        {
            int count = 0;
            for(int i = 0; i < _TopKeys.Length; i++)
            {
                Key key = _TopKeys[i];
                if (key.gameObject.activeInHierarchy)
                    count++;
            }
            return count;
        }
    }
    private int KeysInMidLine
    {
        get
        {
            int count = 0;
            for (int i = 0; i < _MidKeys.Length; i++)
            {
                Key key = _MidKeys[i];
                if (key.gameObject.activeInHierarchy)
                    count++;
            }
            return count;
        }
    }
    private int KeysInBotLine
    {
        get
        {
            int count = 0;
            for (int i = 0; i < _BotKeys.Length; i++)
            {
                Key key = _BotKeys[i];
                if (key.gameObject.activeInHierarchy)
                    count++;
            }
            return count;
        }
    }

    private List<char> _EsensialKeys;

    private Key[] _TopKeys;
    private Key[] _MidKeys;
    private Key[] _BotKeys;

    [SerializeField] private RectTransform _TopLine;
    [SerializeField] private RectTransform _MidLine;
    [SerializeField] private RectTransform _BotLine;

    private void disableAllKeys()
    {
        foreach (Key key in _TopKeys)
            key.gameObject.SetActive(false);
        foreach (Key key in _MidKeys)
            key.gameObject.SetActive(false);
        foreach (Key key in _BotKeys)
            key.gameObject.SetActive(false);
    }
    private Key getKey(char character)
    {
        foreach (Key key in _TopKeys)
            if (key.name == character.ToString())
                return key;
        foreach (Key key in _MidKeys)
            if (key.name == character.ToString())
                return key;
        foreach (Key key in _BotKeys)
                if (key.name == character.ToString())
                return key;
        return null;
    }
    private void enableKey(char character)
    {
        Key keyToEnable;
        switch (character)
        {
            case 'Á':
                keyToEnable = getKey('A');
                break;
            case 'É':
                keyToEnable = getKey('E');
                break;
            case 'Í':
                keyToEnable = getKey('I');
                break;
            case 'Ó':
                keyToEnable = getKey('O');
                break;
            case 'Ú':
                keyToEnable = getKey('U');
                break;
            default:
                keyToEnable = getKey(character);
                break;
        }
        if (keyToEnable == null)
            return;
        keyToEnable.gameObject.SetActive(true);
    }
    private void enableEsencialKeys()
    {
        foreach (char character in _PhraseManager.LevelData.Phrase)
        {
            if (character == ' ')
                continue;
            if (_EsensialKeys.Contains(character) == false)
                _EsensialKeys.Add(character);

            enableKey(character);
        }
    }


    private int keyCount(Key[] keys)
    {
        int count = 0;
        for (int i = 0; i < keys.Length; i++)
        {
            Key key = keys[i];
            if (key.gameObject.activeInHierarchy)
                count++;
        }
        return count;
    }
    private void forceKeyCount(Key[] keys, int target)
    {
        int breakHold = 0;

        for (; keyCount(keys) < target;)
        {
            int index = _PhraseManager.RandomGenerator.Next(0, keys.Length);
            Key key = keys[index];
            if (!key.gameObject.activeInHierarchy)
            {
                key.gameObject.SetActive(true);
                if (_PhraseManager.RandomGenerator.Next(0, 100) < 30)
                    key.SetNonInteractuable();
                else
                    key.SetInteractuable();
            }
            breakHold++;
            if (breakHold > 500)
            {
                Debug.LogError("Too much!");
                return;
            }
        }
    }
    private void enableNonEsencialKeys()
    {
        forceKeyCount(_TopKeys, 10);
        forceKeyCount(_MidKeys, 9);
        forceKeyCount(_BotKeys, 7);
    }
    private void initializeKeyboard()
    {
        disableAllKeys();
        enableEsencialKeys();
        enableNonEsencialKeys();
    }

    public void InitializeKeyboard() => initializeKeyboard();
}