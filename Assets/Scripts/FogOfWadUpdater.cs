using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWadUpdater : MonoBehaviour
{
    [SerializeField] private Tilemap fogOfWarTileMap;
    [SerializeField] private TileBase fogBase;

    public void UpdateFog(List<Room>playerVisistedRooms)
    {
        foreach(Room room in playerVisistedRooms)
        {
            //clean all the visited tiles from the fog
            fogOfWarTileMap.SetTile(new Vector3Int((int)room.gridPos.x, (int)room.gridPos.y, 0),null);
        }
    }
}
