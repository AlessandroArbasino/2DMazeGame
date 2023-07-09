using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private static UIUpdater _instance = null;
    public static UIUpdater Instance { get => _instance; }
    public void InitUiUpdater(Room[,] rooms, List<Vector2> takenPositions)
    {
        this.rooms = rooms;
        this.takenPosition = takenPositions;

        UpdateUI();

        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
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

    public void InitNeightbours(Room currentRoom, RoomType controllRoomType, bool isTileToBeDeleted = false)
    {
        switch (controllRoomType)
        {
            case RoomType.Start:
                break;
            case RoomType.Normal:
                break;
            case RoomType.Enemy:
                if (!isTileToBeDeleted)
                    CheckNearRooms(currentRoom, bloodMap, bloodBase);
                else
                    CancelPreviousUITiles(currentRoom, bloodMap);
                break;
            case RoomType.Hole:
                if (!isTileToBeDeleted)
                    CheckNearRooms(currentRoom, mudMap, mudBase);
                else
                    CancelPreviousUITiles(currentRoom, mudMap);
                break;
            case RoomType.Teleport:
                if (!isTileToBeDeleted)
                    CheckNearRooms(currentRoom, windMap, windBase);
                else
                    CancelPreviousUITiles(currentRoom, windMap);
                break;
        }
    }

    public void CheckNearRooms(Room currentRoom, Tilemap uiMap, TileBase uiBase)
    {
        if (currentRoom.doorTop)
        {
            if (takenPosition.Contains(new Vector2((int)currentRoom.row, (int)currentRoom.col + 1)))
                uiMap.SetTile(new Vector3Int((int)currentRoom.row, (int)currentRoom.col + 1, 0), uiBase);
        }


        if (currentRoom.doorBot)
        {
            if (takenPosition.Contains(new Vector2((int)currentRoom.row, (int)currentRoom.col - 1)))
                uiMap.SetTile(new Vector3Int((int)currentRoom.row, (int)currentRoom.col - 1, 0), uiBase);
        }

        if (currentRoom.doorleft)
        {
            if (takenPosition.Contains(new Vector2((int)currentRoom.row - 1, (int)currentRoom.col)))
                uiMap.SetTile(new Vector3Int((int)currentRoom.row - 1, (int)currentRoom.col, 0), uiBase);
        }

        if (currentRoom.doorRight)
        {
            if (takenPosition.Contains(new Vector2((int)currentRoom.row + 1, (int)currentRoom.col)))
                uiMap.SetTile(new Vector3Int((int)currentRoom.row + 1, (int)currentRoom.col, 0), uiBase);
        }
    }

    public void CancelPreviousUITiles(Room currentRoom, Tilemap uiMap)
    {
        if (currentRoom.doorTop)
        {
            if (takenPosition.Contains(new Vector2((int)currentRoom.row, (int)currentRoom.col + 1)))
                uiMap.SetTile(new Vector3Int((int)currentRoom.row, (int)currentRoom.col + 1, 0), null);
        }


        if (currentRoom.doorBot)
        {
            if (takenPosition.Contains(new Vector2((int)currentRoom.row, (int)currentRoom.col - 1)))
                uiMap.SetTile(new Vector3Int((int)currentRoom.row, (int)currentRoom.col - 1, 0), null);
        }

        if (currentRoom.doorleft)
        {
            if (takenPosition.Contains(new Vector2((int)currentRoom.row - 1, (int)currentRoom.col)))
                uiMap.SetTile(new Vector3Int((int)currentRoom.row - 1, (int)currentRoom.col, 0), null);
        }

        if (currentRoom.doorRight)
        {
            if (takenPosition.Contains(new Vector2((int)currentRoom.row + 1, (int)currentRoom.col)))
                uiMap.SetTile(new Vector3Int((int)currentRoom.row + 1, (int)currentRoom.col, 0), null);
        }
    }
}
