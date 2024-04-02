using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWord : MonoBehaviour
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

    public string AssignedWord => _AssignedWord;
    public Vector2 AbsoluteAnchoredPosition => _GameLine.AnchoredPosition + _RectTransform.anchoredPosition;
    public float Width => _RectTransform.sizeDelta.x;
    private float _GameLetterWidth => _GameLetterPrefab.Width;

    private GameLine _GameLine;

    [SerializeField] private string _AssignedWord;

    [SerializeField] private RectTransform _RectTransform;
    [SerializeField] private HorizontalLayoutGroup _HorizontalLayerGroup;

    [SerializeField, Header("Level structure")] private float _SidesMargin = 35f;
    [SerializeField, Header("Level generation")] private GameLetter _GameLetterPrefab;

    [SerializeField, Header("Level generation")] private TextMeshProUGUI _InitialCharacter;
    [SerializeField, Header("Level generation")] private TextMeshProUGUI _LastCharacter;

    private void initialize(GameLine gameLine, string word)
    {
        _GameLine = gameLine;
        setWord(word);
    }
    private void discard()
    {
        _InitialCharacter.gameObject.SetActive(false);
        _LastCharacter.gameObject.SetActive(false);
        Destroy(gameObject);
    }

    #region Special characters
    private int specialCharactersCount(string word)
    {
        int count = 0;
        foreach(char character in word)
        {
            if (isInitialCharacter(character) || isFinalCharacter(character))
                count++;
        }
        return count;
    }

    private bool isInitialCharacter(char character)
    {
        switch (character)
        {
            case '¡':
            case '¿':
                return true;
            default:
                return false;
        }
    }
    private bool isFinalCharacter(char character)
    {
        switch (character)
        {
            case ',':
            case '.':
            case '?':
            case '!':
            case ':':
                return true;
            default:
                return false;
        }
    }

    private void setInitialCharacter(char character)
    {
        _InitialCharacter.rectTransform.parent.gameObject.SetActive(true);
        _InitialCharacter.text = character.ToString();
    }
    private void setFinalCharacter(char character)
    {
        _LastCharacter.rectTransform.parent.gameObject.SetActive(true);
        _LastCharacter.text = character.ToString();
    }

    private bool trySpecialCharacter(char character)
    {
        if (isInitialCharacter(character))
        {
            setInitialCharacter(character);
            return true;
        }
        else if (isFinalCharacter(character))
        {
            setFinalCharacter(character); ;
            return true;
        }
        return false;
    }
    #endregion

    private void createWord(string word)
    {
        foreach (char character in word)
        {
            if (trySpecialCharacter(character))
                continue;

            char fixedCharacter = GameLetter.FixCharacter(character);

            GameLetter gameLetter = Instantiate(_GameLetterPrefab, transform);
            byte keyNumber = _PhraseManager.CharacterNumber[fixedCharacter];
            Color keyColor = _PhraseManager.CharacterColor[fixedCharacter];

            _LastCharacter.rectTransform.parent.SetSiblingIndex(transform.childCount - 1);

            gameLetter.Initialize(this, fixedCharacter, keyNumber, keyColor);
            _PhraseManager.AddGameLetter(gameLetter);
        }
        float sizeX = getWordWidth(_AssignedWord) + _SidesMargin;
        Vector2 deltaSize = _RectTransform.sizeDelta;
        deltaSize.x = sizeX;
        _RectTransform.sizeDelta = deltaSize;
    }
    private float getWordWidth(string word)
    {
        float wordWidth = 0;
        int specialChars = specialCharactersCount(word);
        wordWidth += (word.Length - specialChars) * _GameLetterWidth;
        wordWidth += Mathf.Clamp((word.Length - 1) * _HorizontalLayerGroup.spacing, 0f, 999999f);
        return wordWidth;
    }
    private void setWord(string word)
    {
        _AssignedWord = word;
        createWord(_AssignedWord);
    }

    public void Initialize(GameLine gameLine, string word) => initialize(gameLine, word);
    public float GetWordWidth(string word) => getWordWidth(word);
    public bool IsSpecialCharacter(char character) => isInitialCharacter(character) || isFinalCharacter(character);
}