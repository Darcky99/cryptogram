using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HintPanel : Singleton<HintPanel>
{
    private GameManager _GameManager => GameManager.Instance;
    private PhraseManager _PhraseManager => PhraseManager.Instance;

    #region Unity
    private void OnEnable()
    {
        setHintCount(_GameManager.Hints);
    }
    #endregion

    public static event Action<bool> OnHintPanel;

    public bool IsHintInterfaceEnabled => _HintInterface.gameObject.activeInHierarchy;

    [SerializeField] private RectTransform _HintInterface;

    [SerializeField] private RectTransform _BuyHintInterfaceButton;
    [SerializeField] private RectTransform _OpenHintInterfaceButton;
    [SerializeField] private RectTransform _CloseHintInterfaceButton;
    [SerializeField] private TextMeshProUGUI _Counter;

    private void setHintPanel(bool condition)
    {
        OnHintPanel?.Invoke(condition);

        if (condition)
            _PhraseManager.ClearSelection();

        _HintInterface.gameObject.SetActive(condition);

        _BuyHintInterfaceButton.gameObject.SetActive(!condition && _GameManager.Hints <= 0);
        _OpenHintInterfaceButton.gameObject.SetActive(!condition && _GameManager.Hints > 0);

        _CloseHintInterfaceButton.gameObject.SetActive(condition);
    }
    private void setHintCount(int count)
    {
        _Counter.text = count.ToString();
        setHintPanel(false);
    }

    public void SetHintCount(int count) => setHintCount(count);

    public void BuyHint() => _GameManager.EarnHint();
    public void OpenHintPanel() => setHintPanel(_GameManager.GameState == eGameState.Playing);
    public void CloseHintPanel() => setHintPanel(false);
}