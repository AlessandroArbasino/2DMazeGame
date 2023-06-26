using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector2 gridPos;
    public CellType myCellType;
    public RoomType roomType;
    public bool doorTop, doorBot, doorleft, doorRight;
   
    public Room(Vector2 _gridPos, RoomType roomType, CellType myCellType)
    {
        gridPos = _gridPos;
        this.roomType = roomType;
        this.myCellType = myCellType;
    }
}
