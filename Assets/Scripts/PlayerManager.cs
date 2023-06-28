using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class PlayerManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerShoot playerShoot;
    private Inputs myInput;

    [SerializeField] private Tilemap playerMap;
    [SerializeField] private TileBase playerBase;

    [SerializeField] private Tilemap arrowMap;
    [SerializeField] private TileBase arrowBase;

    [SerializeField] private FogOfWadUpdater fogUpdater;

    private Room currentRoom;

    public static event Action OnDeath;
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
        playerShoot = new PlayerShoot(rooms, takenPositions, currentRoom);
        fogUpdater.UpdateFog(new List<Room> { currentRoom });
        this.currentRoom = currentRoom;
    }

    public void OnShot(InputAction.CallbackContext context)
    {
        ShootMethod(context.ReadValue<Vector2>(),this.currentRoom);
    }

    private void ShootMethod(Vector2 shootDirection,Room currentRoom)
    {
        StartCoroutine(ShootCouroutine(shootDirection,currentRoom));
    }

    private IEnumerator ShootCouroutine(Vector2 shootDirection, Room currentRoom)
    {
        Room newArrowRoom = playerShoot.Shoot(shootDirection, currentRoom);
        yield return new WaitForSeconds(.2f);

        if (newArrowRoom != null)
        {
            TranslateArrowSprite(newArrowRoom);
            ShootMethod(shootDirection,newArrowRoom);
        }
        else
        {
            yield break;
        }


        if(newArrowRoom.roomType==RoomType.Enemy)
        {
            WinGame();
        }

    }

    private void WinGame()
    {
        Debug.Log("YouWin");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        List<Room> newPlayerVisitedRooms = playerMovement.CheckMovement(context.ReadValue<Vector2>());
        //moving sprite
        if (newPlayerVisitedRooms == null)
            return;
        if (newPlayerVisitedRooms.Count == 0)
            return;

        TranslateSprite(newPlayerVisitedRooms.Last());

        foreach (Room room in newPlayerVisitedRooms)
        {
            if (room.roomType == RoomType.Enemy || room.roomType == RoomType.Hole)
                PlayerDeath();

            if (room.roomType == RoomType.Teleport)
                Teleport();
        }

        fogUpdater.UpdateFog(newPlayerVisitedRooms);
    }
    private void TranslateSprite(Room newCurrentRoom)
    {
        //clean the position before the movement 
        playerMap.SetTile(new Vector3Int((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y, 0), null);
        //set new player position
        currentRoom = newCurrentRoom;
        //set the new player base tile
        playerMap.SetTile(new Vector3Int((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y, 0), playerBase);
    }

    private void TranslateArrowSprite(Room newCurrentRoom)
    {
        //clean the position before the movement 
        arrowMap.SetTile(new Vector3Int((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y, 0), null);
        //set new player position
        currentRoom = newCurrentRoom;
        //set the new player base tile
        arrowMap.SetTile(new Vector3Int((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y, 0), arrowBase);
    }
    private void PlayerDeath()
    {
        OnDeath?.Invoke();
        //myInput.Player.Disable();
    }

    private void Teleport()
    {
        Room teleportRoom = playerMovement.Teleport();

        TranslateSprite(teleportRoom);
        fogUpdater.UpdateFog(new List<Room> { teleportRoom });
    }

}
