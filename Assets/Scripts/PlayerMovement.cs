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
    public Room CheckMovement(Vector2 moveDirection)
    {
        //never equals returns null TODO
        if(moveDirection == Vector2.up)
        {
            if (currentRoom.doorTop)
            {
                return rooms[(int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1];
            }
        }
        else if (moveDirection == Vector2.down)
        {
            if (currentRoom.doorBot)
            {
                return rooms[(int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1];
            }
        }
        else if (moveDirection == Vector2.left)
        {
            if (currentRoom.doorleft)
            {
                return rooms[(int)currentRoom.gridPos.x-1, (int)currentRoom.gridPos.y];
            }
        }

        else if (moveDirection == Vector2.right)
        {
            if (currentRoom.doorRight)
            {
                return rooms[(int)currentRoom.gridPos.x+1, (int)currentRoom.gridPos.y];
            }
        }
        return null;
    }
}
