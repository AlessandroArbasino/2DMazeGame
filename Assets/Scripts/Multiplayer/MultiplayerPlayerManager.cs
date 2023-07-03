using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class MultiplayerPlayerManager : MonoBehaviour
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
    private Room currentArrowRoom;

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

    private void OnDestroy()
    {
        myInput.Player.Shoot.started -= OnShot;
        myInput.Player.Move.started -= OnMove;
    }

    public void InitPlayer(Room[,] rooms, List<Vector2> takenPositions, Room currentRoom)
    {
        playerMovement = new PlayerMovement(rooms, takenPositions);
        playerShoot = new PlayerShoot(rooms, takenPositions);
        fogUpdater.UpdateFog(currentRoom);
        this.currentRoom = currentRoom;
        this.currentArrowRoom = currentRoom;
    }

    public void OnShot(InputAction.CallbackContext context)
    {
        ShootMethod(context.ReadValue<Vector2>(), DoorTypes.TopDoor, currentRoom);
    }

    private void ShootMethod(Vector2 shootDirection, DoorTypes usedDoor, Room currentArrowRoom)
    {
        StartCoroutine(ShootCouroutine(shootDirection, usedDoor, currentArrowRoom));
    }

    private IEnumerator ShootCouroutine(Vector2 shootDirection, DoorTypes usedDoor, Room currentArrowRoom)
    {
        NextRoomEntryDoor entry;
        Vector2 newShootDirection;
        if (currentArrowRoom.myCellType == CellType.Tunnel)
        {
            entry = playerShoot.CheckTunnelShoot(usedDoor, currentArrowRoom);
            newShootDirection = playerShoot.CalcolateNewShootdirection(entry.entryDoor);
        }
        else
        {
            entry = playerShoot.Shoot(shootDirection, currentArrowRoom);
            newShootDirection = shootDirection;
        }
        Room newArrowRoom = entry.nextRoom;
        yield return new WaitForSeconds(.2f);

        if (newArrowRoom != null)
        {
            TranslateArrowSprite(newArrowRoom, newShootDirection);

            if (newArrowRoom.roomType == RoomType.Enemy)
            {
                WinGame();
            }

            ShootMethod(newShootDirection, entry.entryDoor, newArrowRoom);
            yield break;
        }
        else
        {
            arrowMap.SetTile(new Vector3Int((int)currentArrowRoom.gridPos.x, (int)currentArrowRoom.gridPos.y, 0), null);
            yield break;
        }
    }

    private void WinGame()
    {
        myInput.Player.Disable();

        PopUpManager.Instance.SpawnPopUp("Hero you defeat the terrible monster", "WIN", "PlayAgain", delegate { PlayAgain(); });
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveMethod(context.ReadValue<Vector2>(), DoorTypes.TopDoor);
    }

    private void MoveMethod(Vector2 moveDirection, DoorTypes usedDoor)
    {
        StartCoroutine(MoveCouroutine(moveDirection, usedDoor));
    }

    private IEnumerator MoveCouroutine(Vector2 moveDirection, DoorTypes usedDoor)
    {
        NextRoomEntryDoor entry;
        if (currentRoom.myCellType == CellType.Tunnel)
            entry = playerMovement.CheckTunnelMovement(usedDoor, currentRoom);
        else
            entry = playerMovement.CheckNormalMovement(moveDirection, currentRoom);

        Room newCurrentRoom = entry.nextRoom;
        if (newCurrentRoom == null)
            yield break;

        TranslateSprite(newCurrentRoom);
        currentRoom = newCurrentRoom;


        fogUpdater.UpdateFog(entry.nextRoom);

        if (entry.nextRoom.roomType == RoomType.Enemy || entry.nextRoom.roomType == RoomType.Hole)
        {
            PlayerDeath();
            yield break;
        }

        if (entry.nextRoom.roomType == RoomType.Teleport)
        {
            Teleport();
            myInput.Player.Enable();
            yield break;
        }

        if (newCurrentRoom.myCellType == CellType.Tunnel)
        {
            myInput.Player.Disable();
            yield return new WaitForSeconds(.2f);
            MoveMethod(moveDirection, entry.entryDoor);
        }
        else
            myInput.Player.Enable();
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

    private void TranslateArrowSprite(Room newCurrentRoom, Vector2 arrowDirection)
    {
        //clean the position before the movement 
        arrowMap.SetTile(new Vector3Int((int)currentArrowRoom.gridPos.x, (int)currentArrowRoom.gridPos.y, 0), null);
        //set new player position
        currentArrowRoom = newCurrentRoom;
        //set the new player base tile
        arrowMap.SetTile(new Vector3Int((int)currentArrowRoom.gridPos.x, (int)currentArrowRoom.gridPos.y, 0), arrowBase);
        //TileChangeData arrowData = new TileChangeData(new Vector3Int((int)currentArrowRoom.gridPos.x, (int)currentArrowRoom.gridPos.y, 0), arrowBase,Color.black, Matrix4x4.Rotate(Quaternion.LookRotation(arrowDirection,Vector2.up)));
        //arrowMap.SetTile(arrowData, true);
    }
    private void PlayerDeath()
    {
        myInput.Player.Disable();

        PopUpManager.Instance.SpawnPopUp("The monster kills you", "DEFEAT", "PlayAgain", delegate { PlayAgain(); });
    }

    private void PlayAgain()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("GameScene");
    }
    private void Teleport()
    {
        Room teleportRoom = playerMovement.Teleport();

        TranslateSprite(teleportRoom);
        fogUpdater.UpdateFog(teleportRoom);
    }

}
