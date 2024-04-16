using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
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
    }
    private void OnDisable()
    {
        GameManager.OnLevelCompleted -= onLevelCompleted;
    }

    private void Start()
    {
        if (IsTutorialCompleted)
        {
            Destroy(this);
            return;
        }

        _Message_4 = false;
        //_Message_6 = false;

        LevelData levelData = new LevelData("", _PHRASE, "", $"{_LETTER_ONE} {_LETTER_TWO}");
        _MenuManager.OpenGameplay();
        _GameManager.PlayTutorial(levelData);

        _TutorialScreen.EnableMessage(0);
    }

    private void Update()
    {
        if (_TutorialScreen.IsButtonMessage)
            return;

        if (_PhraseManager.IsLevelCompleted)
        {
            _TutorialScreen.DisableAll();
            return;
        }

        if (_PhraseManager.Selection == null)
        {
            Message_2();
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
    #endregion

    public bool IsTutorialCompleted => ES3.KeyExists(_StorageManager.TUTORIAL_COMPLETED) ? ES3.Load<bool>(_StorageManager.TUTORIAL_COMPLETED) : false;

    private GameLetter[] _Group_One;
    private GameLetter[] _Group_Two;

    private Key _Key_One;
    private Key _Key_Two;

    private bool _Message_4;
    //private bool _Message_6;

    [SerializeField] private Keyboard _Keyboard;

    private const string _PHRASE = "SWAP LETTERS TO REVEAL THE HIDDEN MESSAGE!";
    private const char _LETTER_ONE = 'A';
    private const char _LETTER_TWO = 'E';
    

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

    private void message_2()
    {
        getReferences();

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
        //_Message_6 = true;
        Vector3 position = arrow.RectTransform.position;
        position.y -= 130;
        position.x -= 85;
        _TutorialScreen.SetHand(position);
    }

    public void Message_2() => message_2();
    public void Message_5() => message_5();

}