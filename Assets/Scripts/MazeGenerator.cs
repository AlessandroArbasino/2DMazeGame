using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public Vector2 worldSize = new Vector2(20, 20);
    public GameObject roomPrefab;

   public  Room[,] rooms;

    public List<Vector2> takenPositions = new List<Vector2>();

    int gridSizeX, gridSizeY, numberOfRooms = 20;

    private void Start()
    {
        if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2))
        {
            numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y * 2));
        }

        gridSizeX = Mathf.RoundToInt(worldSize.x);
        gridSizeY = Mathf.RoundToInt(worldSize.y);

        CreateRooms();
        SetRoomDoors();
        DrawMap();
    }

    private void DrawMap()
    {
        foreach(Room room in rooms)
        {
            if(room == null)
            {
                continue;
            }

            Vector2 drawPos = room.gridPos;
            //drawPos.x *= 16;// size map sprite
            //drawPos.y *= 8;

            GameObject mapper = Instantiate(roomPrefab, drawPos, Quaternion.identity);

            //MapSpriteSelector mapper = Instantiate(roomPrefab, drawPos, Quaternion.identity).GetComponent<MapSpriteSelector>();

            //mapper.type = room.type;
            //mapper.up = room.doorTop;
            //mapper.down = room.doorBot;
            //mapper.left = room.doorleft;
            //mapper.right = room.doorRight;
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
                Vector2 gridPosition = new Vector2(x, y);

                if (y - 1 < 0)//check above
                {
                    rooms[x, y].doorBot = false;
                }
                else
                {
                    rooms[x, y].doorBot = (rooms[x, y - 1] != null);
                }

                if (y + 1 >= gridSizeY * 2)//check bellow
                {
                    rooms[x, y].doorTop = false;
                }
                else
                {
                    rooms[x, y].doorTop = (rooms[x, y + 1] != null);
                }

                if (y + 1 < 0)//check left
                {
                    rooms[x, y].doorleft = false;
                }
                else
                {
                    rooms[x, y].doorleft = (rooms[x - 1, y] != null);
                }

                if (x + 1 >= gridSizeX * 2)//check right
                {
                    rooms[x, y].doorRight = false;
                }
                else
                {
                    rooms[x, y].doorRight = (rooms[x + 1, y] != null);
                }
            }
        }
    }

    private void CreateRooms()
    {
        rooms = new Room[gridSizeX * 2, gridSizeY * 2];
        //1 for starting room
        rooms[gridSizeX, gridSizeY] = new Room(Vector2.zero, RoomType.Start);

        takenPositions.Insert(0, Vector2.zero);
        Vector2 checkPos = Vector2.zero;

        float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.01f;

        for (int i = 0; i < numberOfRooms - 1; i++)
        {
            float randomPerc = ((float)i) / (((float)numberOfRooms - 1));
            randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);

            checkPos = NewPosition();

            //if (NumberOfNeightbors(checkPos, takenPositions) > 1 && UnityEngine.Random.value > randomCompare)
            //{
            //    int iteration = 0;
            //    do
            //    {
            //        checkPos = SelectiveNewPosition();
            //        iteration++;
            //    } while (NumberOfNeightbors(checkPos, takenPositions) > 1 && iteration < 100);


            //}

            // other  r0om type logic 
            rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new Room(checkPos, RoomType.Start);
            takenPositions.Insert(0, checkPos);
        }
    }

    private Vector2 SelectiveNewPosition()
    {
        throw new NotImplementedException();
    }

    private int NumberOfNeightbors(Vector2 checkPos, List<Vector2> usedPosition)
    {
        int ret = 0;
        if (usedPosition.Contains(checkPos + Vector2.right))
        {
            ret++;
        }
        if (usedPosition.Contains(checkPos + Vector2.left))
        {
            ret++;
        }
        if (usedPosition.Contains(checkPos + Vector2.up))
        {
            ret++;
        }
        if (usedPosition.Contains(checkPos + Vector2.down))
        {
            ret++;
        }
        return ret;
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

}
