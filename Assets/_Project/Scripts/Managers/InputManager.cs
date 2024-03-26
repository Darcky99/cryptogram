using System;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public static event Action<Touch> OnTouch;
    public static event Action<KeyCode> OnKeyDown;

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        Touch[] touches = InputWrapper.Input.touches;
        if (touches.Length != 0)
            OnTouch?.Invoke(touches[0]);
        checkKeys();
    }
    private void checkKeys()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.R))
            OnKeyDown?.Invoke(KeyCode.R);
        if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
            OnKeyDown?.Invoke(KeyCode.Q); 
        if (UnityEngine.Input.GetKeyDown(KeyCode.E))
            OnKeyDown?.Invoke(KeyCode.E);
    }
}