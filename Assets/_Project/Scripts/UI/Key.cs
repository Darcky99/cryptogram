using System;
using UnityEngine.UI;
using UnityEngine;

public class Key : MonoBehaviour
{
    #region Unity
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
    private void onLoadLevel(int levelIndex)
    {
        _Button.interactable = true;
    }
    private void onGameOver()
    {
        _Button.interactable = false;
    }
    #endregion

    public char KeyLetter => gameObject.name[0];
    [SerializeField] private Button _Button;

    public void TypeKey() => PhraseManager.Instance.TrySetLetter(KeyLetter);

    public void SetInteractuable() => _Button.interactable = true;
    public void SetNonInteractuable() => _Button.interactable = false;
}