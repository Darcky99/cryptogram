using UnityEngine;
using System.Collections;

public class Timer
{
    public Timer(MonoBehaviour coroutineOwner)
    {
        _CoroutineOwner = coroutineOwner;
    }

    private MonoBehaviour _CoroutineOwner;
    private Coroutine _TimerCoroutine;

    private float _Time;

    private void setTimer()
    {
        _Time = 0f;
        if (_TimerCoroutine == null)
            _TimerCoroutine = _CoroutineOwner.StartCoroutine(countSeconds());
    }
    private void stop()
    {
        if (_TimerCoroutine != null)
        {
            _CoroutineOwner.StopCoroutine(_TimerCoroutine);
            _TimerCoroutine = null;
        }
    }

    /// <summary>
    /// Starts the count from 0
    /// </summary>
    public void SetTimer() => setTimer();
    public void Stop() => stop();

    private IEnumerator countSeconds()
    {
        while (true)
        {
            _Time += Time.deltaTime;
            yield return null;
        }
    }
}