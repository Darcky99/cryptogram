using System;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    public static event Action<int> OnLoadLevel;
    public static event Action OnLevelCompleted;
    public static event Action OnGameOver;

    #region Unity
    public override void Start()
    {
        RemoteLoad.GetTestLevels();
        //Invoke(nameof(LoadLevel), 0.25f);
    }
    private void OnEnable()
    {
        InputManager.OnKeyDown += onKeyDown;
    }
    private void OnDisable()
    {
        InputManager.OnKeyDown -= onKeyDown;
    }
    #endregion

    #region Callbacks
    private void onKeyDown(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.R:
                LoadLevel();
                break;
            case KeyCode.Q:
                _LevelIndex--;
                if (_LevelIndex < 0)
                    _LevelIndex = 0;
                LoadLevel();
                break;
            case KeyCode.E:
                _LevelIndex++;
                LoadLevel();
                break;
        }
    }
    #endregion

    #region Pause & Resume
    private void pause() => Time.timeScale = 0;
    private void resume() => Time.timeScale = 1;
    #endregion

    #region Game loop
    public int LevelIndex => _LevelIndex;

    private int _LevelIndex = 0;

    public void LoadLevel() => OnLoadLevel?.Invoke(_LevelIndex);
    public void LevelCompleted() => OnLevelCompleted?.Invoke();
    public void GameOver() => OnGameOver?.Invoke();

    public void SetLevelIndex(int levelIndex)
    {
        _LevelIndex = levelIndex;
        LoadLevel();
    }
    #endregion

    #region Save & Load NOT DONE YET!
    public void SaveGame() { }
    public void LoadGame() { }
    public void DeleteGame() { }
    #endregion
}