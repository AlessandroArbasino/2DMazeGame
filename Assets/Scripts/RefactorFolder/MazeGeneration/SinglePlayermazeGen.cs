using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayermazeGen : MazeGenerationBase
{
    public override void Start()
    {

        gridSizeX = Mathf.RoundToInt(worldSize.x);
        gridSizeY = Mathf.RoundToInt(worldSize.y);

        base.Start();
    }
    public override void DrawMap()
    {
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
}