using ExitGames.Client.Photon;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class Room
{
    public int row;
    public int col;
    public CellType myCellType;
    public RoomType roomType;
    public bool doorTop, doorBot, doorleft, doorRight;
    public List<DoorTypes> doors;

    public bool IsDefinitive;
    public Room(int row, int col, bool isDefinitive)
    {
        this.row = row;
        this.col = col;
        doors = new List<DoorTypes>();
        IsDefinitive = isDefinitive;
    }

    public void FillDoorsList()
    {
        if (doorTop)
            doors.Add(DoorTypes.TopDoor);

        if (doorBot)
            doors.Add(DoorTypes.BottomDoor);

        if (doorleft)
            doors.Add(DoorTypes.LeftDoor);

        if (doorRight)
            doors.Add(DoorTypes.RightDoor);
    }

    public void SetRoomType(RoomType roomType, CellType myCellType)
    {
        this.roomType = roomType;
        this.myCellType = myCellType;
    }

    public static byte[] ObjectToByteArray(System.Object obj)
    {
        if (obj == null)
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);

        return ms.ToArray();
    }

    // Convert a byte array to an Object
    public static Room ByteArrayToObject(byte[] arrBytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        Room obj = (Room)binForm.Deserialize(memStream);

        return obj;
    }

    public static readonly byte[] memVector2 = new byte[2 * 4];
    public static short SerializeVector2(StreamBuffer outStream, object customobject)
    {
        Vector2 vo = (Vector2)customobject;
        lock (memVector2)
        {
            byte[] bytes = memVector2;
            int index = 0;
            Protocol.Serialize(vo.x, bytes, ref index);
            Protocol.Serialize(vo.y, bytes, ref index);
            outStream.Write(bytes, 0, 2 * 4);
        }

        return 2 * 4;
    }

    public static object DeserializeVector2(StreamBuffer inStream, short length)
    {
        Vector2 vo = new Vector2();
        lock (memVector2)
        {
            inStream.Read(memVector2, 0, 2 * 4);
            int index = 0;
            Protocol.Deserialize(out vo.x, memVector2, ref index);
            Protocol.Deserialize(out vo.y, memVector2, ref index);
        }

        return vo;
    }
}
