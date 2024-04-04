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

    [SerializeField] private Image _BackgroundImge;
    [SerializeField] private TextMeshProUGUI _Number;

    //ill need to set a number and then on presed let the screencallendar my number

    public void Initialize(Screen_Callendar screen, int levelIndex) 
    {
        _ScreenCallendar = screen;
        _LevelIndex = levelIndex;
        _Number.text = (_LevelIndex + 1).ToString();
    }
    public void OnButtonDown()
    {
        _BackgroundImge.color = _SelectedColor;
        _ScreenCallendar.SetSelection(this);
    }

    public void Deselect()
    {
        _BackgroundImge.color = Color.white;
    }
}