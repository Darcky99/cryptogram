using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Theme_Button : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _TitleText;
    [SerializeField] private TextMeshProUGUI _ProgressText;

    private void initialize(string title, string progress)
    {
        _TitleText.text = title;
        _ProgressText.text = progress;
    }

    public void Inititialize(string title, string progress) => initialize(title, progress);
}