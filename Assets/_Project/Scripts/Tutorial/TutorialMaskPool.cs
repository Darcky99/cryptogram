using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TutorialMaskPool : Pool<TutorialMask>
{
    public void ResetAll()
    {
        TutorialMask[] tutorialMask = GetComponentsInChildren<TutorialMask>(true);
        foreach(TutorialMask mask in tutorialMask)
            mask.gameObject.SetActive(false);
    }
}