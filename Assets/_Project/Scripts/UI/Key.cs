using System;
using UnityEngine.UI;
using UnityEngine;

public class Key : MonoBehaviour
{
    #region Unity
    private void Awake()
    {
        _KeyLetter = gameObject.name[0];
    }
    private void OnEnable()
    {
        GameManager.OnLoadLevel += onLoadLevel;
        GameManager.OnGameOver += onGameOver;
    }
    private void OnDisable()
    {
        GameManager.OnLoadLevel -= onLoadLevel;
        GameManager.OnGameOver -= onGameOver;
    }
    #endregion

    #region Callbacks
    private void onLoadLevel(int obj)
    {
        _Button.interactable = true;
    }
    private void onGameOver()
    {
        _Button.interactable = false;
    }
    #endregion

    private char _KeyLetter;
    [SerializeField] private Button _Button;

    public void TypeKey() => PhraseManager.Instance.TrySetLetter(_KeyLetter);

    public void SetInteractuable() => _Button.interactable = true;
    public void SetNonInteractuable() => _Button.interactable = false;
}