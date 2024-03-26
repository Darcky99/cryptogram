using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelData
{
    public string Phrase { get; }
    public string Author { get; }
    public List<char> PartiallyHiddenLetters { get; }
    public List<char> HidenLetters { get; }
}