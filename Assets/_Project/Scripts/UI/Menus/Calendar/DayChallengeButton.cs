using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DayChallengeButton : MonoBehaviour
{
    private GameManager _GameManager => GameManager.Instance;

    private void OnEnable()
    {
        if(_Month != 0)
            updateState();
    }

    public int LevelIndex => _LevelIndex;

    private Screen_Callendar _ScreenCallendar;

    private int _Month;
    private int _LevelIndex;

    [SerializeField] private Color _SelectedColor;

    [SerializeField] private GameObject _DayButton;
    [SerializeField] private GameObject _Padklock;
    [SerializeField] private GameObject _Thropy;

    [SerializeField] private TextMeshProUGUI _Number;
    [SerializeField] private Image _BackGroundImage;

    private void updateState()
    {
        _DayButton.SetActive(false);
        _Padklock.SetActive(false);
        _Thropy.SetActive(false);

        if (_GameManager.DC_Levels_Progress[_Month].Items[LevelIndex].IsCompleted)
        {
            _Thropy.SetActive(true);
            return;
        }
        DateTime today = DateTime.Today;

        //NEED TO CONSIDER YEARS HERE

        if(_Month < today.Month)
        {
            _DayButton.SetActive(true);
            return;
        }

        if (_LevelIndex <= today.Day - 1)
            _DayButton.SetActive(true);
        else
            _Padklock.SetActive(true);
    }

    public void Initialize(Screen_Callendar screen, int month, int levelIndex) 
    {
        _ScreenCallendar = screen;
        _Month = month;
        _LevelIndex = levelIndex;
        _Number.text = (_LevelIndex + 1).ToString();
        updateState();
    }
    public void OnButtonDown()
    {
        _BackGroundImage.color = _SelectedColor;
        _ScreenCallendar.SetSelection(this);
    }

    public void Deselect()
    {
        _BackGroundImage.color = Color.white;
    }
}