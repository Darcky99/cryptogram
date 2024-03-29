using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    #region Unity
    private void Awake()
    {
        _InactiviryTimer = new Timer(this);
        _LevelTimer = new Timer(this);
    }
    private void Start()
    {
        checkDate();
    }
    private void OnEnable()
    {
        InputManager.OnTouch += onTouch;
        GameManager.OnLoadLevel -= onLoadLevel;
        GameManager.OnLevelCompleted += onLevelCompleted;
        GameManager.OnGameOver += onGameOver;
    }
    private void OnDisable()
    {
        InputManager.OnTouch -= onTouch;
        GameManager.OnLoadLevel -= onLoadLevel;
        GameManager.OnLevelCompleted -= onLevelCompleted;
        GameManager.OnGameOver -= onGameOver;
    }
    #endregion

    #region Callbacks
    private void onTouch(Touch touch)
    {
        Debug.Log("ACTIVITY!");
        _InactiviryTimer.SetTimer();
    }

    private void onLoadLevel(int levelIndex)
    {
        _LevelTimer.SetTimer();
    }
    private void onLevelCompleted()
    {
        _LevelTimer.Stop();
    }
    private void onGameOver()
    {
        _LevelTimer.Stop();
    }
    #endregion

    public static event Action OnNewDay;

    private const string LAST_DATE = "gp_LastDate";

    private Timer _InactiviryTimer;
    private Timer _LevelTimer;

    private void checkDate()
    {
        DateTime today = DateTime.Today;
        DateTime lastLoggedDate = today;
        if (ES3.KeyExists(LAST_DATE))
            lastLoggedDate = ES3.Load<DateTime>(LAST_DATE);
        if (lastLoggedDate != today)
        {
            OnNewDay?.Invoke();
            Debug.Log("NEW DAY!");
        }
        ES3.Save(LAST_DATE, today);
    }
}