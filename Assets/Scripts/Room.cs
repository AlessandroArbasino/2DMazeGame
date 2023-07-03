using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Room
{
    public Vector2 gridPos;
    public CellType myCellType;
    public RoomType roomType;
    public bool doorTop, doorBot, doorleft, doorRight;
    public List<DoorTypes> doors;

    public bool IsDefinitive;
    public Room(Vector2 _gridPos, bool isDefinitive)
    {
        gridPos = _gridPos;
        doors = new List<DoorTypes>();
        IsDefinitive = isDefinitive;
    }

    public void FillDoorsList()
    {
        if (doorTop)
            doors.Add(DoorTypes.TopDoor);

        if (doorBot)
            doors.Add(DoorTypes.BottomDoor);

        if (doorleft)
            doors.Add(DoorTypes.LeftDoor);

        if (doorRight)
            doors.Add(DoorTypes.RightDoor);
    }

    public void SetRoomType(RoomType roomType, CellType myCellType)
    {
        this.roomType = roomType;
        this.myCellType = myCellType;
    }

}
