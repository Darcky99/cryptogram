using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HintPanel : Singleton<HintPanel>
{
    public static event Action<bool> OnHintPanel;

    [SerializeField] private RectTransform _HintInterface;

    [SerializeField] private RectTransform _OpenHintInterfaceButton;
    [SerializeField] private RectTransform _CloseHintInterfaceButton;
    [SerializeField] private TextMeshProUGUI _Counter;

    private void setHintPanel(bool condition)
    {
        _HintInterface.gameObject.SetActive(condition);
        _OpenHintInterfaceButton.gameObject.SetActive(!condition);
        _CloseHintInterfaceButton.gameObject.SetActive(condition);
        OnHintPanel?.Invoke(condition);
    }

    public void SetHintCount(int count)
    {
        _Counter.text = count.ToString();
        setHintPanel(false);
    }

    public void OpenHintPanel() => setHintPanel(true);
    public void CloseHintPanel() => setHintPanel(false);
}