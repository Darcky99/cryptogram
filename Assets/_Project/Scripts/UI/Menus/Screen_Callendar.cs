using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Screen_Callendar : MenuScreenBase
{
    private GameManager _GameManager => GameManager.Instance;
    private MenuManager _MenuManager => MenuManager.Instance;

    private void Start()
    {
        initializeScreen();
    }

    private int _Month;

    private DayChallengeButton _Selected;
    private DayChallengeButton[] _DayButtons;

    [SerializeField] private MonthText _MonthText;
    [SerializeField] private TextMeshProUGUI DaySelectionText;
    [SerializeField] private RectTransform _DayButtonsContainer;
    
    [SerializeField] private DayChallengeButton _DayChallengeButtonPrefab;

    private void initializeScreen()
    {
        DateTime today = DateTime.Today;
        _Month = today.Month;
        _MonthText.SetMonth(_Month);
        int dayCount = DateTime.DaysInMonth(today.Year, today.Month);

        for(int i = 0; i < dayCount; i++)
        {
            DayChallengeButton instance = Instantiate(_DayChallengeButtonPrefab, _DayButtonsContainer);
            instance.Initialize(this, i);
        }
    }

    public void SetSelection(DayChallengeButton button)
    {
        _Selected?.Deselect();
        _Selected = button;
        DaySelectionText.text = $"DAY {_Selected.LevelIndex + 1}";
    }
    public void GoToDailyChallengeLevel()
    {
        _GameManager.SetLevelIndex(_Selected.LevelIndex);

        _MenuManager.ChangeScreenState(eScreen.DailyChallengeCalendar, false);
        _MenuManager.ChangeScreenState(eScreen.Gameplay, true);

        _GameManager.PLayDailyChallenge(_Month, _Selected.LevelIndex);
    }

    public void MainMenu()
    {
        _MenuManager.ChangeScreenState(eScreen.DailyChallengeCalendar, false);
        _MenuManager.ChangeScreenState(eScreen.MainMenu, true);
    }
}