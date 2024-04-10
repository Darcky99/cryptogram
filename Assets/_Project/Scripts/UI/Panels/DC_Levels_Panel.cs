using TMPro;
using System;
using UnityEngine;

public class DC_Levels_Panel : MonoBehaviour
{
    private MenuManager _MenuManager => MenuManager.Instance;

    private void Start()
    {
        initialize();
    }

    [SerializeField] private TextMeshProUGUI _DayText;
    [SerializeField] private MonthText _MonthText;

    private void initialize()
    {
        DateTime today = DateTime.Today;

        string dayString = today.Day <= 9 ? $"0{today.Day}" : today.Day.ToString();

        _DayText.text = dayString;
        _MonthText.SetMonth(today.Month);
    }


    public void OpenDCCallendar()
    {
        _MenuManager.ChangeScreenState(eScreen.MainMenu, false);
        _MenuManager.ChangeScreenState(eScreen.DC_Calendar, true);
    }
}