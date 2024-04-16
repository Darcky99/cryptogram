using DG.Tweening;
using UnityEngine;

public class AnimatorBase : MonoBehaviour
{
		
	public bool m_IsPlaying;
	public Tween Tween;

	[Space]

	public bool PlayOnEnable;
	public bool RandomStartTime = false;
	[Range(0, 1)] public float StartTime = 0;

	[Space]
	public bool UseDotweenID = false;

	public int Id = k_NoId;
	protected const int k_NoId = -9999;
	//[ShowIf(nameof(UseDotweenID)), ValueDropdown(nameof(DotweenIDs))]
	//public int DoTweenID = DOTweenIDs.Default;

	//protected int FinalID { get => UseDotweenID ? DoTweenID : Id; }


	public float Duration = 1;
	public Ease Ease = Ease.Linear;
	public bool IsRelative;

	[Space]

	public UpdateType UpdateType = UpdateType.Normal;
	public bool UpdateFrameIndependant = false;

	[Space]

	public int LoopCount = 0;
	private bool m_UsingLoops => LoopCount > 0 || LoopCount == -1;
	public LoopType LoopType;

	protected virtual void Awake()
	{
		if (RandomStartTime)
		{
			StartTime = Random.Range(0.0f, 1.0f);
		}
	}

	protected virtual void Start()
	{
		SetRestartAction();
	}

	protected virtual void SetRestartAction()
	{
	}

	protected virtual void OnEnable()
	{
		//GameManager.OnGameReset += OnGameReset;

		ResetValues();
		if (PlayOnEnable)
			StartAnimation();
	}

	protected virtual void OnDisable()
	{
		//GameManager.OnGameReset -= OnGameReset;

		StopAnimation();
	}

	protected virtual void OnGameReset()
    {
		//if (!DOTWeenIDsBase.IdsBase.Contains(FinalID))
		//{
		//	ResetValues();
		//	//Debug.LogError(FinalID + "		" + DOTWeenIDsBase.IdsBase.Contains(FinalID), gameObject);
		//}
	}

	public virtual void ResetValues()
	{
		StopAnimation();
	}

	public virtual void StartAnimation()
	{
		Tween?.Kill(true);
		m_IsPlaying = true;
	}

	public virtual void StopAnimation()
	{
		m_IsPlaying = false;
		Tween?.Kill(true);
	}

	public virtual void RestartAnimation()
	{
		ResetValues();
		StartAnimation();
	}

	public virtual void PauseAnimation()
    {
		Tween?.Pause();
    }

	public void ContinueAnimation()
    {
		Tween?.Play();
    }

	public virtual void SetId(Tween i_Tween)
    {
		//if (FinalID != k_NoId)
		//{
		//	if (i_Tween != null)
		//	{
		//		i_Tween.SetId(FinalID);
		//	}
		//}
    }
}