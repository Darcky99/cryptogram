using TMPro;
using System;
using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PhraseManager : Singleton<PhraseManager>
{
    private GameBrain _GameBrain => GameBrain.Instance;
    private GameManager _GameManager => GameManager.Instance;

    #region Unity
    protected override void OnAwakeEvent()
    {
        base.OnAwakeEvent();
        _GameLetters = new List<GameLetter>();
        _CharacterNumber = new Dictionary<char, byte>();
        _CharacterColor = new Dictionary<char, Color>();
    }
    private void OnEnable()
    {
        GameManager.OnLoadLevel += onLoadLevel;
        GameManager.OnLevelCompleted += onLevelCompleted;
        GameManager.OnGameOver += onGameOver;
    }
    private void OnDisable()
    {
        GameManager.OnLoadLevel -= onLoadLevel;
        GameManager.OnLevelCompleted -= onLevelCompleted;
        GameManager.OnGameOver -= onGameOver;
    }
    #endregion

    #region Callbacks
    private void onLoadLevel(int levelIndex)
    {
        clearGameElements();

        _RandomGenerator = new System.Random(levelIndex);
        _LevelData = _GameBrain.GetCurrentHyperCasualLevel();

        _Keyboard.InitializeKeyboard();
        StartCoroutine(generateLevel());
    }

    private void onLevelCompleted()
    {
        displayQuote();
    }
    private void onGameOver()
    {
        Invoke(nameof(clearSelection), 0.248f * 6f);
    }
    #endregion

    public static event Action<GameLetter> OnSelection;
    public static event Action<char> OnLetterCompleted;

    public bool IsGeneratingLevelFlag => _IsGeneratingLevelFlag;
    public bool IsLevelCompleted
    {
        get
        {
            foreach (GameLetter gameLetter in _GameLetters)
                if (!gameLetter.IsCompleted)
                    return false;
            return true;
        } 
    }
    public Dictionary<char, byte> CharacterNumber => _CharacterNumber;
    public Dictionary<char, Color> CharacterColor => _CharacterColor;
    public ILevelData LevelData => _LevelData;
    public System.Random RandomGenerator => _RandomGenerator;

    private GameLine _LastLine => _GameLines.Last();

    private int _MistakeCount = 0;
    private System.Random _RandomGenerator;
    private bool _IsGeneratingLevelFlag;
    private Dictionary<char, byte> _CharacterNumber;
    private Dictionary<char, Color> _CharacterColor;

    [SerializeField, Header("Level generation")] private ILevelData _LevelData;

    [SerializeField, Header("Level generation")] private RectTransform _GameArea;
    [SerializeField, Header("Level generation")] private ScrollRect _ScrollRect;
    [SerializeField, Header("Level generation")] private RectTransform _LevelContent;
    [SerializeField, Header("Level generation")] private VerticalLayoutGroup _VerticalLayerGroup;
    [SerializeField, Header("Level generation")] private GameLine _GameLinePrefab;
    [SerializeField, Header("Level generation")] private Keyboard _Keyboard;

    [SerializeField, Header("Level structure")] private List<GameLine> _GameLines;
    [SerializeField, Header("Level structure")] private List<GameLetter> _GameLetters;

    [SerializeField, Header("Game information")] private GameLetter _LetterSelected;
    [SerializeField, Header("Game information"),] private Color[] _BackGroundColors;

    private int wrapAround(int number, int min, int max)
    {
        int range = max - min + 1;
        number = (number - min) % range;
        if (number < 0)
            number += range;
        return number + min;
    }

    #region Level generation
    private void clearGameElements()
    {
        _LevelData = null;
        _LetterSelected = null;
        _CharacterNumber.Clear();
        _CharacterColor.Clear();
        _ScrollRect.verticalNormalizedPosition = 1;
        mistake(0);
        foreach (GameLine gameLine in _GameLines)
            gameLine.Discard();
        _GameLines.Clear();
        _GameLetters.Clear();
        hideQuote();
    }
    private GameLine addNewLine()
    {
        GameLine line = Instantiate(_GameLinePrefab, _LevelContent);
        _GameLines.Add(line);
        return line;
    }
    private void generateLettersIndex()
    {
        List<int> colorIndex = new List<int>();

        foreach (char character in _LevelData.Phrase)
        {
            if (_GameLinePrefab.GameWordPrefab.IsSpecialCharacter(character))
                continue;

            char fixedCharacter = GameLetter.FixCharacter(character);

            while (_CharacterNumber.ContainsKey(fixedCharacter) == false)
            {
                byte randomNumber = (byte)_RandomGenerator.Next(1, 99);
                if (_CharacterNumber.ContainsValue(randomNumber) == false)
                    _CharacterNumber.Add(fixedCharacter, randomNumber);
            }
            while (_CharacterColor.ContainsKey(fixedCharacter) == false)
            {
                byte randomNumber = (byte)_RandomGenerator.Next(0, _BackGroundColors.Length);
                Color selection = _BackGroundColors[randomNumber];
                if (colorIndex.Contains(randomNumber) == false)
                {
                    colorIndex.Add(randomNumber);
                    _CharacterColor.Add(fixedCharacter, selection);
                }
            }
            if (_CharacterColor.ContainsKey(fixedCharacter) == false)
                Debug.LogError($"Run out of colors, please assign more for this puzzle");
        }
    }
    private float getLinesHeight(GameLine[] gameLines)
    {
        float LevelHeight = 0f;
        foreach (GameLine gameLine in gameLines)
            LevelHeight += gameLine.SizeDelta.y;
        LevelHeight += Mathf.Clamp((gameLines.Length - 1) * _VerticalLayerGroup.spacing, 0f, 999999f);
        return LevelHeight;
    }
    private void fixHeight()
    {
        float height = getLinesHeight(_GameLines.ToArray());
        Vector2 deltaSize = _LevelContent.sizeDelta;
        deltaSize.y = height;
        _LevelContent.sizeDelta = deltaSize;
    }
    private void completeLetters()
    {
        foreach (char character in _LevelData.Phrase)
            checkCompletition(character);
    }
    private IEnumerator generateLevel()
    {
        _IsGeneratingLevelFlag = true;
        generateLettersIndex();
        string[] words = _LevelData.Phrase.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (_GameLines.Count == 0 || _LastLine.CanFitWord(words[i]) == false)
                addNewLine();
            yield return null;
            if (!_LastLine.TryAddWord(words[i]))
                Debug.LogError($"This word seems to be to long {words[i]}");
        }
        completeLetters();
        fixHeight();
        _IsGeneratingLevelFlag = false;
    }
    #endregion

    #region Level state
    private bool isCompleted(char letter)
    {
        foreach (GameLetter gameLetter in _GameLetters)
            if (gameLetter.AssignedLetter == letter && !gameLetter.IsCompleted)
                return false;
        return true;
    }
    private void mistake(int count)
    {
        _MistakeCount = count;
        MistakesPanel.Instance.DisplayMistakeCount(_MistakeCount);
        if (_MistakeCount >= 3)
            _GameManager.GameOver();
    }
    #endregion

    #region Vertical scrolling
    private Tween _ScrollMove;

    private bool isContentVisible(Vector2 anchoredPosition)
    {
        anchoredPosition.y += _GameLinePrefab.SizeDelta.y;
        float sizeY = _GameArea.rect.height;
        float diference = _LevelContent.sizeDelta.y - _GameArea.rect.height;
        float scrollOffset = (1 - _ScrollRect.verticalNormalizedPosition) * diference;
        anchoredPosition.y += scrollOffset;

        Vector2 bounds = new Vector2(0, -sizeY) * 0.9f;

        if (anchoredPosition.y > bounds.x || anchoredPosition.y < bounds.y)
            return false;
        return true;
    }
    private void setVerticalScrollPosition(GameLetter focusLetter)
    {
        bool isVisible = isContentVisible(focusLetter.AbsoluteAnchoredPosition);
        if (isVisible)
            return;
        if (_ScrollMove != null)
            _ScrollMove?.Kill();

        float absY = Mathf.Abs(focusLetter.AbsoluteAnchoredPosition.y);
        float sizeDeltaY = _LevelContent.sizeDelta.y;

        float offset = _GameLinePrefab.SizeDelta.y;
        offset *= absY < (sizeDeltaY / 2f) ? -1 : 1;
        absY += offset;

        float verticalNormalizedPosition = Mathf.Clamp((sizeDeltaY - absY) / sizeDeltaY, 0f, 1f);
        float absDiference = Mathf.Abs(_ScrollRect.verticalNormalizedPosition - verticalNormalizedPosition);
        float time = absDiference * 0.65f;

        _ScrollMove = DOVirtual.Float(_ScrollRect.verticalNormalizedPosition, verticalNormalizedPosition, time,  
            (float value) => _ScrollRect.verticalNormalizedPosition = value)
            .SetEase(Ease.InOutSine);
    }
    #endregion

    #region Hints
    public bool IsHintSequenceFlag => _IsHintSequenceFlag;

    private bool _IsHintSequenceFlag;
    private WaitForSeconds _ForceCompleteDelay = new WaitForSeconds(0.56f * 1.1f);

    private GameLetter[] getIncomplete(char character)
    {
        List<GameLetter> incomplete = new List<GameLetter>();
        foreach(GameLetter gameLetter in _GameLetters)
            if(gameLetter.AssignedLetter == character && !gameLetter.IsCompleted)
                incomplete.Add(gameLetter);
        return incomplete.ToArray();
    }
    private IEnumerator forceCompletition(char character)
    {
        if (_IsGeneratingLevelFlag)
            yield break;

        _IsHintSequenceFlag = true;

        clearSelection();
        GameBrain.Instance.ExpendHint();
        GameLetter[] incomplete = getIncomplete(character);
        yield return _ForceCompleteDelay;

        foreach (GameLetter gameLetter in incomplete)
        {
            gameLetter.TrySetLetterInText(character);
            setVerticalScrollPosition(gameLetter);
            yield return _ForceCompleteDelay;
        }
        if (IsLevelCompleted)
            GameManager.Instance.LevelCompleted();

        _IsHintSequenceFlag = false;
    }
    
    public void ForceCompletition(char character) => StartCoroutine(forceCompletition(character));
    #endregion

    #region Selection and setting
    private void setSelection(GameLetter selected)
    {
        _LetterSelected = selected;
        OnSelection?.Invoke(_LetterSelected);

        if (selected == null)
            return;
        setVerticalScrollPosition(_LetterSelected);
    }
    private void clearSelection() => setSelection(null);
    private void checkCompletition(char character)
    {
        if (isCompleted(character))
        {
            OnLetterCompleted?.Invoke(character);
            clearSelection();
        }
        if (IsLevelCompleted)
            GameManager.Instance.LevelCompleted();
    }
    private void changeSelection(eChangeSelectionMode selectionMode)
    {
        if (_GameLetters == null || _LetterSelected == null)
            return;

        int increase = 0, breaker = 0;
        switch (selectionMode)
        {
            case eChangeSelectionMode.Previous:
                increase = -1;
                break;
            case eChangeSelectionMode.Next:
                increase = 1;
                break;
        }

        int letterIndex = 0;
        for (int c = 0; c < _GameLetters.Count; c++)
            if (_GameLetters[c] == _LetterSelected)
            {
                letterIndex = c;
                break;
            }
        for (int n = increase; Mathf.Abs(n) < _GameLetters.Count; n += increase)
        {
            breaker++;
            int index = wrapAround(letterIndex + n, 0, _GameLetters.Count - 1);
            GameLetter gameLetter = _GameLetters[index];
            if (gameLetter.IsCompleted == false && _LetterSelected.AssignedLetter == gameLetter.AssignedLetter)
            {
                setSelection(gameLetter);
                return;
            }

            if (breaker > 500)
            {
                Debug.LogError("Ya wey!");
                return;
            }
        }
    }

    public void SetSelection(GameLetter selected) => setSelection(selected);
    public void ClearSelection() => clearSelection();
    public void TrySetLetter(char character)
    {
        if (_LetterSelected == null || _LetterSelected.IsCompleted || _LetterSelected.IsWrong)
            return;
        bool isCorrect = _LetterSelected.TrySetLetterInText(character);
        if (!isCorrect)
            mistake(++_MistakeCount);
    }
    public void CheckCompletition(char character)
    {
        if (_IsGeneratingLevelFlag)
            return;

        checkCompletition(character);
    }
    public void AddGameLetter(GameLetter gameLetter) => _GameLetters.Add(gameLetter);
    public void ChangeSelection(eChangeSelectionMode changeSelectionMode) => changeSelection(changeSelectionMode);
    #endregion

    #region Quote
    [SerializeField, Header("Quote")] private TextMeshProUGUI _QuoteText;

    private void displayQuote()
    {
        _QuoteText.rectTransform.SetParent(_LevelContent);
        _QuoteText.text = _LevelData.Author;
        _QuoteText.gameObject.SetActive(true);
    }
    private void hideQuote()
    {
        _QuoteText.rectTransform.SetParent(_LevelContent.parent);
        _QuoteText.text = " ";
        _QuoteText.gameObject.SetActive(false);
    }
    #endregion

}