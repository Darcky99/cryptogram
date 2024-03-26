using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LifePanel : Singleton<LifePanel>
{
    [SerializeField] private TextMeshProUGUI _Counter;

    public void setLifeCount(int count)
    {
        _Counter.text = count.ToString();
    }
}