using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelData
{
    public string Phrase { get; }
    public string Author { get; }

    public char[] PartiallyHidden { get; }
    public char[] Hidden { get; }
}