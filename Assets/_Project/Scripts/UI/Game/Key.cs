using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

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
    private void onLoadLevel(ILevelData levelData)
    {
        setInteraction(true);
        setHint(false);
    }
    private void onGameOver()
    {
        setInteraction(false);
    }
    #endregion

    public RectTransform RectTransform => _Text.rectTransform;
    public char KeyLetter => gameObject.name[0];

    [SerializeField] private Button _Button;
    [SerializeField] private TextMeshProUGUI _Text;

    [SerializeField] private Color _HintColor;

    private void setInteraction(bool enable) 
    {
        _Button.interactable = enable;
        _Text.color = Color.black;
    }
    private void setHint(bool enable) => _Text.color = enable ? _HintColor : Color.black;

    public void TypeKey() => PhraseManager.Instance.TrySetLetter(KeyLetter);

    public void SetInteraction(bool enable) => setInteraction(enable);
    public void SetHint(bool enable) => setHint(enable);
}