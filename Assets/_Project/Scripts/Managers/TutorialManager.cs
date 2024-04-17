using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private MenuManager _MenuManager => MenuManager.Instance;
    private GameManager _GameManager => GameManager.Instance;
    private PhraseManager _PhraseManager => PhraseManager.Instance;
    private Screen_Tutorial _TutorialScreen => _MenuManager.TutorialScreen;
    private StorageManager _StorageManager => StorageManager.Instance;

    #region Unity
    private void OnEnable()
    {
        GameManager.OnLevelCompleted += onLevelCompleted;
        PhraseManager.OnLevelGenerated += onLevelGenerated;
    }
    private void OnDisable()
    {
        GameManager.OnLevelCompleted -= onLevelCompleted;
        PhraseManager.OnLevelGenerated -= onLevelGenerated;
    }

    private void Start()
    {
        if (IsTutorialCompleted)
        {
            Destroy(this);
            return;
        }

        _Message_3 = false;
        _Message_4 = false;
        _Message_6 = false;
        _LevelLoaded = false;

        LevelData levelData = new LevelData("", _PHRASE, "", $"{_LETTER_ONE} {_LETTER_TWO}");
        _MenuManager.OpenGameplay();
        _GameManager.PlayTutorial(levelData);

    }

    private void Update()
    {
        if (_TutorialScreen.IsButtonMessage || !_LevelLoaded)
            return;
        if (_Message_3)
        {
            if (_PhraseManager.Selection != null && _PhraseManager.Selection.IsCompleted)
                _TutorialScreen.DisableAll();
            return;
        }

        if (_PhraseManager.IsLevelCompleted)
        {
            _TutorialScreen.DisableAll();
            return;
        }

        if (_PhraseManager.Selection == null)
        {
            message_2();
            return;
        }
        if (_PhraseManager.Selection.IsCompleted == false)
        {
            message_3();
            return;
        }
        if(_PhraseManager.Selection.IsCompleted && !_Message_4)
        {
            message_4();
            return;
        }
        if (_PhraseManager.Selection.IsCompleted && !_TutorialScreen.IsMessage_5)
        {
            message_6();
            return;
        }
    }
    #endregion

    #region Callbacks
    private void onLevelCompleted()
    {
        ES3.Save(_StorageManager.TUTORIAL_COMPLETED, true);
    }
    private void onLevelGenerated()
    {
        _LevelLoaded = true;
        getReferences();

        message_1();
    }
    #endregion

    public bool IsTutorialCompleted => ES3.KeyExists(_StorageManager.TUTORIAL_COMPLETED) ? ES3.Load<bool>(_StorageManager.TUTORIAL_COMPLETED) : false;

    private GameLetter[] _Group_One;
    private GameLetter[] _Group_Two;

    private Key _Key_One;
    private Key _Key_Two;

    private bool _LevelLoaded;
    private bool _Message_3;
    private bool _Message_4;
    private bool _Message_6;

    [SerializeField] private Keyboard _Keyboard;

    private const string _PHRASE = "SWAP LETTERS TO REVEAL THE HIDDEN MESSAGE!";
    private const char _LETTER_ONE = 'A';
    private const char _LETTER_TWO = 'E';

    private void setCustomText()
    {
        switch (Application.systemLanguage)
        {
            default:
                _TutorialScreen.SetCustomText($"Each number refers to a letter. \n For example, {_Group_One[0].AssignedNumber} is {_Group_One[0].AssignedLetter}.");
                break;
            case SystemLanguage.Spanish:
                _TutorialScreen.SetCustomText($"Cada letra tiene un numero. \n Por ejemplo, {_Group_One[0].AssignedNumber} es {_Group_One[0].AssignedLetter}.");
                break;
        }
    }
    private void getReferences()
    {
        _Group_One = _PhraseManager.GetGameLetters(_LETTER_ONE);
        _Group_Two = _PhraseManager.GetGameLetters(_LETTER_TWO);
        _Key_One = _Keyboard.GetKey(_LETTER_ONE);
        _Key_Two = _Keyboard.GetKey(_LETTER_TWO);
    }

    private bool isComplete(out GameLetter next)
    {
        next = null;
        foreach(GameLetter gameLetter in _Group_One)
            if (!gameLetter.IsCompleted)
            {
                next = gameLetter;
                return false;
            }
        foreach (GameLetter gameLetter in _Group_Two)
            if (!gameLetter.IsCompleted)
            {
                next = gameLetter;
                return false;
            }
        return true;
    }

    private void message_1()
    {
        setCustomText();
        GameLetter gameLetter = _Group_One[0];
        gameLetter.TrySetLetterInText(gameLetter.AssignedLetter);
        _TutorialScreen.EnableMessage(0);

        _TutorialScreen.SetMask(true);
        Vector3 position = gameLetter.RectTransform.position;
        TutorialMaskPool.s_Instance.DeQueue().SetPosition(position);
    }
    private void message_2()
    {
        _TutorialScreen.EnableMessage(1);
        isComplete(out GameLetter gameLetter);
        Vector3 position = gameLetter.RectTransform.position;
        position.y -= 30;
        _TutorialScreen.SetHand(position);
    }
    private void message_3()
    {
        _TutorialScreen.EnableMessage(2);
        Key correct = _PhraseManager.Selection.AssignedLetter == _Key_One.KeyLetter ? _Key_One : _Key_Two;
        Vector3 position = correct.RectTransform.position;
        position.y -= 55;
        _TutorialScreen.SetHand(position);

        if (_Message_6)
            _Message_3 = true;
    }
    private void message_4()
    {
        _TutorialScreen.EnableMessage(3);
        _Message_4 = true;
    }
    private void message_5() 
    {
        _TutorialScreen.EnableMessage(4);
        _TutorialScreen.SetMask(true);

        Vector3 position_one = _Group_One[1].RectTransform.position;
        Vector3 position_two = _Group_One[2].RectTransform.position;
        TutorialMaskPool.s_Instance.DeQueue().SetPosition(position_one);
        TutorialMaskPool.s_Instance.DeQueue().SetPosition(position_two);
    }
    private void message_6()
    {
        ArrowUI arrow = _Keyboard.GetComponentInChildren<ArrowUI>();
        _TutorialScreen.EnableMessage(5);
        Vector3 position = arrow.RectTransform.position;
        position.y -= 130;
        position.x -= 85;
        _TutorialScreen.SetHand(position);

        _Message_6 = true;
    }

    public void Message_2() => message_2();
    public void Message_5() => message_5();
}