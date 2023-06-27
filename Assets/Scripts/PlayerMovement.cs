using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement 
{
    public Room[,] rooms;
    public List<Vector2> takenPositions = new List<Vector2>();
    Room currentRoom;
    public PlayerMovement(Room[,] rooms, List<Vector2> takenPositions,Room CurrentRoom)
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
                if ((int)currentRoom.gridPos.y + 2 < rooms.Length)
                    if (rooms[(int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1]!=null)
                    returnedList.Add(rooms[(int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1]);
            }
        }
        else if (moveDirection.y < 0)
        {
            if (currentRoom.doorBot)
            {
                if((int)currentRoom.gridPos.y - 2 > 0)
                if (rooms[(int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1] != null)
                    returnedList.Add(rooms[(int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1]);
            }
        }
        else if (moveDirection.x < 0)
        {
            if (currentRoom.doorleft)
            {
                if ((int)currentRoom.gridPos.x - 2 > 0)
                    if (rooms[(int)currentRoom.gridPos.x-1, (int)currentRoom.gridPos.y] != null)
                        returnedList.Add(rooms[(int)currentRoom.gridPos.x-1, (int)currentRoom.gridPos.y]);
            }
        }

        else if (moveDirection.x > 0)
        {
            if (currentRoom.doorRight)
            {
                if((int)currentRoom.gridPos.x + 2< rooms.Length)
                if (rooms[(int)currentRoom.gridPos.x+1, (int)currentRoom.gridPos.y] != null)
                    returnedList.Add(rooms[(int)currentRoom.gridPos.x+1, (int)currentRoom.gridPos.y]);
            }
        }
        return returnedList;
    }
}
