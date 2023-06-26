using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerShoot playerShoot;
    void Start()
    {
        //enabling input Actions
    }

    public void InitPlayer(Room[,] rooms, List<Vector2> takenPositions, Room CurrentRoom)
    {
        playerMovement = new PlayerMovement(rooms, takenPositions, CurrentRoom);
        playerShoot= new PlayerShoot( rooms, takenPositions,CurrentRoom);
    }

}
