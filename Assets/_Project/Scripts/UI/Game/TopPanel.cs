using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopPanel : MonoBehaviour
{
    #region Unity
    private void OnEnable()
    {
        HintPanel.OnHintPanel += onHintPanel;
    }
    private void OnDisable()
    {
        HintPanel.OnHintPanel -= onHintPanel;
    }
    #endregion

    private void onHintPanel(bool active)
    {
        _TopBackground.SetActive(active);
    }

    [SerializeField] private GameObject _TopBackground;
}