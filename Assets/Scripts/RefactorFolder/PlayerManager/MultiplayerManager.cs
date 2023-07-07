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

    protected override void TranslateArrowSprite(Room previosArrowRoom, Room newCurrentRoom, bool isOpponent = false)
    {
        if (!isOpponent)
            base.TranslateArrowSprite(previosArrowRoom, newCurrentRoom, isOpponent);
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
        base.PlayerDeath(whatKillsPlayer);

        PhotonNetwork.RaiseEvent(OpponentsDeath, null, raiseEventOption, SendOptions.SendReliable);
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
