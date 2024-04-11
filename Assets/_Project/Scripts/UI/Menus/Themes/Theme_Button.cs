using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Theme_Button : MonoBehaviour
{
    private MenuManager _MenuManager => MenuManager.Instance;
    private GameManager _GameManager => GameManager.Instance;

    private string _Theme => _TitleText.text;

    private int _LevelCount;

    [SerializeField] private TextMeshProUGUI _TitleText;
    [SerializeField] private TextMeshProUGUI _ProgressText;

    private void setTheme(string theme)
    {
        _TitleText.text = theme;
        _LevelCount = _GameManager.GetThemeLevelsCount(_Theme);

        updateProgress();
    }
    private void updateProgress()
    {
        _ProgressText.text = $"{_GameManager.ThemesProgress[_Theme].LevelIndex + 1} / {_LevelCount}";
    }

    public void SetTheme(string theme) => setTheme(theme);
    public void UpdateProgress() => updateProgress();


    public void OnButtonDown()
    {
        _MenuManager.ChangeScreenState(eScreen.TH_Selection, false);
        _MenuManager.ChangeScreenState(eScreen.MainMenu, false);
        _MenuManager.ChangeScreenState(eScreen.Gameplay, true);

        _GameManager.PlayThemeLevels(_Theme);
    }
}