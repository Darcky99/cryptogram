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
        CurrentMonth();
        
    }

    public int Month => _Month;

    private int _Month;

    private DayChallengeButton _Selected;
    private DayChallengeButton[] _DayButtons;

    [SerializeField] private Arrow_Button _LeftArrow;
    [SerializeField] private Arrow_Button _RightArrow;

    [SerializeField] private MonthText _MonthText;
    [SerializeField] private TextMeshProUGUI DaySelectionText;
    [SerializeField] private RectTransform _DayButtonsContainer;
    
    [SerializeField] private DayChallengeButton _DayChallengeButtonPrefab;

    private void clear()
    {
        _Selected = null;
        _Month = 0;

        if (_DayButtons == null)
            return;

        for (int i = 0; i < _DayButtons.Length; i++)
            Destroy(_DayButtons[i].gameObject);
    }

    private void setMonth(int year, int month)
    {
        clear();

        if (month < 1 || month > 12)
            Debug.LogError("Non valid month");

        _Month = month;
        int dayCount = DateTime.DaysInMonth(year, month);

        _DayButtons = new DayChallengeButton[dayCount];

        _MonthText.SetMonth(_Month);
        _GameManager.SetMonth(_Month);

        for (int i = 0; i < dayCount; i++)
        {
            DayChallengeButton instance = Instantiate(_DayChallengeButtonPrefab, _DayButtonsContainer);
            instance.Initialize(this, _Month, i);
            _DayButtons[i] = instance;
        }
        _LeftArrow.UpdateState();
        _RightArrow.UpdateState();
    }

    public void PreviousMonth()
    {
        int year = DateTime.Today.Year;
        int month = DateTime.Today.Month;

        bool firstMonth = month == 1;
        int previousMonth = firstMonth ? 12 : month - 1;
        int previousMonthYear = firstMonth ? year - 1 : year;

        setMonth(previousMonthYear, previousMonth);
    }
    public void CurrentMonth()
    {
        int year = DateTime.Today.Year;
        int month = DateTime.Today.Month;
        setMonth(year, month);
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