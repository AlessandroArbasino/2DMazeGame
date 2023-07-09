using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWadUpdater : MonoBehaviour
{
    [SerializeField] private Tilemap fogOfWarTileMap;
    [SerializeField] private TileBase fogBase;

    public void UpdateFog(Room discoveredRoom)
    {
        //clean all the visited tiles from the fog
        fogOfWarTileMap.SetTile(new Vector3Int((int)discoveredRoom.row, (int)discoveredRoom.col, 0), null);
    }

    public bool CheckFogTile(Room controllRoom)
    {
        return fogOfWarTileMap.HasTile(new Vector3Int((int)controllRoom.row, (int)controllRoom.col, 0));
    }
}
