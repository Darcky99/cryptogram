using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DayChallengeButton : MonoBehaviour
{
    public int LevelIndex => _LevelIndex;

    private Screen_Callendar _ScreenCallendar;
    private int _LevelIndex;

    [SerializeField] private Color _SelectedColor;

    [SerializeField] private RectTransform _DayButton;
    [SerializeField] private RectTransform _Padklock;

    [SerializeField] private TextMeshProUGUI _Number;
    [SerializeField] private Image _BackGroundImage;

    public void Initialize(Screen_Callendar screen, int levelIndex, bool show) 
    {
        //somewhere I have to deside if it shows or now.
        _DayButton.gameObject.SetActive(show);
        _Padklock.gameObject.SetActive(!show);

        _ScreenCallendar = screen;
        _LevelIndex = levelIndex;
        _Number.text = (_LevelIndex + 1).ToString();
    }
    public void OnButtonDown()
    {
        _BackGroundImage.color = _SelectedColor;
        _ScreenCallendar.SetSelection(this);
    }

    public void Deselect()
    {
        _BackGroundImage.color = Color.white;
    }
}