using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[JsonConverter(typeof(StringEnumConverter))]
public enum RoomType
{
    Start,
    Normal,
    Enemy,
    Hole,
    Teleport,
    Arrow
}
[JsonConverter(typeof(StringEnumConverter))]
public enum CellType
{
    Tunnel,
    Room
}
[JsonConverter(typeof(StringEnumConverter))]
public enum DoorTypes
{
    TopDoor,
    BottomDoor,
    LeftDoor,
    RightDoor
}
