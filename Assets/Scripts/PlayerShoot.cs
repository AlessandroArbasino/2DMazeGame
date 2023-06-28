using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerShoot
{
    public Room[,] rooms;
    public List<Vector2> takenPositions = new List<Vector2>();
    Room currentRoom;
    private int gridSizeX = 20;
    private int gridSizeY = 20;

    public PlayerShoot(Room[,] rooms, List<Vector2> takenPositions, Room currentRoom)
    {
        this.rooms = rooms;
        this.takenPositions = takenPositions;
        this.currentRoom = currentRoom;
    }

    public Room Shoot(Vector2 shootDirection,Room currentPlayerRoom)
    {
        currentRoom= currentPlayerRoom;
        Room returnedRoom = null;
        if (shootDirection.y > 0)
        {
            if (currentRoom.doorTop)
            {
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1)))
                    returnedRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1));
            }
        }
        else if (shootDirection.y < 0)
        {
            if (currentRoom.doorBot)
            {
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1)))
                    returnedRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1));
            }
        }
        else if (shootDirection.x < 0)
        {
            if (currentRoom.doorleft)
            {
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x - 1, (int)currentRoom.gridPos.y)))
                    returnedRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x - 1, (int)currentRoom.gridPos.y));
            }
        }

        else if (shootDirection.x > 0)
        {
            if (currentRoom.doorRight)
            {
                if (takenPositions.Contains(new Vector2((int)currentRoom.gridPos.x + 1, (int)currentRoom.gridPos.y)))
                    returnedRoom = GetNextRoom(new Vector2((int)currentRoom.gridPos.x + 1, (int)currentRoom.gridPos.y));
            }
        }
        return returnedRoom;
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
