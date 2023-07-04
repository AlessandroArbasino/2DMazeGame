using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement
{
    public Room[,] rooms;
    public List<Vector2> takenPositions = new List<Vector2>();
    Room currentRoom;
    private int gridSizeX = 20;
    private int gridSizeY = 20;
    List<Room> returnedList = new List<Room>();

    public PlayerMovement(Room[,] rooms, List<Vector2> takenPositions)
    {
        this.rooms = rooms;
        this.takenPositions = takenPositions;
    }

    public NextRoomEntryDoor CheckNormalMovement(Vector2 moveDirection,Room currentRoom)
    {
        // the next room ingress room is the opposite 
        Room newRoom = null;
        DoorTypes usedDoor = DoorTypes.TopDoor;
        if (moveDirection.y > 0)
            if (currentRoom.doorTop)
                if (takenPositions.Contains(new Vector2((int)currentRoom.row, (int)currentRoom.col + 1)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.row, (int)currentRoom.col + 1));
                    usedDoor = DoorTypes.BottomDoor;
                }


        if (moveDirection.y < 0)
            if (currentRoom.doorBot)
                if (takenPositions.Contains(new Vector2((int)currentRoom.row, (int)currentRoom.col - 1)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.row, (int)currentRoom.col - 1));
                    usedDoor = DoorTypes.TopDoor;
                }


        if (moveDirection.x < 0)
            if (currentRoom.doorleft)
                if (takenPositions.Contains(new Vector2((int)currentRoom.row - 1, (int)currentRoom.col)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.row - 1, (int)currentRoom.col));
                    usedDoor = DoorTypes.RightDoor;
                }



        if (moveDirection.x > 0)
            if (currentRoom.doorRight)
                if (takenPositions.Contains(new Vector2((int)currentRoom.row + 1, (int)currentRoom.col)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.row + 1, (int)currentRoom.col));
                    usedDoor = DoorTypes.LeftDoor;
                }

        return new NextRoomEntryDoor(newRoom,usedDoor);
    }

    public NextRoomEntryDoor CheckTunnelMovement(DoorTypes usedDoor, Room currentRoom)
    {
        Room newRoom = null;
        DoorTypes newusedDoor = DoorTypes.TopDoor;
        if (usedDoor != DoorTypes.TopDoor)
            if (currentRoom.doorTop)
                if (takenPositions.Contains(new Vector2((int)currentRoom.row, (int)currentRoom.col + 1)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.row, (int)currentRoom.col + 1));
                    newusedDoor = DoorTypes.BottomDoor;
                }

        if (usedDoor != DoorTypes.BottomDoor)
            if (currentRoom.doorBot)
                if (takenPositions.Contains(new Vector2((int)currentRoom.row, (int)currentRoom.col - 1)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.row, (int)currentRoom.col - 1));
                    newusedDoor = DoorTypes.TopDoor;
                }


        if (usedDoor != DoorTypes.LeftDoor)
            if (currentRoom.doorleft)
                if (takenPositions.Contains(new Vector2((int)currentRoom.row - 1, (int)currentRoom.col)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.row - 1, (int)currentRoom.col));
                    newusedDoor = DoorTypes.RightDoor;
                }

        if (usedDoor != DoorTypes.RightDoor)
            if (currentRoom.doorRight)
                if (takenPositions.Contains(new Vector2((int)currentRoom.row + 1, (int)currentRoom.col)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.row + 1, (int)currentRoom.col));
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
                if (rooms[x, y].row == newGripPositoin.x && rooms[x, y].col == newGripPositoin.y)
                {
                    newRoom = rooms[x, y];
                    break;
                }
            }
        }
        return newRoom;
    }

    private Room GetTeleportRoom(Vector2 newGripPositionIn)
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
                if (rooms[x, y].row == newGripPositionIn.x && rooms[x, y].col == newGripPositionIn.y)
                {
                    newRoom = rooms[x, y];
                    break;
                }
            }
        }

        return newRoom;
    }

    public Room Teleport()
    {
        int randomCell = UnityEngine.Random.Range(0, takenPositions.Count);

        Room teleportRoom = GetTeleportRoom(takenPositions[randomCell]);

        if (teleportRoom.roomType != RoomType.Enemy)
        {
            currentRoom = teleportRoom;
            return teleportRoom;
        }
        teleportRoom = Teleport();
        return teleportRoom;
    }
}
