using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameLetter : MonoBehaviour
{
    private PhraseManager _PhraseManager => PhraseManager.Instance;
    private GameManager _GameManager => GameManager.Instance;

    #region Unity
    private void OnEnable()
    {
        GameManager.OnLoadLevel += onLoadLevel;
        GameManager.OnGameOver += onGameOver;

        PhraseManager.OnLetterCompleted += onLetterCompleted;
        PhraseManager.OnSelection += onSelection;

        HintPanel.OnHintPanel += onHintPanel;
    }
    private void OnDisable()
    {
        GameManager.OnLoadLevel -= onLoadLevel;
        GameManager.OnGameOver -= onGameOver;

        PhraseManager.OnLetterCompleted -= onLetterCompleted;
        PhraseManager.OnSelection -= onSelection;

        HintPanel.OnHintPanel -= onHintPanel;
    }
    #endregion

    #region Callbacks
    private void onLoadLevel(ILevelData levelData)
    {
        discard();
    }
    private void onGameOver()
    {
        _Button.interactable = false;
    }

    private void onLetterCompleted(char completed)
    {
        if (_AssignedLetter == completed)
            completeCharacter();
    }
    private void onSelection(GameLetter gameLetter)
    {
        if (gameLetter == null || IsCompleted)
            diselect();
        else if (gameLetter == this)
            hardSelected();
        else if (gameLetter == null || gameLetter.AssignedLetter == _AssignedLetter)
            softSelected();
        else
            diselect();
    }

    private void onHintPanel(bool condition)
    {
        if(!IsCompleted)
            _HintHighlight.gameObject.SetActive(condition);
    }
    #endregion

    public RectTransform RectTransform => _RectTransform;
    public bool IsCompleted => FixCharacter(_LetterInText) == _AssignedLetter;
    public char AssignedLetter => _AssignedLetter;
    public byte AssignedNumber => _AssignedNumber;
    public Vector2 AbsoluteAnchoredPosition => _GameWord.AbsoluteAnchoredPosition + _RectTransform.anchoredPosition;
    public float Width => _RectTransform.sizeDelta.x;

    private bool _IsHighlight => _ColorAlpha == 1f;
    private float _ColorAlpha => _ColorBackGroundImage.color.a;
    private char _LetterInText => _LetterText.text.Length > 0 ? _LetterText.text[0] : ' ';

    private const float _DiselectedAlpha = 0.5f;

    private GameWord _GameWord;
    private char _AssignedLetter;
    private byte _AssignedNumber;
    private Color _AssignedColor;

    [SerializeField] private RectTransform _RectTransform;
    [SerializeField] private TextMeshProUGUI _LetterText;
    [SerializeField] private TextMeshProUGUI _NumberText;
    [SerializeField] private Image _ColorBackGroundImage;
    [SerializeField] private Button _Button;
    [SerializeField] private Image _HintHighlight;

    #region Init & Disc
    private void initialize(GameWord parent, char assignedLetter, byte assignedNumber, Color assignedColor)
    {
        _GameWord = parent;
        _AssignedLetter = assignedLetter;
        setAssignedNumber(assignedNumber);
        setAssignedColor(assignedColor);

        if (_PhraseManager.LevelData.Hidden.Contains(_AssignedLetter))
            return;
        if (_PhraseManager.LevelData.PartiallyHidden.Contains(_AssignedLetter))
        {
            int random = _GameManager.RandomGenerator.Next(0, 100);
            if (random < 50)
                return;
        }
        completeSingle(false);
    }
    private void discard()
    {
        _LetterText.rectTransform.DOKill();
        _ColorBackGroundImage.rectTransform.DOKill();
        _GeneralAnimation?.Kill();
        _Wrong?.Kill();
        _NumberText.transform.localScale = Vector3.one;
        Destroy(gameObject);
    }
    #endregion

    #region Selection
    private void hardSelected()
    {
        _GeneralAnimation?.Kill();
        _LetterText.rectTransform.localScale = Vector3.one;
        _ColorBackGroundImage.rectTransform.localScale = Vector3.one;
        Sequence sequence = DOTween.Sequence();
        Vector3 scaleUp = Vector3.one * 1.4f;
        Vector3 scaleDown = new Vector3(1.25f, 1.3f);

        _GeneralAnimation = sequence
            .Append(_LetterText.rectTransform.DOScale(scaleUp, 0.08f).SetEase(Ease.OutExpo))
            .Join(_ColorBackGroundImage.rectTransform.DOScale(scaleUp, 0.08f).SetEase(Ease.OutExpo))

            .Append(_LetterText.rectTransform.DOScale(scaleDown, 0.04f).SetEase(Ease.OutQuart))
            .Join(_ColorBackGroundImage.rectTransform.DOScale(scaleDown, 0.04f).SetEase(Ease.OutQuart));

        setBackGroundColorAlpha(1f);
    }
    private void softSelected()
    {
        _GeneralAnimation?.Kill();
        Sequence sequence = DOTween.Sequence();
        Vector3 scaleUp = Vector3.one * 1.4f;
        Vector3 scaleDown = new Vector3(1f, 1f);
        sequence.Pause();
        if (!_IsHighlight)
        {
            _LetterText.rectTransform.localScale = Vector3.one;
            _ColorBackGroundImage.rectTransform.localScale = Vector3.one;

            float scaleUp_Duration = 0.08f;

            _GeneralAnimation = sequence
                .Append(_LetterText.rectTransform.DOScale(scaleUp, scaleUp_Duration).SetEase(Ease.OutExpo))
                .Join(_ColorBackGroundImage.rectTransform.DOScale(scaleUp, scaleUp_Duration).SetEase(Ease.OutExpo));
        }
        float scaleDown_Duration = 0.04f;
        sequence
            .Append(_LetterText.rectTransform.DOScale(scaleDown, scaleDown_Duration).SetEase(Ease.OutQuart))
            .Join(_ColorBackGroundImage.rectTransform.DOScale(scaleDown, scaleDown_Duration).SetEase(Ease.OutQuart));

        _GeneralAnimation = sequence;
        _GeneralAnimation.Play();
        setBackGroundColorAlpha(1f);
    }
    private void diselect()
    {
        _ColorBackGroundImage.rectTransform.DOScale(1f, 0.16f);
        if(!_SingleAnimation)
            _LetterText.rectTransform.DOScale(1f, 0.08f);
        setBackGroundColorAlpha(_DiselectedAlpha);
    }
    #endregion

    #region Animation
    public bool IsWrong => _Wrong == null ? false : _Wrong.IsPlaying();

    private float _AnimationDuration => _ScaleUpDuration + _ScaleDownDuration;

    private float _ScaleUpDuration = 0.28f;
    private float _ScaleDownDuration = 0.28f;

    private Tween _GeneralAnimation;
    private Tween _Wrong;

    [SerializeField] private Color _Error;

    private Tween fadeIn()
    {
        Color color = _LetterText.color;
        Tween _FadeIn = DOVirtual.Float(0, 1, _ScaleUpDuration, (float value) =>
        {
            color.a = value;
            _LetterText.color = color;
        }).SetEase(Ease.InOutSine);
        return _FadeIn;
    }
    private Tween fadeOut()
    {
        Color color = _LetterText.color;
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(DOVirtual.Float(1, 0, _ScaleDownDuration, (float value) =>
            {
                color.a = value;
                _LetterText.color = color;
            })
            .SetEase(Ease.InOutSine))
            .Join(_LetterText.rectTransform.DOScale(0, _ScaleDownDuration).SetEase(Ease.OutSine));
        Tween _FadeOut = sequence;
        return _FadeOut;
    }
    private Tween scaleUpDown()
    {
        _LetterText.transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(_LetterText.rectTransform.DOScale(1.2f, _ScaleUpDuration).SetEase(Ease.InSine))
            .Append(_LetterText.rectTransform.DOScale(1f, _ScaleDownDuration).SetEase(Ease.OutSine));
        Tween _ScaleUpDown = sequence;
        return _ScaleUpDown;
    }
    private void punchUp()
    {
        _GeneralAnimation = _LetterText.rectTransform.DOPunchScale(Vector3.one * 0.3f, _AnimationDuration, elasticity : 0.25f);
    }
    private void wrong(char character)
    {
        _LetterText.text = character.ToString();
        _LetterText.color = Color.white;
        _ColorBackGroundImage.color = _Error;

        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(scaleUpDown())
            .Join(fadeIn())
            .Join(DOVirtual.Color(_ColorBackGroundImage.color, _Error, _ScaleUpDuration, (Color color) => {
                _ColorBackGroundImage.color = color;
            }))
            .Append(_LetterText.rectTransform.DOPunchRotation(Vector3.forward * 15, 0.248f, 20, elasticity : 4).SetEase(Ease.InBounce))
            .Join(_ColorBackGroundImage.rectTransform.DOPunchRotation(Vector3.forward * 15, 0.248f, 20, elasticity: 4).SetEase(Ease.InBounce))
            .Join(_LetterText.rectTransform.DOPunchScale(Vector3.one * 0.12f, 0.248f).SetEase(Ease.InBounce))
            .Append(fadeOut())
            .Append(DOVirtual.Color(_Error, _PhraseManager.CharacterColor[_AssignedLetter], _ScaleDownDuration, (Color color) => {
                _ColorBackGroundImage.color = color;
            }))
            .OnComplete(() => {
                _LetterText.text = "";
                _LetterText.color = Color.black;
            });
        _Wrong = sequence;
    }
    #endregion

    #region Assign
    private void setAssignedNumber(byte assignedNumber)
    {
        _AssignedNumber = assignedNumber;
        _NumberText.text = _AssignedNumber.ToString();
    }
    private void setAssignedColor(Color assignedColor)
    {
        _AssignedColor = assignedColor;
        setBackGroundColorAlpha(_DiselectedAlpha);
    }
    private void setBackGroundColorAlpha(float alpha)
    {
        Color color = _AssignedColor;
        color.a = alpha;
        _ColorBackGroundImage.color = color;
    }
    #endregion

    #region Complete
    private bool _SingleAnimation;

    private void useCorrectCharacter()
    {
        int index = _RectTransform.GetSiblingIndex() - 1;
        _LetterText.text = _GameWord.AssignedWord[index].ToString();
    }
    private void completeSingle(bool animate = true)
    {
        _GeneralAnimation?.Kill();
        useCorrectCharacter();
        if (animate)
        {
            _SingleAnimation = true;

            Sequence sequence = DOTween.Sequence();
            sequence
                .Append(scaleUpDown())
                .Join(fadeIn())
                .OnComplete(() => _SingleAnimation = false);
        }
        PhraseManager.Instance.CheckCompletition(_AssignedLetter);
    }
    private void completeCharacter()
    {
        _NumberText.gameObject.SetActive(false);
        _ColorBackGroundImage.gameObject.SetActive(false);

        if (_SingleAnimation)
            return;

        useCorrectCharacter();

        if (_PhraseManager.IsGeneratingLevelFlag == false)
            punchUp();
    }
    private bool trySetLetterInText(char character)
    {
        bool isCorrect = character == _AssignedLetter;
        if (isCorrect)
            completeSingle();
        else
            wrong(character);
        return isCorrect;
    }
    #endregion

    private void setAsSelected()
    {
        PhraseManager.Instance.SetSelection(this);
    }
    private void forceCompleteLetter()
    {
        trySetLetterInText(_AssignedLetter);
        PhraseManager.Instance.ForceCompletition(_AssignedLetter);
    }

    public void Initialize(GameWord parent, char assignedLetter, byte assignedNumber, Color assignedColor) => initialize(parent, assignedLetter, assignedNumber, assignedColor);
    public bool TrySetLetterInText(char character) => trySetLetterInText(character);

    public void OnButtonDown()
    {
        if (IsCompleted || _PhraseManager.IsHintSequenceFlag)
            return;

        if (HintPanel.Instance.IsHintInterfaceEnabled)
        {
            forceCompleteLetter();
            return;
        }
        setAsSelected();
    }

    public static char FixCharacter(char character)
    {
        switch (character)
        {
            case 'Á':
                return 'A';
            case 'É':
                return 'E';
            case 'Í':
                return 'I';
            case 'Ó':
                return 'O';
            case 'Ú':
                return 'U';
            default:
                return character;
        }
    }
}