using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector2 gridPos;
    public CellType myCellType;
    public RoomType roomType;
    public bool doorTop, doorBot, doorleft, doorRight;
    public List<DoorTypes> doors;
   
    public Room(Vector2 _gridPos, RoomType roomType, CellType myCellType)
    {
        gridPos = _gridPos;
        this.roomType = roomType;
        this.myCellType = myCellType;
        doors= new List<DoorTypes>();
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
}
