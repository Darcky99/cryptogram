using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen_Themes : MonoBehaviour
{
    private void OnEnable()
    {
        populateThemeSelection();
    }

    [SerializeField] private RectTransform _Panel;
    [SerializeField] private RectTransform _Container;
    [SerializeField] private Theme_Button _ThemeButtonPrefab;

    private void populateThemeSelection()
    {
        //for each theme json we find we need to create one button
    }
}