using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement
{
    public Room[,] rooms;
    public List<Vector2> takenPositions = new List<Vector2>();
    Room currentRoom;
    private int gridSizeX = 20;
    private int gridSizeY = 20;

    public PlayerMovement(Room[,] rooms, List<Vector2> takenPositions, Room CurrentRoom)
    {
        this.rooms = rooms;
        this.takenPositions = takenPositions;
        this.currentRoom = CurrentRoom;
    }
    public List<Room> CheckMovement(Vector2 moveDirection)
    {
        List<Room> returnedList = new List<Room>();
        if (moveDirection.y > 0)
        {
            if (currentRoom.doorTop)
            {
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1)))
                    returnedList.Add(GetNextRoom(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1)));
            }
        }
        else if (moveDirection.y < 0)
        {
            if (currentRoom.doorBot)
            {
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1)))
                    returnedList.Add(GetNextRoom(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1)));
            }
        }
        else if (moveDirection.x < 0)
        {
            if (currentRoom.doorleft)
            {
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x - 1, (int)currentRoom.gridPos.y)))
                    returnedList.Add(GetNextRoom(new Vector2((int)currentRoom.gridPos.x - 1, (int)currentRoom.gridPos.y)));
            }
        }

        else if (moveDirection.x > 0)
        {
            if (currentRoom.doorRight)
            {
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x + 1, (int)currentRoom.gridPos.y)))
                    returnedList.Add(GetNextRoom(new Vector2((int)currentRoom.gridPos.x + 1, (int)currentRoom.gridPos.y)));
            }
        }

        if (returnedList.Count > 0)
            currentRoom = returnedList.Last();
        return returnedList;
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
}
