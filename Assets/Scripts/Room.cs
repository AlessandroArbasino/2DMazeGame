using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector2 gridPos;
    public RoomType roomType;
    public int type;
    public bool doorTop, doorBot, doorleft, doorRight;
   
    public Room(Vector2 _gridPos, RoomType roomType)
    {
        gridPos = _gridPos;
        this.roomType = roomType;
    }
}
