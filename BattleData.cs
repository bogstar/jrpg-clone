using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BattleData {

    public string id;

    public string[] enemies;
    public int gridSizeX;
    public int gridSizeY;
    public int numberOfRocks;
    public int numberOfHills;
    public int numberOfPaths;
}
