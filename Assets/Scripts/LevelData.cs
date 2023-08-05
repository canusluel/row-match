using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public int gridWidth;
    public int gridHeight;
    public int moveCount;
    public List<char> gridData;
}