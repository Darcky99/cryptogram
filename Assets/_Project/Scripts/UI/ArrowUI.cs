using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowUI : MonoBehaviour
{
    [SerializeField] public eChangeSelectionMode ChangeSelectionMode;

    public void ChangeSelection() => PhraseManager.Instance.ChangeSelection(ChangeSelectionMode);
}