using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class SpawnTypeValues 
{
    public RoomType type;
    public Tilemap myTypeMap;
    public TileBase mybaseTyle;
    public Color UiColor;
    public int min;
    public int max;
}
