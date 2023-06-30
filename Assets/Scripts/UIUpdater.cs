using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UIUpdater : MonoBehaviour
{
    [SerializeField] private Tilemap bloodMap;
    [SerializeField] private TileBase bloodBase;

    [SerializeField] private Tilemap windMap;
    [SerializeField] private TileBase windBase;

    [SerializeField] private Tilemap mudMap;
    [SerializeField] private TileBase mudBase;

    private int gridSizeX = 20;
    private int gridSizeY = 20;
    private Room[,] rooms;
    private List<Vector2> takenPosition;
    public void InitUiUpdater(Room[,] rooms, List<Vector2> takenPositions)
    {
        this.rooms = rooms;
        this.takenPosition = takenPositions;

        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int x = 0; x < (gridSizeX * 2); x++)
        {
            for (int y = 0; y < (gridSizeY * 2); y++)
            {
                if (rooms[x, y] == null)
                {
                    continue;
                }

                InitNeightbours(rooms[x, y], rooms[x, y].roomType);
            }
        }
    }

    private void InitNeightbours(Room currentRoom, RoomType controllRoomType)
    {
        switch (controllRoomType)
        {
            case RoomType.Start:
                break;
            case RoomType.Normal:
                break;
            case RoomType.Enemy:
                CheckNearRooms(currentRoom, bloodMap, bloodBase);
                break;
            case RoomType.Hole:
                CheckNearRooms(currentRoom, mudMap, mudBase);
                break;
            case RoomType.Teleport:
                CheckNearRooms(currentRoom, windMap, windBase);
                break;
        }
    }

    public void CheckNearRooms(Room currentRoom, Tilemap uiMap, TileBase uiBase)
    {
        if (currentRoom.doorTop)
        {
            if (takenPosition.Contains(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1)))
                uiMap.SetTile(new Vector3Int((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y + 1, 0), uiBase);
        }


        if (currentRoom.doorBot)
        {
            if (takenPosition.Contains(new Vector2((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1)))
                uiMap.SetTile(new Vector3Int((int)currentRoom.gridPos.x, (int)currentRoom.gridPos.y - 1, 0), uiBase);
        }

        if (currentRoom.doorleft)
        {
            if (takenPosition.Contains(new Vector2((int)currentRoom.gridPos.x - 1, (int)currentRoom.gridPos.y)))
                uiMap.SetTile(new Vector3Int((int)currentRoom.gridPos.x - 1, (int)currentRoom.gridPos.y, 0), uiBase);
        }

        if (currentRoom.doorRight)
        {
            if (takenPosition.Contains(new Vector2((int)currentRoom.gridPos.x + 1, (int)currentRoom.gridPos.y)))
                uiMap.SetTile(new Vector3Int((int)currentRoom.gridPos.x + 1, (int)currentRoom.gridPos.y, 0), uiBase);
        }
    }
}
