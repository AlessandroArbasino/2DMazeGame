using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MultiplayerManager : PlayerManagerbase
{
    protected const byte Translate_Arrow_Sprite_Event_Code = 1;
    protected const byte Translate_Player_Sprite_Event_Code = 2;
    protected const byte Destroy_ArrowSprite = 8;
    protected const byte OpponentsDeath = 9;
    protected const byte MonsterKilled = 10;
    protected const byte NoMoreAmmo = 20;
    protected const byte Move_Monster = 21;
    // Start is called before the first frame update

    RaiseEventOptions raiseEventOption = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

    [SerializeField] private Tilemap opponentMapMap;
    [SerializeField] private TileBase opponentMapBase;

    [SerializeField] private Tilemap oppponetArrowMap;
    protected override void Start()
    {
        base.Start();
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
    }

    public override void InitPlayer(Room[,] rooms, List<Vector2> takenPositions, Room currentRoom)
    {
        base.InitPlayer(rooms, takenPositions, currentRoom);

        if (PhotonNetwork.IsMasterClient)
        {
            TurnManager.Instance.BeginTurnMessage();
        }

        Vector3Int drawPos = new Vector3Int((int)currentRoom.row, (int)currentRoom.col, 0);
        opponentMapMap.SetTile(drawPos, opponentMapBase);
    }


    public override void OnShot(InputAction.CallbackContext context)
    {
        base.OnShot(context);

        if (currentArrowNumber == 0)
        {
            LoseGame("Not enought arrows to win the game");
            PhotonNetwork.RaiseEvent(NoMoreAmmo, null, raiseEventOption, SendOptions.SendReliable);
        }
    }

    protected override IEnumerator ShootCouroutine(Vector2 shootDirection, DoorTypes usedDoor, Room currentArrowRoom)
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
            object[] content = new object[] { currentArrowRoom, newArrowRoom,shootDirection };
            PhotonNetwork.RaiseEvent(Translate_Arrow_Sprite_Event_Code, content, raiseEventOption, SendOptions.SendReliable);
            TranslateArrowSprite(shootDirection, currentArrowRoom, newArrowRoom);

            if(newArrowRoom == currentRoom)
            {
                PlayerDeath(RoomType.Arrow);
            }
            if (newArrowRoom.roomType == RoomType.Enemy)
            {
                WinGame("Mission achieved: you defeated the monster");
                PhotonNetwork.RaiseEvent(MonsterKilled, null, raiseEventOption, SendOptions.SendReliable);
            }

            ShootMethod(newShootDirection, entry.entryDoor, newArrowRoom);
            yield break;
        }
        else
        {
            arrowMap.SetTile(new Vector3Int((int)currentArrowRoom.row, (int)currentArrowRoom.col, 0), null);
            object[] content = new object[] { currentArrowRoom };
            TurnManager.Instance.EnableInput();
            PhotonNetwork.RaiseEvent(Destroy_ArrowSprite, content, raiseEventOption, SendOptions.SendReliable);

            Room newMonsterPos=NewMonsterPosition();
            object[] monsterContent = new object[] { newMonsterPos };
            PhotonNetwork.RaiseEvent(Move_Monster, monsterContent, raiseEventOption, SendOptions.SendReliable);
            yield break;
        }
    }

    protected override void WinGame(string popUpMessage)
    {
        TurnManager.Instance.DisableInput();

        PopUpManager.Instance.SpawnPopUp(popUpMessage, "WIN", "Play Again", delegate { PlayAgain(); }, PopUpButtonNumbers.MainMenuPopUp);
    }

    protected override void LoseGame(string message)
    {
        TurnManager.Instance.DisableInput();
        PopUpManager.Instance.SpawnPopUp(message, "Defeat", "Play Again", delegate { PlayAgain(); }, PopUpButtonNumbers.MainMenuPopUp);

    }
    protected override void OnMove(InputAction.CallbackContext context)
    {
        base.OnMove(context);
        TurnManager.Instance.EndTurnMessage();
    }
    protected override IEnumerator MoveCouroutine(Vector2 moveDirection, DoorTypes usedDoor)
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

    protected override void TranslateArrowSprite(Vector2 shootDirection,Room previosArrowRoom, Room newCurrentRoom, bool isOpponent = false)
    {
        if (!isOpponent)
            base.TranslateArrowSprite(shootDirection,previosArrowRoom, newCurrentRoom, isOpponent);
        else
        {
            oppponetArrowMap.SetTile(new Vector3Int((int)previosArrowRoom.row, (int)previosArrowRoom.col, 0), null);
            //set the new player base tile
            oppponetArrowMap.SetTile(new Vector3Int((int)newCurrentRoom.row, (int)newCurrentRoom.col, 0), arrowBase);
        }

    }

    protected override void TranslateSprite(Room CurrentPlayerRoom, Room newCurrentRoom, bool isOpponent = false)
    {
        if (!isOpponent)
            base.TranslateSprite(CurrentPlayerRoom, newCurrentRoom, isOpponent);

        else
        {
            //clean the position before the movement 
            opponentMapMap.SetTile(new Vector3Int((int)CurrentPlayerRoom.row, (int)CurrentPlayerRoom.col, 0), null);
            //set the new player base tile
            opponentMapMap.SetTile(new Vector3Int((int)newCurrentRoom.row, (int)newCurrentRoom.col, 0), opponentMapBase);
        }
    }

    protected override void PlayerDeath(RoomType whatKillsPlayer)
    {
        PhotonNetwork.RaiseEvent(OpponentsDeath, null, raiseEventOption, SendOptions.SendReliable);
        base.PlayerDeath(whatKillsPlayer);
    }

    protected override void Teleport()
    {
        Room teleportRoom = playerMovement.Teleport();

        object[] content = new object[] { this.currentRoom, teleportRoom };
        RaiseEventOptions raiseEventOption = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(Translate_Player_Sprite_Event_Code, content, raiseEventOption, SendOptions.SendReliable);
        TranslateSprite(currentRoom, teleportRoom);
        currentRoom = teleportRoom;
        fogUpdater.UpdateFog(teleportRoom); ;
    }

    public void SetMonsterPosition(Room newMonsterPosition)
    {
        //translate sprite
        TranslateMonsterSprite(currentMonsterRoom, newMonsterPosition);

        //delete old blood ui
        UIUpdater.Instance.InitNeightbours(currentMonsterRoom, RoomType.Enemy, true);
        currentMonsterRoom.roomType = RoomType.Normal;
        newMonsterPosition.roomType= RoomType.Enemy;
        currentMonsterRoom = newMonsterPosition;
        //draw the new blood ui
        UIUpdater.Instance.InitNeightbours(newMonsterPosition, RoomType.Enemy);
    }
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == Translate_Arrow_Sprite_Event_Code)
        {
            object[] data = (object[])photonEvent.CustomData;
            Room previousArrowRoom = (Room)data[0];
            Room otherNewArrowRoom = (Room)data[1];
            Vector2 shootDirection = (Vector2)data[2];

            TranslateArrowSprite(shootDirection, previousArrowRoom, otherNewArrowRoom);
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
        if(eventCode == Move_Monster)
        {
            object[] data = (object[])photonEvent.CustomData;
            Room newMonsterRoom = (Room)data[0];
            SetMonsterPosition(newMonsterRoom);
        }
        if (eventCode == OpponentsDeath)
        {
            WinGame("The opponent is dead congratulations");
        }
        if (eventCode == MonsterKilled)
        {
            LoseGame("The opponent killed the monster");
        }
        if (eventCode == NoMoreAmmo)
        {
            WinGame("The opponent has no arrows left to kill the monster");
        }
    }
}
