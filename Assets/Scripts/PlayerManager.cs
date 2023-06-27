using System.Collections;
using System.Collections.Generic;
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

    public void InitPlayer(Room[,] rooms, List<Vector2> takenPositions, Room CurrentRoom)
    {
        playerMovement = new PlayerMovement(rooms, takenPositions, CurrentRoom);
        playerShoot= new PlayerShoot( rooms, takenPositions,CurrentRoom);
        fogUpdater.UpdateFog(new List<Room> { CurrentRoom });
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

        fogUpdater.UpdateFog(newPlayerVisitedRooms);
    }

}
