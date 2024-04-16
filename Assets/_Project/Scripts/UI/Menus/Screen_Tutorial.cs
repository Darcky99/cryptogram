using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen_Tutorial : MonoBehaviour
{
    public bool IsButtonMessage => _TutorialMessages[0].gameObject.activeInHierarchy || _TutorialMessages[3].gameObject.activeInHierarchy;
    public bool IsMessage_5 => _TutorialMessages[4].gameObject.activeInHierarchy;


    [SerializeField] private RectTransform _Mask;
    [SerializeField] private RectTransform _Hand;
    [SerializeField] private RectTransform[] _TutorialMessages;

    private void disableAll()
    {
        foreach (RectTransform message in _TutorialMessages)
            message.gameObject.SetActive(false);
    }

    private void enableMessage(int index)
    {
        if (_TutorialMessages[index].gameObject.activeInHierarchy)
            return;

        _Hand.gameObject.SetActive(false);
        _Mask.gameObject.SetActive(false);
        for (int i = 0; i < _TutorialMessages.Length; i++)
            _TutorialMessages[i].gameObject.SetActive(i == index);
    }

    public void DisableAll() => disableAll();
    public void SetHand(Vector2 position)
    {
        _Hand.gameObject.SetActive(true);
        _Hand.position = position;
    }
    public void EnableMessage(int index) => enableMessage(index);
    public void SetMask(bool enable) => _Mask.gameObject.SetActive(enable);
}