using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MistakeDot : MonoBehaviour
{
    public RectTransform RectTransform => _RectTransform;
    public Image CrossImage => _CrossImage;

    [SerializeField] private RectTransform _RectTransform;
    [SerializeField] private Image _CrossImage;
}