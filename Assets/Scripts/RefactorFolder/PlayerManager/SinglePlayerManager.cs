using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class SinglePlayerManager : PlayerManagerbase
{
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
            TranslateArrowSprite(currentArrowRoom, newArrowRoom);

            if (newArrowRoom.roomType == RoomType.Enemy)
            {
                WinGame("Hero you defeat the terrible monster");
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
            TurnManager.Instance.EnableInput();
            yield return new WaitForSeconds(.2f);
            MoveMethod(moveDirection, entry.entryDoor);
        }
        else
            TurnManager.Instance.EnableInput();
    }
}
