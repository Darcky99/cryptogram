using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HC_Levels_Panel : MonoBehaviour
{
    private GameManager _GameManager => GameManager.Instance;
    private MenuManager _MenuManager => MenuManager.Instance;
    private StorageManager _StorageManager => StorageManager.Instance;

    #region Unity
    private void Awake()
    {
        _LevelText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        StartCoroutine(updateLevelText());
    }
    #endregion

    private TextMeshProUGUI _LevelText;

    private IEnumerator updateLevelText()
    {
        yield return null;
        levelText();
    }
    private void levelText()
    {
        _LevelText.text = $"LEVEL {_GameManager.HC_Index + 2}";
    }

    public void PlayHCLevels()
    {
        _MenuManager.OpenGameplay();
        _GameManager.PlayHC();
    }
}