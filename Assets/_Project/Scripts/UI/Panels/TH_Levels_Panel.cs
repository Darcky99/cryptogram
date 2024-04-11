using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TH_Levels_Panel : MonoBehaviour
{
    private MenuManager _MenuManager => MenuManager.Instance;

    public void OpenTHScreen()
    {
        _MenuManager.ChangeScreenState(eScreen.TH_Selection, true);
    }
}