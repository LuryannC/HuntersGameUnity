using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TreasureType
{
    None,
    Location,
    Art,
    BusStop,
}

[System.Serializable]
public struct TreasureStructure
{
    public string displayName;
    public TreasureType treasureType;
    public string[] coordsString;
    [HideInInspector] public Texture2D treasureIcon;
    [HideInInspector] public Mesh treasureMesh;
    public float collectDistance;
}
