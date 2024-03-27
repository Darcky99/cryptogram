using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class GameLetter : MonoBehaviour
{
    private PhraseManager _PhraseManager => PhraseManager.Instance;

    #region Unity
    private void OnEnable()
    {
        GameManager.OnLoadLevel += onLoadLevel;
        GameManager.OnGameOver += onGameOver;

        PhraseManager.OnLetterCompleted += onLetterCompleted;
        PhraseManager.OnSelection += onSelection;
    }
    private void OnDisable()
    {
        GameManager.OnLoadLevel -= onLoadLevel;
        GameManager.OnGameOver -= onGameOver;

        PhraseManager.OnLetterCompleted -= onLetterCompleted;
        PhraseManager.OnSelection -= onSelection;
    }
    #endregion

    #region Callbacks
    private void onLoadLevel(int obj)
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
            complete();
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
    #endregion

    public bool IsCompleted => _LetterInText == _AssignedLetter;
    public char AssignedLetter => _AssignedLetter;
    public Vector2 AbsoluteAnchoredPosition => _GameWord.AbsoluteAnchoredPosition + _RectTransform.anchoredPosition;
    public float Width => _RectTransform.sizeDelta.x;

    private float _ColorAlpha => _ColorBackGroundImage.color.a;
    private bool _IsHighlight => _ColorAlpha == 1f;
    private char _LetterInText => _LetterText.text.Length > 0 ? _LetterText.text[0] : ' ';

    private const float _DiselectedAlpha = 0.5f;

    private GameWord _GameWord;
    private char _AssignedLetter;
    private byte _AssignedNumber;
    private Color _AssignedColor;
    private Sequence _ScaleAnimation;

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

        if (_PhraseManager.LevelData.HidenLetters.Contains(_AssignedLetter))
            return;
        if (_PhraseManager.LevelData.PartiallyHiddenLetters.Contains(_AssignedLetter))
        {
            int random = _PhraseManager.RandomGenerator.Next(0, 100);
            if (random < 50)
                return;
        }
        completeSingle(false);
    }
    private void discard()
    {
        _FadeIn?.Kill();
        _ScaleUpDown?.Kill();
        _PunchUp?.Kill();
        _NumberText.transform.localScale = Vector3.one;
        Destroy(gameObject);
    }
    #endregion

    #region Selection
    private void hardSelected()
    {
        _ScaleAnimation?.Kill();
        _LetterText.rectTransform.localScale = Vector3.one;
        _ColorBackGroundImage.rectTransform.localScale = Vector3.one;
        Sequence sequence = DOTween.Sequence();
        Vector3 scaleUp = Vector3.one * 1.4f;
        Vector3 scaleDown = new Vector3(1.25f, 1.3f);

        _ScaleAnimation = sequence
            .Append(_LetterText.rectTransform.DOScale(scaleUp, 0.08f).SetEase(Ease.OutExpo))
            .Join(_ColorBackGroundImage.rectTransform.DOScale(scaleUp, 0.08f).SetEase(Ease.OutExpo))

            .Append(_LetterText.rectTransform.DOScale(scaleDown, 0.04f).SetEase(Ease.OutQuart))
            .Join(_ColorBackGroundImage.rectTransform.DOScale(scaleDown, 0.04f).SetEase(Ease.OutQuart));

        setBackGroundColorAlpha(1f);
    }
    private void softSelected()
    {
        _ScaleAnimation?.Kill();
        Sequence sequence = DOTween.Sequence();
        Vector3 scaleUp = Vector3.one * 1.4f;
        Vector3 scaleDown = new Vector3(1f, 1f);
        sequence.Pause();
        if (!_IsHighlight)
        {
            _LetterText.rectTransform.localScale = Vector3.one;
            _ColorBackGroundImage.rectTransform.localScale = Vector3.one;

            float scaleUp_Duration = 0.08f;

            _ScaleAnimation = sequence
                .Append(_LetterText.rectTransform.DOScale(scaleUp, scaleUp_Duration).SetEase(Ease.OutExpo))
                .Join(_ColorBackGroundImage.rectTransform.DOScale(scaleUp, scaleUp_Duration).SetEase(Ease.OutExpo));
                //.Join(DOVirtual.Float(_ColorAlpha, 1f, scaleUp_Duration, setBackGroundColorAlpha).SetEase(Ease.OutExpo));
        }
        float scaleDown_Duration = 0.04f;
        sequence
            .Append(_LetterText.rectTransform.DOScale(scaleDown, scaleDown_Duration).SetEase(Ease.OutQuart))
            .Join(_ColorBackGroundImage.rectTransform.DOScale(scaleDown, scaleDown_Duration).SetEase(Ease.OutQuart));
            //.Join(DOVirtual.Float(_ColorAlpha, _DiselectedAlpha, scaleDown_Duration, setBackGroundColorAlpha).SetEase(Ease.OutExpo));

        _ScaleAnimation = sequence;
        _ScaleAnimation.Play();
        setBackGroundColorAlpha(1f);
        //in case this is wrong, uncoment this one above and comment the 2 DOVirtual lines above
    }
    private void diselect()
    {
        _ScaleAnimation?.Kill();
        _LetterText.rectTransform.DOScale(1f, 0.08f);
        _ColorBackGroundImage.rectTransform.DOScale(1f, 0.16f);

        setBackGroundColorAlpha(_DiselectedAlpha);
    }
    #endregion

    #region Animation
    public bool IsWrong => _Wrong == null ? false : _Wrong.IsPlaying();

    private float _AnimationDuration => _ScaleUp + _ScaleDown;

    private float _ScaleUp = 0.09f;
    private float _ScaleDown = 0.28f;

    private Tween _FadeIn;
    private Tween _ScaleUpDown;
    private Tween _PunchUp;
    private Tween _Wrong;

    [SerializeField] private Color _Error;

    private void fadeIn()
    {
        Color color = _LetterText.color;
        _FadeIn = DOVirtual.Float(0, 1, _AnimationDuration, (float value) =>
        {
            color.a = value;
            _LetterText.color = color;
        }).SetEase(Ease.InOutBounce);
    }
    private void scaleUpDown()
    {
        _LetterText.transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(_LetterText.rectTransform.DOScale(1.2f, _ScaleUp).SetEase(Ease.InBounce))
            .Append(_LetterText.rectTransform.DOScale(1f, _ScaleDown).SetEase(Ease.OutBounce));
        _ScaleUpDown = sequence;
    }
    private void punchUp()
    {
        if (_ScaleUpDown != null && _ScaleUpDown.IsPlaying())
            return;
        _PunchUp = _LetterText.rectTransform.DOPunchScale(Vector3.one * 0.3f, _AnimationDuration, elasticity : 0.25f);
    }
    private void wrong(char character)
    {
        _LetterText.text = character.ToString();
        _LetterText.color = _Error;

        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(_LetterText.transform.DOPunchScale(Vector3.one * 0.12f, 0.248f).SetEase(Ease.InBounce))
            .Join(_LetterText.transform.DOPunchRotation(Vector3.forward * 45f, 0.248f).SetEase(Ease.InBounce))
            .OnComplete(() => {
                _LetterText.text = "";
                _LetterText.color = Color.black;
            });
        _Wrong = sequence;
    }
    #endregion

    #region Assign and complete
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
    private void completeSingle(bool animate = true)
    {
        _LetterText.text = _AssignedLetter.ToString();

        if(animate)
        {
            fadeIn();
            scaleUpDown();
        }
        PhraseManager.Instance.CheckCompletition(_AssignedLetter);
    }
    private void complete()
    {
        _LetterText.text = _AssignedLetter.ToString();
        _HintHighlight.gameObject.SetActive(false);
        _NumberText.gameObject.SetActive(false);
        _ColorBackGroundImage.gameObject.SetActive(false);

        if (_PhraseManager.IsGeneratingLevelFlag == false)
            punchUp();
    }
    private bool trySetLetterInText(char character)
    {
        bool isCorrect = compare(character);
        if (isCorrect)
            completeSingle();
        else
            wrong(character);
        return isCorrect;
    }
    #endregion

    private bool compare(char character)
    {
        switch (_AssignedLetter)
        {
            case 'Á':
                return character == 'A' || character == 'Á';
            case 'É':
                return character == 'E' || character == 'É';
            case 'Í':
                return character == 'I' || character == 'Í';
            case 'Ó':
                return character == 'O' || character == 'Ó';
            case 'Ú':
                return character == 'U' || character == 'Ú';
            default:
                return character == _AssignedLetter;
        }
    }
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
        if (IsCompleted)
            return;
        if (HintPanel.Instance.IsHint)
        {
            forceCompleteLetter();
            return;
        }

        setAsSelected();
    }
}