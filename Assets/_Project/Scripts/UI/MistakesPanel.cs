using UnityEngine;
using DG.Tweening;

public class MistakesPanel : Singleton<MistakesPanel>
{
    #region Unity
    private void OnEnable()
    {
        GameManager.OnLoadLevel += onLevelLoaded;
    }
    private void OnDisable()
    {
        GameManager.OnLoadLevel -= onLevelLoaded;
    }
    #endregion

    #region Callbacks
    private void onLevelLoaded(int levelIndex)
    {
        _CrossAnimation?.Kill();
    }
    #endregion

    private Tween _CrossAnimation;

    [SerializeField] private MistakeDot[] _MistakeDots;

    public void DisplayMistakeCount(int count)
    {
        for(int i = 0; i < _MistakeDots.Length; i++)
        {
            bool condition = count > i;
            MistakeDot current = _MistakeDots[i];
            RectTransform cross = _MistakeDots[i].CrossImage.rectTransform;
            cross.gameObject.SetActive(condition);
            if (!condition)
                continue;
            cross.localScale = Vector3.zero;
            Sequence sequence = DOTween.Sequence();
            float scaleUpDuration = 0.25f;
            float scaleDownDuration = 0.15f;
            sequence
                .Append(cross.DOScale(1.2f, scaleUpDuration).SetEase(Ease.InSine))
                .Join(DOVirtual.Float(0f, 1f, scaleUpDuration, (float value) =>
                {
                    Color color = current.CrossImage.color;
                    color.a = value;
                    current.CrossImage.color = color;
                }).SetEase(Ease.InSine))
                .Append(cross.DOScale(1.0f, scaleDownDuration).SetEase(Ease.OutSine));
            _CrossAnimation = sequence;
        }
    }
}