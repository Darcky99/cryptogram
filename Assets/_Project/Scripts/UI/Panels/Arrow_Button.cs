using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Button : MonoBehaviour
{
    private Screen_Callendar _Screen_Callendar => MenuManager.Instance.MainMenuScreen.Callendar;

    [SerializeField] private eDirection _Direction;
    [SerializeField] private GameObject _Button;


    private void updateState()
    {
        bool present = _Screen_Callendar.Month == DateTime.Today.Month;

        switch (_Direction)
        {
            case eDirection.Left:
                _Button.SetActive(present);
                break;
            case eDirection.Right:
                _Button.SetActive(!present);
                break;
        }
    }

    public void UpdateState() => updateState();

    public void OnButtonDown()
    {
        switch (_Direction)
        {
            case eDirection.Left:
                _Screen_Callendar.PreviousMonth();
                break;
            case eDirection.Right:
                _Screen_Callendar.CurrentMonth();
                break;
        }
        updateState();
    }
}