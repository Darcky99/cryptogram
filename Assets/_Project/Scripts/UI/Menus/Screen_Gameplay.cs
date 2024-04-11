using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen_Gameplay : MenuScreenBase
{
    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
    }
    private void OnEnable()
    {
        _CanvasGroup.alpha = 0;
        DOVirtual.DelayedCall(0.1f, () => 
        {
            DOVirtual.Float(0, 1, 1.6f, (float value) => _CanvasGroup.alpha = value);
        });
    }

    private CanvasGroup _CanvasGroup;
}