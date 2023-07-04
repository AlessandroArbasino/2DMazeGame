using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class MultiplayerPlayerManager : MonoBehaviour,IOnEventCallback
{
    protected const byte Translate_Arrow_Sprite_Event_Code = 1;
    protected const byte Translate_Player_Sprite_Event_Code = 2;

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

        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDestroy()
    {
        myInput.Player.Shoot.started -= OnShot;
        myInput.Player.Move.started -= OnMove;

        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
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
            object[] content = new object[] { newArrowRoom };
            RaiseEventOptions raiseEventOption = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(Translate_Arrow_Sprite_Event_Code, content, raiseEventOption,SendOptions.SendReliable);
            TranslateArrowSprite(newArrowRoom);

            if (newArrowRoom.roomType == RoomType.Enemy)
            {
                WinGame();
            }

            ShootMethod(newShootDirection, entry.entryDoor, newArrowRoom);
            yield break;
        }
        else
        {
            arrowMap.SetTile(new Vector3Int((int)currentArrowRoom.row, (int)currentArrowRoom.col, 0), null);
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

        object[] content = new object[] { this.currentRoom,newCurrentRoom };
        RaiseEventOptions raiseEventOption = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(Translate_Player_Sprite_Event_Code, content, raiseEventOption, SendOptions.SendReliable);
        TranslateSprite(currentRoom, newCurrentRoom);
        currentRoom = newCurrentRoom;


        fogUpdater.UpdateFog(entry.nextRoom);

        if (entry.nextRoom.roomType == RoomType.Enemy || entry.nextRoom.roomType == RoomType.Hole)
        {
            //PlayerDeath();
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
    private void TranslateSprite(Room CurrentPlayerRoom,Room newCurrentRoom)
    {
        //clean the position before the movement 
        playerMap.SetTile(new Vector3Int((int)CurrentPlayerRoom.row, (int)CurrentPlayerRoom.col, 0), null);
        //set the new player base tile
        playerMap.SetTile(new Vector3Int((int)newCurrentRoom.row, (int)newCurrentRoom.col, 0), playerBase);
    }

    private void TranslateArrowSprite(Room newCurrentRoom)
    {
        //clean the position before the movement 
        arrowMap.SetTile(new Vector3Int((int)currentArrowRoom.row, (int)currentArrowRoom.col, 0), null);
        //set new player position
        currentArrowRoom = newCurrentRoom;
        //set the new player base tile
        arrowMap.SetTile(new Vector3Int((int)currentArrowRoom.row, (int)currentArrowRoom.col, 0), arrowBase);
        //TileChangeData arrowData = new TileChangeData(new Vector3Int((int)currentArrowRoom.row, (int)currentArrowRoom.col, 0), arrowBase,Color.black, Matrix4x4.Rotate(Quaternion.LookRotation(arrowDirection,Vector2.up)));
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
        currentRoom= teleportRoom;
        TranslateSprite(currentRoom,teleportRoom);
        fogUpdater.UpdateFog(teleportRoom);
    }


    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == Translate_Arrow_Sprite_Event_Code)
        {
            object[] data = (object[])photonEvent.CustomData;
            Room otherArrowRoom = (Room)data[0];

            TranslateArrowSprite(otherArrowRoom);
        }
        if (eventCode == Translate_Player_Sprite_Event_Code)
        {
            object[] data = (object[])photonEvent.CustomData;
            Room previousPlayerRoom = (Room)data[0];
            Room otherPlayerRoom = (Room)data[1];

            TranslateSprite(previousPlayerRoom,otherPlayerRoom);
        }
    }
}
