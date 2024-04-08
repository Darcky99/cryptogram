using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NextLevelDummy_Panel : Singleton<NextLevelDummy_Panel>
{
    #region Unity
    private void OnEnable()
    {
        GameManager.OnLevelCompleted += onLevelCompleted;
        GameManager.OnGameOver += onGameOver;
    }
    private void OnDisable()
    {
        GameManager.OnLevelCompleted += onLevelCompleted;
        GameManager.OnGameOver -= onGameOver;
    }
    #endregion

    #region Callbacks
    private void onLevelCompleted()
    {
        StartCountDown("NEXT LEVEL IN: ", () => Debug.Log("NEX LEVEL FUNCTION NOT CREATED!") /*GameManager.Instance.LoadLevel()*/);
    }
    private void onGameOver()
    {
        StartCountDown("RESTARTING LEVEL IN: ", () => GameManager.Instance.ResetLevel());
    }
    #endregion

    private WaitForSeconds _CountDownDelay = new WaitForSeconds(1f);

    [SerializeField] private RectTransform _Visuals;
    [SerializeField] private TextMeshProUGUI _Text;

    private void setTime(string message, int count)
    {
        _Text.text = $"{message} {count}s";
    }
    private IEnumerator loadLevelCountDown(string message, Action endAction)
    {
        for(int i = 3; i > 0; i--)
        {
            setTime(message, i);
            yield return _CountDownDelay;
        }
        endAction();
        _Visuals.gameObject.SetActive(false);
    }

    public void StartCountDown(string message, Action endAction)
    {
        _Visuals.gameObject.SetActive(true);
        StartCoroutine(loadLevelCountDown(message, endAction));
    }
}