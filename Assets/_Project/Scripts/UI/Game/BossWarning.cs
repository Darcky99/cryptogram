using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using DG.Tweening;

public class BossWarning : Singleton<BossWarning>
{
    [SerializeField] private TextMeshProUGUI _Text;

    public void TriggerBossWarning()
    {
        _Text.gameObject.SetActive(true);
        _Text.rectTransform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(_Text.rectTransform.DOScale(1, 0.7f).SetEase(Ease.InSine))
            .Join(DOVirtual.Float(0, 1, 0.7f, (float value) => _Text.alpha = value).SetEase(Ease.InSine))
            .OnComplete(() => DOVirtual.DelayedCall(1f, () => _Text.gameObject.SetActive(false)));
    }
}

[CustomEditor(typeof(BossWarning))]
public class BossWarningEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BossWarning bossWarning = (BossWarning)target;

        if (GUILayout.Button("Trigger Boss Warning Animation"))
            bossWarning.TriggerBossWarning();
    }

}