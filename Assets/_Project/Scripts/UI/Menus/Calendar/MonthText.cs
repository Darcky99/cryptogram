using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonthText : MonoBehaviour
{
    private void Awake()
    {
        _MonthText = GetComponent<TextMeshProUGUI>();
    }

    private TextMeshProUGUI _MonthText;

    public void SetMonth(int month)
    {
        switch (month)
        {
            case 1:
                _MonthText.text = "January";
                break;
            case 2:
                _MonthText.text = "February";
                break;
            case 3:
                _MonthText.text = "March";
                break;
            case 4:
                _MonthText.text = "April";
                break;
            case 5:
                _MonthText.text = "May";
                break;
            case 6:
                _MonthText.text = "June";
                break;
            case 7:
                _MonthText.text = "July";
                break;
            case 8:
                _MonthText.text = "August";
                break;
            case 9:
                _MonthText.text = "September";
                break;
            case 10:
                _MonthText.text = "October";
                break;
            case 11:
                _MonthText.text = "November";
                break;
            case 12:
                _MonthText.text = "December";
                break;
            default:
                _MonthText.text = "Invalid Month";
                break;
        }
    }
}