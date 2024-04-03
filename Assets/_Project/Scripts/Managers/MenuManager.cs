using System;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] private MenuScreenBase _MainMenu;
    [SerializeField] private MenuScreenBase _DailyChanllengeCallendar;
    [SerializeField] private MenuScreenBase _Gameplay;

    protected override void OnAwakeEvent()
    {
        base.OnAwakeEvent();
    }

    private void changeScreenState(eScreen screen, bool condition)
    {
        switch (screen)
        {
            case eScreen.MainMenu:
                _MainMenu.SetActive(condition);
                break;
            case eScreen.DailyChallengeCalendar:
                _DailyChanllengeCallendar.SetActive(condition);
                break;
            case eScreen.Gameplay:
                _Gameplay.SetActive(condition);
                break;
        }
    }

    public void ChangeScreenState(eScreen screen, bool condition) => changeScreenState(screen, condition);
}