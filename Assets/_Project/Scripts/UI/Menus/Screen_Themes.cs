using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Screen_Themes : MenuScreenBase
{
    private MenuManager _MenuManager => MenuManager.Instance;
    private GameManager _GameManager => GameManager.Instance;

    #region Unity
    private void Awake()
    {
        populateThemeSelection();
    }
    private void OnEnable()
    {
        updateProgress();
    }
    #endregion

    private Theme_Button[] _ThemeButtons;

    [SerializeField] private RectTransform _Panel;
    [SerializeField] private RectTransform _Container;
    [SerializeField] private Theme_Button _ThemeButtonPrefab;

    private void populateThemeSelection()
    {
        Dictionary<string, ContinousProgress> themesProgress = _GameManager.TH_Levels_Progress;
        string[] themes = _GameManager.Themes;
        _ThemeButtons = new Theme_Button[themes.Length];

        for (int i = 0; i < themesProgress.Count; i++)
        {
            string theme = themes[i];
            _ThemeButtons[i] = Instantiate(_ThemeButtonPrefab, _Container);
            _ThemeButtons[i].SetTheme(theme);
        }
    }

    private void updateProgress()
    {
        for (int i = 0; i < _ThemeButtons.Length; i++)
            _ThemeButtons[i].UpdateProgress();
    }

    public void CloseMenu()
    {
        _MenuManager.MainMenuScreen.DisableAllPanels();
    }
}