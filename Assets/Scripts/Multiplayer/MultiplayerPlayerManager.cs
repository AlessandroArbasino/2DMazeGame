using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class MultiplayerPlayerManager : MonoBehaviour, IOnEventCallback
{
    protected const byte Translate_Arrow_Sprite_Event_Code = 1;
    protected const byte Translate_Player_Sprite_Event_Code = 2;
    protected const byte Destroy_ArrowSprite = 8;
    protected const byte OpponentsDeath = 9;
    protected const byte MonsterKilled = 10;
    protected const byte NoMoreAmmo = 20;

    private PlayerMovement playerMovement;
    private PlayerShoot playerShoot;

    [SerializeField] private Tilemap playerMap;
    [SerializeField] private TileBase playerBase;

    [SerializeField] private Tilemap opponentMapMap;
    [SerializeField] private TileBase opponentMapBase;

    [SerializeField] private Tilemap arrowMap;
    [SerializeField] private Tilemap oppponetArrowMap;
    [SerializeField] private TileBase arrowBase;

    [SerializeField] private FogOfWadUpdater fogUpdater;

    RaiseEventOptions raiseEventOption = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

    private Room currentRoom;

    [SerializeField] private int currentArrowNumber = 3;
    [SerializeField] private TextMeshProUGUI currentArrowNumberText;
    void Start()
    {
        TurnManager.Instance.GetInputClass().Player.Shoot.started += OnShot;
        TurnManager.Instance.GetInputClass().Player.Move.started += OnMove;

        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDestroy()
    {
        TurnManager.Instance.GetInputClass().Player.Shoot.started -= OnShot;
        TurnManager.Instance.GetInputClass().Player.Move.started -= OnMove;

        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void InitPlayer(Room[,] rooms, List<Vector2> takenPositions, Room currentRoom)
    {
        playerMovement = new PlayerMovement(rooms, takenPositions);
        playerShoot = new PlayerShoot(rooms, takenPositions);
        fogUpdater.UpdateFog(currentRoom);
        this.currentRoom = currentRoom;

        if (PhotonNetwork.IsMasterClient)
        {
            TurnManager.Instance.BeginTurnMessage();
        }

        Vector3Int drawPos = new Vector3Int((int)currentRoom.row, (int)currentRoom.col, 0);
        opponentMapMap.SetTile(drawPos, opponentMapBase);
    }

    public void OnShot(InputAction.CallbackContext context)
    {
        if (currentArrowNumber > 0)
        {
            ShootMethod(context.ReadValue<Vector2>(), DoorTypes.TopDoor, currentRoom);
            currentArrowNumber--;
            currentArrowNumberText.text = $"Remainig Arrows : {currentArrowNumber.ToString()}";

            if (currentArrowNumber == 0)
            {
                LoseGame("Not enought arrows to win the game");
                PhotonNetwork.RaiseEvent(NoMoreAmmo, null, raiseEventOption, SendOptions.SendReliable);
            }
        }
        //else
        //{
        //    LoseGame("Not enought arrows to win the game");
        //    PhotonNetwork.RaiseEvent(NoMoreAmmo, null, raiseEventOption, SendOptions.SendReliable);
        //}
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
            object[] content = new object[] { currentArrowRoom, newArrowRoom };
            PhotonNetwork.RaiseEvent(Translate_Arrow_Sprite_Event_Code, content, raiseEventOption, SendOptions.SendReliable);
            TranslateArrowSprite(currentArrowRoom, newArrowRoom);

            if (newArrowRoom.roomType == RoomType.Enemy)
            {
                WinGame("Hero you defeat the terrible monster");
                PhotonNetwork.RaiseEvent(MonsterKilled, null, raiseEventOption, SendOptions.SendReliable);
            }

            ShootMethod(newShootDirection, entry.entryDoor, newArrowRoom);
            yield break;
        }
        else
        {
            arrowMap.SetTile(new Vector3Int((int)currentArrowRoom.row, (int)currentArrowRoom.col, 0), null);

            object[] content = new object[] { currentArrowRoom };
            PhotonNetwork.RaiseEvent(Destroy_ArrowSprite, content, raiseEventOption, SendOptions.SendReliable);
            yield break;
        }
    }

    private void WinGame(string popUpMessage)
    {
        TurnManager.Instance.DisableInput();

        PopUpManager.Instance.SpawnPopUp(popUpMessage, "WIN", "PlayAgain", delegate { PlayAgain(); });


    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveMethod(context.ReadValue<Vector2>(), DoorTypes.TopDoor);
        TurnManager.Instance.EndTurnMessage();
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

        object[] content = new object[] { this.currentRoom, newCurrentRoom };
        PhotonNetwork.RaiseEvent(Translate_Player_Sprite_Event_Code, content, raiseEventOption, SendOptions.SendReliable);
        TranslateSprite(currentRoom, newCurrentRoom);
        currentRoom = newCurrentRoom;


        fogUpdater.UpdateFog(entry.nextRoom);

        if (entry.nextRoom.roomType == RoomType.Enemy || entry.nextRoom.roomType == RoomType.Hole)
        {
            PlayerDeath(entry.nextRoom.roomType);
            yield break;
        }

        if (entry.nextRoom.roomType == RoomType.Teleport)
        {
            Teleport();
            TurnManager.Instance.EnableInput();
            yield break;
        }

        if (newCurrentRoom.myCellType == CellType.Tunnel)
        {
            TurnManager.Instance.DisableInput();
            yield return new WaitForSeconds(.2f);
            MoveMethod(moveDirection, entry.entryDoor);
        }
        else
            TurnManager.Instance.EnableInput();
    }
    private void TranslateSprite(Room CurrentPlayerRoom, Room newCurrentRoom, bool isOpponent = false)
    {
        if (!isOpponent)
        {
            //clean the position before the movement 
            playerMap.SetTile(new Vector3Int((int)CurrentPlayerRoom.row, (int)CurrentPlayerRoom.col, 0), null);
            //set the new player base tile
            playerMap.SetTile(new Vector3Int((int)newCurrentRoom.row, (int)newCurrentRoom.col, 0), playerBase);
        }
        else
        {
            //clean the position before the movement 
            opponentMapMap.SetTile(new Vector3Int((int)CurrentPlayerRoom.row, (int)CurrentPlayerRoom.col, 0), null);
            //set the new player base tile
            opponentMapMap.SetTile(new Vector3Int((int)newCurrentRoom.row, (int)newCurrentRoom.col, 0), opponentMapBase);
        }
    }

    private void TranslateArrowSprite(Room previosArrowRoom, Room newCurrentRoom, bool isOpponent = false)
    {

        if (!isOpponent)
        {
            //clean the position before the movement 
            arrowMap.SetTile(new Vector3Int((int)previosArrowRoom.row, (int)previosArrowRoom.col, 0), null);
            //set the new player base tile
            arrowMap.SetTile(new Vector3Int((int)newCurrentRoom.row, (int)newCurrentRoom.col, 0), arrowBase);
        }
        else
        {
            oppponetArrowMap.SetTile(new Vector3Int((int)previosArrowRoom.row, (int)previosArrowRoom.col, 0), null);
            //set the new player base tile
            oppponetArrowMap.SetTile(new Vector3Int((int)newCurrentRoom.row, (int)newCurrentRoom.col, 0), arrowBase);
        }
    }
    private void PlayerDeath(RoomType whatKillsPlayer)
    {
        TurnManager.Instance.DisableInput();

        switch (whatKillsPlayer)
        {
            case RoomType.Enemy:
                LoseGame("The monster kills you");
                break;
            case RoomType.Hole:
                LoseGame("You fall into an endless hole");
                break;
        }


        PhotonNetwork.RaiseEvent(OpponentsDeath, null, raiseEventOption, SendOptions.SendReliable);
    }

    private void LoseGame(string message)
    {
        TurnManager.Instance.DisableInput();
        PopUpManager.Instance.SpawnPopUp(message, "Defeat", "PlayAgain", delegate { PlayAgain(); });
    }
    private void PlayAgain()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("GameScene");
    }
    private void Teleport()
    {
        Room teleportRoom = playerMovement.Teleport();

        object[] content = new object[] { this.currentRoom, teleportRoom };
        RaiseEventOptions raiseEventOption = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(Translate_Player_Sprite_Event_Code, content, raiseEventOption, SendOptions.SendReliable);
        TranslateSprite(currentRoom, teleportRoom);
        currentRoom = teleportRoom;
        fogUpdater.UpdateFog(teleportRoom);
    }


    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == Translate_Arrow_Sprite_Event_Code)
        {
            object[] data = (object[])photonEvent.CustomData;
            Room previousArrowRoom = (Room)data[0];
            Room otherNewArrowRoom = (Room)data[1];

            TranslateArrowSprite(previousArrowRoom, otherNewArrowRoom);
        }
        if (eventCode == Translate_Player_Sprite_Event_Code)
        {
            object[] data = (object[])photonEvent.CustomData;
            Room previousPlayerRoom = (Room)data[0];
            Room otherPlayerRoom = (Room)data[1];

            TranslateSprite(previousPlayerRoom, otherPlayerRoom, true);
        }
        if (eventCode == Destroy_ArrowSprite)
        {
            object[] data = (object[])photonEvent.CustomData;
            Room previousArrowPos = (Room)data[0];

            arrowMap.SetTile(new Vector3Int((int)previousArrowPos.row, (int)previousArrowPos.col, 0), null);
        }
        if (eventCode == OpponentsDeath)
        {
            WinGame("The other player is dead congratulations");
        }
        if (eventCode == MonsterKilled)
        {
            LoseGame("The other player kills the monster");
        }
        if (eventCode == NoMoreAmmo)
        {
            WinGame("The other player has no arrow to kill the monster");
        }
    }
}
