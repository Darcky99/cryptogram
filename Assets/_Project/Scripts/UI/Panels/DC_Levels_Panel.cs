using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Levels_Panel : MonoBehaviour
{
    private MenuManager _MenuManager => MenuManager.Instance;

    public void PlayHCLevels()
    {
        _MenuManager.ChangeScreenState(eScreen.MainMenu, false);
        _MenuManager.ChangeScreenState(eScreen.DailyChallengeCalendar, true);
    }
}