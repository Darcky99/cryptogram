using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelData
{
    public string Phrase { get; }
    public string Author { get; }

    public char[] PartiallyHiddenLetters { get; }
    public char[] HiddenLetters { get; }
}