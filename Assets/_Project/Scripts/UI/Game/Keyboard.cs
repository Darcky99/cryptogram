using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    private GameManager _GameManager => GameManager.Instance;
    private PhraseManager _PhraseManager => PhraseManager.Instance;

    #region Unity
    private void Awake()
    {
        _EsensialKeys = new List<char>();
        _Forbidden = new List<char>();
        _TopKeys = _TopLine.GetComponentsInChildren<Key>(true);
        _MidKeys = _MidLine.GetComponentsInChildren<Key>(true);
        _BotKeys = _BotLine.GetComponentsInChildren<Key>(true);
    }
    private void OnEnable()
    {
        PhraseManager.OnLetterCompleted += onLetterCompleted;
    }
    private void OnDisable()
    {
        PhraseManager.OnLetterCompleted -= onLetterCompleted;
    }
    #endregion

    #region Callback
    private void onLetterCompleted(char letter)
    {
        if (_PhraseManager.IsGeneratingLevelFlag)
            return;

        setKeyInteraction(letter, false);
    }
    #endregion

    private List<char> _EsensialKeys;

    private Key[] _TopKeys;
    private Key[] _MidKeys;
    private Key[] _BotKeys;

    [SerializeField] private RectTransform _TopLine;
    [SerializeField] private RectTransform _MidLine;
    [SerializeField] private RectTransform _BotLine;

    #region Keys state
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
    private void setKey(char character, bool enable)
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
        keyToEnable.gameObject.SetActive(enable);
    }
    private void disableAllKeys()
    {
        foreach (Key key in _TopKeys)
            key.gameObject.SetActive(false);
        foreach (Key key in _MidKeys)
            key.gameObject.SetActive(false);
        foreach (Key key in _BotKeys)
            key.gameObject.SetActive(false);
    }
    #endregion

    #region Esencial keys
    private void enableEsencialKeys()
    {
        foreach (char character in _PhraseManager.LevelData.Phrase)
        {
            if (character == ' ')
                continue;
            if (_EsensialKeys.Contains(character) == false)
                _EsensialKeys.Add(character);

            setKey(character, true);
        }
    }
    private void enableNonEsencialKeys()
    {
        forceKeyCount(_TopKeys, 10);
        forceKeyCount(_MidKeys, 9);
        forceKeyCount(_BotKeys, 7);
    }
    #endregion

    #region Language
    private List<char> _Forbidden;

    private void forbit(params char[] characters)
    {
        foreach (char character in characters)
            if (_Forbidden.Contains(character) == false)
                _Forbidden.Add(character);
    }
    private void invertYZ()
    {
        Key Y = getKey('Y');
        Key Z = getKey('Z');
        Transform YParent = Y.transform.parent;
        Transform ZParent = Z.transform.parent;
        int YsinblingIndex = Y.transform.GetSiblingIndex();
        int ZsinblingIndex = Z.transform.GetSiblingIndex();

        Y.transform.SetParent(ZParent);
        Z.transform.SetParent(YParent);
        Y.transform.SetSiblingIndex(ZsinblingIndex);
        Z.transform.SetSiblingIndex(YsinblingIndex);
    }
    private void setLanguajeConfiguration()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Spanish:
                forbit('Ä', 'Ö', 'Ü');
                break;
            case SystemLanguage.German:
                forbit('Ñ');
                invertYZ();
                break;
            default:
                forbit('Ñ', 'Ä', 'Ö', 'Ü');
                break;
        }
    }
    #endregion

    #region Key count
    private int enabledKeysCount(Key[] keys)
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

        for ( ; enabledKeysCount(keys) < target; )
        {
            int index = _GameManager.RandomGenerator.Next(0, keys.Length);
            Key key = keys[index];
            if (!key.gameObject.activeInHierarchy && _Forbidden.Contains(key.KeyLetter) == false)
            {
                key.gameObject.SetActive(true);
                if (_GameManager.RandomGenerator.Next(0, 100) < 30)
                    key.SetInteraction(false);
                else
                    key.SetInteraction(true);
            }
            #region Loop breaker
            breakHold++;
            if (breakHold > 500)
            {
                Debug.LogError("Too much!");
                return;
            }
            #endregion
        }
    }
    #endregion

    #region Hints
    private void setKeyInteraction(char character, bool enabled) => getKey(character).SetInteraction(enabled);

    public void SetKeyHint(char character, bool enabled) => getKey(character).SetHint(enabled);
    #endregion

    private void initializeKeyboard()
    {
        disableAllKeys();
        setLanguajeConfiguration();
        enableEsencialKeys();
        enableNonEsencialKeys();
    }

    public void InitializeKeyboard() => initializeKeyboard();
}