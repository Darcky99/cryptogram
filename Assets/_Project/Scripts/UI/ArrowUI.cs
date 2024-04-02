using UnityEngine;
using UnityEngine.UI;

public class ArrowUI : MonoBehaviour
{
    #region Unity
    private void OnEnable()
    {
        GameManager.OnLoadLevel += onLoadLevel;
        GameManager.OnGameOver += onGameOver;
    }
    private void OnDisable()
    {
        GameManager.OnLoadLevel -= onLoadLevel;
        GameManager.OnGameOver -= onGameOver;
    }
    #endregion

    #region Callbacks
    private void onLoadLevel(ILevelData levelData)
    {
        _Button.interactable = true;
    }
    private void onGameOver()
    {
        _Button.interactable = false;
    }
    #endregion

    [SerializeField] private eChangeSelectionMode _ChangeSelectionMode;
    [SerializeField] private Button _Button;

    public void ChangeSelection() => PhraseManager.Instance.ChangeSelection(_ChangeSelectionMode);
}