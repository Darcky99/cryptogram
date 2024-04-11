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
            int t = DateTime.Today.Year + DateTime.Today.DayOfYear;
            instance.Initialize(this, i, i <= today.Day - 1);
        }
        _GameManager.SetMonth(_Month);
    }

    public void SetSelection(DayChallengeButton button)
    {
        _Selected?.Deselect();
        _Selected = button;
        DaySelectionText.text = $"DAY {_Selected.LevelIndex + 1}";
    }
    public void GoToDailyChallengeLevel()
    {
        _MenuManager.OpenGameplay();
        _GameManager.PlayDH(_Selected.LevelIndex);
    }

    public void MainMenu()
    {
        _MenuManager.MainMenuScreen.DisableAllPanels();
    }
}