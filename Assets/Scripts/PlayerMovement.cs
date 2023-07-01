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

    public PlayerMovement(Room[,] rooms, List<Vector2> takenPositions, Room CurrentRoom)
    {
        this.rooms = rooms;
        this.takenPositions = takenPositions;
        this.currentRoom = CurrentRoom;
    }

    public List<Room> Move(Vector2 moveDirection)
    {
        returnedList = new List<Room>();
        CheckMovement(moveDirection);

        return returnedList;
    }
    public void CheckMovement(Vector2 moveDirection)
    {
        if (moveDirection != Vector2.zero)
        {
            CheckNormalMovement(moveDirection);
        }


    }

    private void CheckNormalMovement(Vector2 moveDirection)
    {
        // the next room ingress room is the opposite 
        Room newRoom = null;
        DoorTypes usedDoor = DoorTypes.TopDoor;
        if (moveDirection.y > 0)
            if (currentRoom.doorTop)
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1), DoorTypes.BottomDoor);
                    usedDoor = DoorTypes.BottomDoor;
                }


        if (moveDirection.y < 0)
            if (currentRoom.doorBot)
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1), DoorTypes.TopDoor);
                    usedDoor = DoorTypes.TopDoor;
                }


        if (moveDirection.x < 0)
            if (currentRoom.doorleft)
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x - 1, (int)currentRoom.gridPos.y)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x - 1, (int)currentRoom.gridPos.y), DoorTypes.RightDoor);
                    usedDoor = DoorTypes.RightDoor;
                }



        if (moveDirection.x > 0)
            if (currentRoom.doorRight)
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x + 1, (int)currentRoom.gridPos.y)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x + 1, (int)currentRoom.gridPos.y), DoorTypes.LeftDoor);
                    usedDoor = DoorTypes.LeftDoor;
                }

        if (newRoom != null)
        {
            currentRoom = newRoom;
            returnedList.Add(newRoom);

            if (newRoom.myCellType == CellType.Tunnel)
            {
                CheckTunnelMovement(usedDoor);
            }
            return;
        }

        return;
    }

    private void CheckTunnelMovement(DoorTypes usedDoor)
    {
        Room newRoom = null;
        DoorTypes newusedDoor = DoorTypes.TopDoor;
        if (usedDoor != DoorTypes.TopDoor)
            if (currentRoom.doorTop)
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1), DoorTypes.BottomDoor);
                    newusedDoor = DoorTypes.BottomDoor;
                }

        if (usedDoor != DoorTypes.BottomDoor)
            if (currentRoom.doorBot)
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1), DoorTypes.TopDoor);
                    newusedDoor = DoorTypes.TopDoor;
                }


        if (usedDoor != DoorTypes.LeftDoor)
            if (currentRoom.doorleft)
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x - 1, (int)currentRoom.gridPos.y)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x - 1, (int)currentRoom.gridPos.y), DoorTypes.RightDoor);
                    newusedDoor = DoorTypes.RightDoor;
                }

        if (usedDoor != DoorTypes.RightDoor)
            if (currentRoom.doorRight)
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x + 1, (int)currentRoom.gridPos.y)))
                {
                    newRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x + 1, (int)currentRoom.gridPos.y), DoorTypes.LeftDoor);
                    newusedDoor = DoorTypes.LeftDoor;
                }

        if (newRoom != null)
        {
            currentRoom = newRoom;
            returnedList.Add(newRoom);

            if (newRoom.myCellType == CellType.Tunnel)
            {
                CheckTunnelMovement(newusedDoor);
            }
            return;
        }

        return;
    }

    private Room GetNextRoom(Vector2 newGripPositoin, DoorTypes usedDoor)
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
                if (rooms[x, y].gridPos == newGripPositionIn)
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
