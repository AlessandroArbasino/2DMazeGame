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
    public bool checkMovement()
    {
        return true;
    }
}
