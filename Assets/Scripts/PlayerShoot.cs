using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot
{
    public Room[,] rooms;
    public List<Vector2> takenPositions = new List<Vector2>();
    Room CurrentRoom;
    public PlayerShoot(Room[,] rooms, List<Vector2> takenPositions,Room CurrentRoom)
    {
        this.rooms = rooms;
        this.takenPositions = takenPositions;
        this.CurrentRoom = CurrentRoom;
    }

    public Room Shoot(Vector2 shootDirection)
    {
        //saving the shoot direction 
        // couroutine 
        // same as the movement 
        return null;
    }
}
