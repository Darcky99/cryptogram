using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLine : MonoBehaviour
{
    private PhraseManager _PhraseManager => PhraseManager.Instance;

    #region Unity
    private void OnEnable()
    {
        GameManager.OnLoadLevel += onLoadLevel;
    }
    private void OnDisable()
    {
        GameManager.OnLoadLevel -= onLoadLevel;
    }
    #endregion

    #region Callbacks
    private void onLoadLevel(ILevelData levelData)
    {
        discard();
    }
    #endregion

    public Vector2 AnchoredPosition => _RectTransform.anchoredPosition;
    public Vector2 SizeDelta => _RectTransform.sizeDelta;
    public GameWord GameWordPrefab => _GameWordPrefab;

    private string[] _Words = new string[0];
    private List<GameWord> _GameWords = new List<GameWord>();

    [SerializeField] private RectTransform _RectTransform;
    [SerializeField] private HorizontalLayoutGroup _HorizontalLayerGroup;
    [SerializeField, Header("Level structure")] private GameWord _GameWordPrefab;

    #region Level creation
    private void createWord(string word)
    {
        GameWord gameWord = Instantiate(_GameWordPrefab, transform);
        gameWord.Initialize(this, word);
        _GameWords.Add(gameWord);
    }
    private void discard()
    {
        Destroy(gameObject);
    }

    public bool CanFitWord(string word) => canFitWord(word);
    public bool TryAddWord(string world) => tryAddWord(world);
    public void Discard() => discard();
    #endregion

    #region Add words by width
    private float getPhraseWidth(string[] words)
    {
        float PhraseWidth = 0f;
        foreach (GameWord gameWord in _GameWords)
            PhraseWidth += gameWord.Width;

        PhraseWidth += Mathf.Clamp((words.Length - 1) * _HorizontalLayerGroup.spacing, 0f, 999999f);
        return PhraseWidth;
    }
    private void addWord(string newWord)
    {
        createWord(newWord);

        string[] newPhrase = new string[_Words.Length + 1];
        _Words.CopyTo(newPhrase, 0);
        newPhrase[newPhrase.Length - 1] = newWord;
        _Words = newPhrase;
    }
    private bool canFitWord(string word)
    {
        float currentWidth = getPhraseWidth(_Words);
        float nextWidth = currentWidth + _GameWordPrefab.GetWordWidth(word);
        float lineWidth = _RectTransform.sizeDelta.x;

        return nextWidth < lineWidth;
    }
    private bool tryAddWord(string word)
    {
        bool canFit = canFitWord(word);
        if (canFit)
            addWord(word);
        return canFit;
    }
    #endregion
}