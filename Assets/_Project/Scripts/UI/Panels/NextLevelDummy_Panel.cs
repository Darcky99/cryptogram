using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NextLevelDummy_Panel : Singleton<NextLevelDummy_Panel>
{
    private GameManager _GameManager => GameManager.Instance;
    private MenuManager _MenuManager => MenuManager.Instance;

    #region Unity
    private void OnEnable()
    {
        GameManager.OnLevelCompleted += onLevelCompleted;
        GameManager.OnGameOver += onGameOver;
    }
    private void OnDisable()
    {
        GameManager.OnLevelCompleted -= onLevelCompleted;
        GameManager.OnGameOver -= onGameOver;
    }
    #endregion

    #region Callbacks
    private void onLevelCompleted()
    {
        switch (_GameManager.GameMode)
        {
            case eGameMode.HC:
                setCountDown("NEXT LEVEL IN: ", () => _GameManager.LoadLevel());
                break;
            case eGameMode.DC:
                setCountDown("GOING OUT IN: ", () => _MenuManager.OpenMainMenu());
                break;
            case eGameMode.TH:
                setCountDown("COMPLETE : ", () => {
                    if (_GameManager.IsThemeCompleted())
                        _MenuManager.OpenMainMenu();
                    else
                        _GameManager.LoadLevel();
                });
                break;
        }
        
    }
    private void onGameOver()
    {
        setCountDown("RESTARTING LEVEL IN: ", () => _GameManager.ResetLevel());
    }
    #endregion

    private WaitForSeconds _CountDownDelay = new WaitForSeconds(1f);

    [SerializeField] private RectTransform _Visuals;
    [SerializeField] private TextMeshProUGUI _Text;


    private void setTime(string message, int count)
    {
        _Text.text = $"{message} {count}s";
    }
    private IEnumerator countDown(string message, Action endAction)
    {
        _Visuals.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            setTime(message, i);
            yield return _CountDownDelay;
        }

        endAction();
        _Visuals.gameObject.SetActive(false);
    }

    private void setCountDown(string message, Action endAction) => StartCoroutine(countDown(message, endAction));
}