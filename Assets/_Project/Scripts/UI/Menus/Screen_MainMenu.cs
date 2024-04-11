using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen_MainMenu : MenuScreenBase
{
    [SerializeField] private RectTransform _Calendar;
    [SerializeField] private RectTransform _ThemeSelection;

    private void disableAllPanels()
    {
        _Calendar.gameObject.SetActive(false);
        _ThemeSelection.gameObject.SetActive(false);
    }

    public void DisableAllPanels() => disableAllPanels();
    public void OpenCalendar()
    {
        disableAllPanels();
        _Calendar.gameObject.SetActive(true);
    }
    public void OpenThemeSelection()
    {
        disableAllPanels();
        _ThemeSelection.gameObject.SetActive(true);
    }
}