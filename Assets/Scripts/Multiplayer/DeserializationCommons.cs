using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeserializationCommons 
{
    public static Room[] SendArray2D(Room[,] room2DArray)
    {
        
        Room[] rooomFlatArray = room2DArray.Cast<Room>().ToArray();
        return rooomFlatArray;
    }

    public static Room[,] ReceiveArray2D(int elementSize, Room[] arrayFlat)
    {
        var elementCount = arrayFlat.Length / elementSize;
        Room[,]roomArray2D = new Room[elementCount, elementSize];
        for (var x = 0; x < elementCount; x++)
        {
            for (int y = 0; y < elementSize; y++)
            {
                roomArray2D[x, y] = arrayFlat[x * elementSize + y];
            }
        }

        return roomArray2D;
    }
}
