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
    private void Start()
    {
        levelText();
    }
    #endregion

    private TextMeshProUGUI _LevelText;

    private void levelText()
    {
        _LevelText.text = $"LEVEL {/*_StorageManager.GetLevelIndex(eLevelsCollection.HC)*/ + 1}";
    }

    public void PlayHCLevels()
    {
        _MenuManager.ChangeScreenState(eScreen.MainMenu, false);
        _MenuManager.ChangeScreenState(eScreen.Gameplay, true);

        _GameManager.PlayHCLevels();
    }
}