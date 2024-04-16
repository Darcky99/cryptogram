using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMask : MonoBehaviour
{
    private void Awake()
    {
        _RectTranform = GetComponent<RectTransform>();
    }

    private RectTransform _RectTranform;

    public void SetPosition(Vector3 position) => _RectTranform.position = position;
}