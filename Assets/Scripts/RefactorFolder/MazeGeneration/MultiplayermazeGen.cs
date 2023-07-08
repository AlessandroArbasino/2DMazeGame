using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayermazeGen : MazeGenerationBase
{
    protected const byte UpdateTakenPositions = 3;
    protected const byte UpdateRooms = 4;
    protected const byte DrawMapByte = 5;

    public override void Awake()
    {
        base.Awake();

        PhotonPeer.RegisterType(typeof(Room), (byte)'M', Room.ObjectToByteArray, Room.ByteArrayToObject);
        PhotonPeer.RegisterType(typeof(Vector2), (byte)'W', Room.SerializeVector2, Room.DeserializeVector2);
    }
    public override void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += SetTakenPositions;
        PhotonNetwork.NetworkingClient.EventReceived += SetRooms;
        PhotonNetwork.NetworkingClient.EventReceived += DrawMapEvent;

        if (PhotonNetwork.IsMasterClient)
            base.Start();

        if (PhotonNetwork.IsMasterClient)
        {
            Vector2[] takenPosArray = takenPositions.ToArray();
            object[] positionsContent = new object[] { takenPosArray };
            RaiseEventOptions raiseEventOptionPos = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(UpdateTakenPositions, positionsContent, RaiseEventOptions.Default, SendOptions.SendReliable);


            RaiseEventOptions raiseEventOptionRoom = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

            for (int x = 0; x < (gridSizeX * 2); x++)
            {
                for (int y = 0; y < (gridSizeY * 2); y++)
                {
                    if (rooms[x, y] == null)
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

    public override void DrawMap()
    {
        if(PhotonNetwork.IsMasterClient)
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

