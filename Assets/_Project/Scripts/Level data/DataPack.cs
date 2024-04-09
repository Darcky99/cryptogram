using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataPack
{
    public DataPack(string path)
    {

    }

    public int Version => _Version;
    public LevelData[] Pack => _Pack;

    private int _Version;
    private LevelData[] _Pack;

}