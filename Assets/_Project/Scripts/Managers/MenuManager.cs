using System;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    public Screen_MainMenu MainMenuScreen => _MainMenuScreen;
    public Screen_Gameplay GameplayScreen => _GameplayScreen;
    public Screen_Tutorial TutorialScreen => _TutorialScreen;

    [SerializeField] private Screen_MainMenu _MainMenuScreen;
    [SerializeField] private Screen_Gameplay _GameplayScreen;
    [SerializeField] private Screen_Tutorial _TutorialScreen;

    protected override void OnAwakeEvent()
    {
        base.OnAwakeEvent();
    }

    private void openGameplay()
    {
        _MainMenuScreen.gameObject.SetActive(false);
        _GameplayScreen.gameObject.SetActive(true);
    }
    private void openMainMenu()
    {
        _GameplayScreen.gameObject.SetActive(false);
        _TutorialScreen.gameObject.SetActive(false);
        _MainMenuScreen.gameObject.SetActive(true);
    }

    public void OpenGameplay() => openGameplay();
    public void OpenMainMenu() => openMainMenu();
}