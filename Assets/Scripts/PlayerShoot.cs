using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NextRoomEntryDoor
{
    public Room nextRoom;
    public DoorTypes entryDoor;
    public DoorTypes exitDoor;

    public NextRoomEntryDoor(Room nextRoom, DoorTypes entryDoor)
    {
        this.entryDoor = entryDoor;
        this.nextRoom = nextRoom;
    }
}
public class PlayerShoot
{
    public Room[,] rooms;
    public List<Vector2> takenPositions = new List<Vector2>();
    private int gridSizeX = 20;
    private int gridSizeY = 20;

    public PlayerShoot(Room[,] rooms, List<Vector2> takenPositions)
    {
        this.rooms = rooms;
        this.takenPositions = takenPositions;
    }

    public NextRoomEntryDoor Shoot(Vector2 shootDirection, Room currentRoom)
    {
        Room newRoom = null;
        DoorTypes newusedDoor = DoorTypes.TopDoor;
        if (shootDirection.y > 0)
        {
            if (currentRoom.doorTop)
            {
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1));
                    newusedDoor = DoorTypes.BottomDoor;
                }
            }
        }
        else if (shootDirection.y < 0)
        {
            if (currentRoom.doorBot)
            {
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1));
                    newusedDoor = DoorTypes.TopDoor;
                }
            }
        }
        else if (shootDirection.x < 0)
        {
            if (currentRoom.doorleft)
            {
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x - 1, (int)currentRoom.gridPos.y)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x - 1, (int)currentRoom.gridPos.y));
                    newusedDoor = DoorTypes.RightDoor;
                }
            }
        }

        else if (shootDirection.x > 0)
        {
            if (currentRoom.doorRight)
            {
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x + 1, (int)currentRoom.gridPos.y)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x + 1, (int)currentRoom.gridPos.y));
                    newusedDoor = DoorTypes.LeftDoor;
                }
            }
        }
        return new NextRoomEntryDoor(newRoom, newusedDoor);
    }

    public NextRoomEntryDoor CheckTunnelShoot(DoorTypes usedDoor, Room currentRoom)
    {
        Room newRoom = null;
        DoorTypes newusedDoor = DoorTypes.TopDoor;
        if (usedDoor != DoorTypes.TopDoor)
            if (currentRoom.doorTop)
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1));
                    newusedDoor = DoorTypes.BottomDoor;
                }

        if (usedDoor != DoorTypes.BottomDoor)
            if (currentRoom.doorBot)
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1));
                    newusedDoor = DoorTypes.TopDoor;
                }


        if (usedDoor != DoorTypes.LeftDoor)
            if (currentRoom.doorleft)
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x - 1, (int)currentRoom.gridPos.y)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x - 1, (int)currentRoom.gridPos.y));
                    newusedDoor = DoorTypes.RightDoor;
                }

        if (usedDoor != DoorTypes.RightDoor)
            if (currentRoom.doorRight)
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x + 1, (int)currentRoom.gridPos.y)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x + 1, (int)currentRoom.gridPos.y));
                    newusedDoor = DoorTypes.LeftDoor;
                }

        return new NextRoomEntryDoor(newRoom, newusedDoor);
    }
    private Room GetNextRoom(Vector2 newGripPositoin)
    {
        Room newRoom = null;
        for (int x = 0; x < (gridSizeX * 2); x++)
        {
            for (int y = 0; y < (gridSizeY * 2); y++)
            {
                if (rooms[x, y] == null)
                {
                    continue;
                }
                if (rooms[x, y].gridPos == newGripPositoin)
                {
                    newRoom = rooms[x, y];
                    break;
                }
            }
        }
        return newRoom;
    }

    public Vector2 CalcolateNewShootdirection(DoorTypes usedDoor)
    {
        Debug.Log(usedDoor.ToString());
        return usedDoor switch
        {
            DoorTypes.TopDoor => new Vector2(0, -1),
            DoorTypes.BottomDoor => new Vector2(0, 1),
            DoorTypes.LeftDoor => new Vector2(1, 0),
            DoorTypes.RightDoor => new Vector2(-1, 0),
            _ => Vector2.zero,
        };
    }
}
