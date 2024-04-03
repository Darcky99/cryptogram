using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class MenuScreenBase : MonoBehaviour
{
    public void SetActive(bool condition) => gameObject.SetActive(condition);
}