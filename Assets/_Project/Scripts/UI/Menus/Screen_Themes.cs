using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Screen_Themes : MenuScreenBase
{
    private MenuManager _MenuManager => MenuManager.Instance;
    private GameManager _GameManager => GameManager.Instance;

    private void Start()
    {
        populateThemeSelection();
    }

    [SerializeField] private RectTransform _Panel;
    [SerializeField] private RectTransform _Container;
    [SerializeField] private Theme_Button _ThemeButtonPrefab;

    private const string _THEMES_PATH = "Assets/_Project/Documents/JSON LEVELS/TH";

    private void populateThemeSelection()
    {
        Dictionary<string, ContinousProgress> themesProgress = _GameManager.ThemesProgress;
        string[] themes = _GameManager.Themes;

        for (int i = 0; i < themesProgress.Count; i++)
        {
            string theme = themes[i];
            Instantiate(_ThemeButtonPrefab, _Container).Inititialize(themes[i], $"{_GameManager.ThemesProgress[theme].LevelIndex + 1} / {_GameManager.GetThemeLevelsCount(theme)}");
        }
    }

    public void CloseMenu()
    {
        _MenuManager.ChangeScreenState(eScreen.TH_Selection, false);
    }
}