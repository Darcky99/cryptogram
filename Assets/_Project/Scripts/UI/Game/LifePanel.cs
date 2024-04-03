using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LifePanel : Singleton<LifePanel>
{
    private GameManager _GameManager => GameManager.Instance;

    [SerializeField] private TextMeshProUGUI _Counter;

    private void OnEnable()
    {
        SetLifeCount(_GameManager.Lifes);
    }

    public void SetLifeCount(int count)
    {
        _Counter.text = count.ToString();
    }
}