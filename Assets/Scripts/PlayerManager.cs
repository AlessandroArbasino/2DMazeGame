using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerShoot playerShoot;
    private Inputs myInput;

    [SerializeField] private Tilemap playerMap;
    [SerializeField] private TileBase playerBase;

    [SerializeField] private FogOfWadUpdater fogUpdater;

    private Room currentRoom;
    private void Awake()
    {
        //enabling input Actions
        myInput = new Inputs();
        myInput.Player.Enable();

    }
    void Start()
    {
        myInput.Player.Shoot.started += OnShot;
        myInput.Player.Move.started += OnMove;
    }

    public void InitPlayer(Room[,] rooms, List<Vector2> takenPositions, Room currentRoom)
    {
        playerMovement = new PlayerMovement(rooms, takenPositions, currentRoom);
        playerShoot= new PlayerShoot( rooms, takenPositions,currentRoom);
        fogUpdater.UpdateFog(new List<Room> { currentRoom });
        this.currentRoom = currentRoom;
    }

    public void OnShot(InputAction.CallbackContext context)
    {
        playerShoot.Shoot(context.ReadValue<Vector2>());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        List<Room> newPlayerVisitedRooms=playerMovement.CheckMovement(context.ReadValue<Vector2>());
        //moving sprite
        if(newPlayerVisitedRooms == null)
        {
            Debug.Log("cantMove");
            return;
        }
        if (newPlayerVisitedRooms.Count == 0)
        {
            Debug.Log("no door");
            return;
        }

        //clean the position before the movement 
        playerMap.SetTile(new Vector3Int((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y, 0), null);
        //set new player position
        currentRoom = newPlayerVisitedRooms.Last();
        //set the new player base tile
        playerMap.SetTile(new Vector3Int((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y, 0), playerBase);


        fogUpdater.UpdateFog(newPlayerVisitedRooms);
    }

}
