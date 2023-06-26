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
    private void Awake()
    {
        //enabling input Actions
        myInput = new Inputs();
        myInput.Player.Enable();

    }
    void Start()
    {
        myInput.Player.Shoot.performed += OnShot;
        myInput.Player.Move.performed += OnMove;
    }

    public void InitPlayer(Room[,] rooms, List<Vector2> takenPositions, Room CurrentRoom)
    {
        playerMovement = new PlayerMovement(rooms, takenPositions, CurrentRoom);
        playerShoot= new PlayerShoot( rooms, takenPositions,CurrentRoom);
    }

    public void OnShot(InputAction.CallbackContext context)
    {
        playerShoot.Shoot(context.ReadValue<Vector2>());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Room newPlayerPos=playerMovement.CheckMovement(context.ReadValue<Vector2>());
        //moving sprite
        if(newPlayerPos.myCellType == CellType.Tunnel)
        {
            //to the other door and move to the next tile
            newPlayerPos = playerMovement.CheckMovement(context.ReadValue<Vector2>());
        }
    }

}
