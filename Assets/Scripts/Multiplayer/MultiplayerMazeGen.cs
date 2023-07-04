using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MultiplayerMazeGen : MonoBehaviourPun
{
    public Vector2 worldSize = new Vector2(20, 20);
    [SerializeField] public Room[,] rooms;
    public List<Vector2> takenPositions = new List<Vector2>();

    protected const byte UpdateTakenPositions = 3;
    protected const byte UpdateRooms = 4;
    protected const byte DrawMapByte = 5;

    int gridSizeX, gridSizeY = 20;
    int numberOfRooms = 40;
    public List<SpawnTypeValues> spawnTypeValues = new List<SpawnTypeValues>();

    public Tilemap DungeonMap;
    public TileBase baseDungeonTile;

    public MultiplayerPlayerManager playerManager;
    public UIUpdater updater;
    public Dictionary<Vector2, Room> roomsPositionType = new Dictionary<Vector2, Room>();

    private void Awake()
    {
        PhotonPeer.RegisterType(typeof(Room), (byte)'M', Room.ObjectToByteArray, Room.ByteArrayToObject);
        PhotonPeer.RegisterType(typeof(Vector2), (byte)'W', Room.SerializeVector2, Room.DeserializeVector2);

        this.rooms = new Room[20 * 2, 20 * 2];
    }
    private void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += SetTakenPositions;
        PhotonNetwork.NetworkingClient.EventReceived += SetRooms;
        PhotonNetwork.NetworkingClient.EventReceived += DrawMapEvent;

        if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2))
        {
            numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y * 2));
        }

        gridSizeX = Mathf.RoundToInt(worldSize.x);
        gridSizeY = Mathf.RoundToInt(worldSize.y);

        if (PhotonNetwork.IsMasterClient)
        {
            CreateRooms();
            SetRoomDoors();
            DrawMap();
            updater.InitUiUpdater(rooms, takenPositions);

            Vector2[] takenPosArray = takenPositions.ToArray();
            object[] positionsContent = new object[] { takenPosArray };
            RaiseEventOptions raiseEventOptionPos = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(UpdateTakenPositions, positionsContent, RaiseEventOptions.Default, SendOptions.SendReliable);


            RaiseEventOptions raiseEventOptionRoom = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

            for (int x = 0; x < (gridSizeX * 2); x++)
            {
                for (int y = 0; y < (gridSizeY * 2); y++)
                {
                    if (rooms[x,y] == null)
                    {
                        continue;
                    }
                    Room roomToSend = rooms[x, y];
                    object[] singleRoomContent = new object[] { x, y, roomToSend };
                    PhotonNetwork.RaiseEvent(UpdateRooms, singleRoomContent, RaiseEventOptions.Default, SendOptions.SendReliable);
                }
            }

            PhotonNetwork.RaiseEvent(DrawMapByte, null, RaiseEventOptions.Default, SendOptions.SendReliable);

        }

    }
    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= SetTakenPositions;
        PhotonNetwork.NetworkingClient.EventReceived -= SetRooms;
        PhotonNetwork.NetworkingClient.EventReceived -= DrawMapEvent;
    }



    private void DrawMap()
    {
        if (PhotonNetwork.IsMasterClient)
            SetMonsterRoom();
        foreach (Room room in rooms)
        {
            if (room == null)
            {
                continue;
            }

            DrawTileByType(room);
        }
    }

    private void DrawTileByType(Room room)
    {
        Vector3Int drawRoomPos = new Vector3Int((int)room.row, (int)room.col, 0);
        DungeonMap.SetTile(drawRoomPos, baseDungeonTile);

        //a tunnel cannot have anything inside maybe ui ?
        if (room.myCellType == CellType.Tunnel)
            return;

        if (room.roomType == RoomType.Start)
            playerManager.InitPlayer(rooms, takenPositions, room);

        Debug.Log(room.roomType);
        foreach (SpawnTypeValues value in spawnTypeValues)
        {
            if (value.type == room.roomType)
            {
                Vector3Int drawPos = new Vector3Int((int)room.row, (int)room.col, 0);
                value.myTypeMap.SetTile(drawPos, value.mybaseTyle);
            }
        }
    }
    private void SetRoomDoors()
    {
        for (int x = 0; x < (gridSizeX * 2); x++)
        {
            for (int y = 0; y < (gridSizeY * 2); y++)
            {
                if (rooms[x, y] == null)
                {
                    continue;
                }

                if (y - 1 < 0)
                {
                    rooms[x, y].doorBot = false;
                }
                else
                {
                    rooms[x, y].doorBot = (rooms[x, y - 1] != null);
                }

                if (y + 1 >= gridSizeY * 2)
                {
                    rooms[x, y].doorTop = false;
                }
                else
                {
                    rooms[x, y].doorTop = (rooms[x, y + 1] != null);
                }

                if (y + 1 < 0)
                {
                    rooms[x, y].doorleft = false;
                }
                else
                {
                    rooms[x, y].doorleft = (rooms[x - 1, y] != null);
                }

                if (x + 1 >= gridSizeX * 2)
                {
                    rooms[x, y].doorRight = false;
                }
                else
                {
                    rooms[x, y].doorRight = (rooms[x + 1, y] != null);
                }

                ChooseRoomType(rooms[x, y]);
            }
        }
    }

    private void CreateRooms()
    {
        //1 for starting room
        rooms[gridSizeX, gridSizeY] = new Room(0, 0, true);
        rooms[gridSizeX, gridSizeY].SetRoomType(RoomType.Start, CellType.Room);
        //roomsPositionType.Add(Vector2.zero, new Room(Vector2.zero, RoomType.Start));

        takenPositions.Insert(0, Vector2.zero);

        float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.01f;

        for (int i = 0; i < numberOfRooms - 1; i++)
        {
            float randomPerc = ((float)i) / (((float)numberOfRooms - 1));
            randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);

            Vector2 checkPos = NewPosition();

            takenPositions.Insert(0, checkPos);
            rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new Room((int)checkPos.x, (int)checkPos.y, false);
        }
    }
    private Vector2 NewPosition()
    {
        int x = 0; int y = 0;
        Vector2 checkingPos = Vector2.zero;

        do
        {
            int index = Mathf.RoundToInt(UnityEngine.Random.value * (takenPositions.Count - 1));
            x = (int)takenPositions[index].x;
            y = (int)takenPositions[index].y;

            bool UpDown = (UnityEngine.Random.value < 0.5f);
            bool positive = (UnityEngine.Random.value < 0.5f);

            if (UpDown)
            {
                if (positive)
                {
                    y += 1;
                }
                else
                {
                    y -= 1;
                }
            }
            else
            {
                if (positive)
                {
                    x += 1;
                }
                else
                {
                    x -= 1;
                }
            }
            checkingPos = new Vector2(x, y);

        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY);
        return checkingPos;
    }


    private void ChooseRoomType(Room currentRoom)
    {
        if (currentRoom.IsDefinitive)
            return;

        int choosedRandom = UnityEngine.Random.Range(1, 100);
        currentRoom.FillDoorsList();

        if (currentRoom.doors.Count == 2)
        {
            currentRoom.SetRoomType(RoomType.Normal, CellType.Tunnel);
            return;
        }

        RoomType chosenType = RoomType.Normal;
        foreach (SpawnTypeValues value in spawnTypeValues)
        {
            if (choosedRandom > value.min && choosedRandom < value.max)
            {
                chosenType = value.type;
                //limit too the first usefull case
                break;
            }
        }
        //roomsPositionType.Add(checkPos, new Room(checkPos, value.type));
        currentRoom.SetRoomType(chosenType, CellType.Room);
    }

    private void SetMonsterRoom()
    {
        Room monsterRoom = null;
        do
        {
            int randomCell = UnityEngine.Random.Range(0, takenPositions.Count);
            monsterRoom = GetMonsterRoom(takenPositions[randomCell]);

        } while (monsterRoom.roomType != RoomType.Normal || monsterRoom.myCellType != CellType.Room);


        if (monsterRoom != null)
            monsterRoom.roomType = RoomType.Enemy;
    }
    private Room GetMonsterRoom(Vector2 newGripPositionIn)
    {
        Room newRoom = null;
        for (int x = 0; x < (gridSizeX * 2); x++)
        {
            for (int y = 0; y < (gridSizeY * 2); y++)
            {
                if (rooms[x, y] == null)
                {
                    continue;
                }
                if (rooms[x, y].row == newGripPositionIn.x && rooms[x, y].col == newGripPositionIn.y)
                {
                    newRoom = rooms[x, y];
                    break;
                }
            }
        }

        return newRoom;
    }

    public void SetTakenPositions(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == UpdateTakenPositions)
        {
            object[] data = (object[])photonEvent.CustomData;
            //Room[,] rooms = (Room[,])data[0];
            List<Vector2> takenPositions = new List<Vector2>((Vector2[])data[0]);
            this.takenPositions = takenPositions;

            //SetRoomsAndDraw(rooms, takenPositions);
        }

    }

    private void SetRooms(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == UpdateRooms)
        {
            object[] data = (object[])photonEvent.CustomData;
            int row = (int)data[0];
            int column = (int)data[1];
            Room sentRoom = (Room)data[2];

            this.rooms[row, column] = sentRoom;

        }
    }

    private void DrawMapEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == UpdateRooms)
        {
            DrawMap();
            updater.InitUiUpdater(rooms, takenPositions);
        }
    }
}

