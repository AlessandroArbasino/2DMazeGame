using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class MazeGenerationBase : MonoBehaviour
{
    #region Variables
    public Vector2 worldSize = new Vector2(20, 20);
	[SerializeField] public Room[,] rooms;
	public List<Vector2> takenPositions = new List<Vector2>();

	public int gridSizeX = 10;
	public int gridSizeY = 10;
	protected int numberOfRooms = 40;
	[SerializeField] protected int minNumberOfRooms = 100;
	public List<SpawnTypeValues> spawnTypeValues = new List<SpawnTypeValues>();

	public Tilemap DungeonMap;
	public TileBase baseDungeonTile;

	public PlayerManagerbase playerManager;
	public UIUpdater updater;
	public Dictionary<Vector2, Room> roomsPositionType = new Dictionary<Vector2, Room>();
    #endregion
    public virtual void Awake()
	{
		this.rooms = new Room[20 * 2, 20 * 2];
		numberOfRooms = Random.Range(minNumberOfRooms, (int)worldSize.x * (int)worldSize.y);

		gridSizeX = (int)worldSize.x / 2;
		gridSizeY = (int)worldSize.y / 2;
	}

	public virtual void Start()
	{
		CreateRooms();
		SetRoomDoors();
		DrawMap();
		updater.InitUiUpdater(rooms, takenPositions);
	}


	public virtual void DrawMap()
	{

	}

	protected void DrawTileByType(Room room)
	{
		Vector3Int drawRoomPos = new Vector3Int((int)room.row, (int)room.col, 0);
		DungeonMap.SetTile(drawRoomPos, baseDungeonTile);

		//a tunnel cannot have anything inside maybe ui ?
		if (room.myCellType == CellType.Tunnel)
			return;

		if (room.roomType == RoomType.Start)
			playerManager.InitPlayer(rooms, takenPositions, room);

		foreach (SpawnTypeValues value in spawnTypeValues)
		{
			if (value.type == room.roomType)
			{
				Vector3Int drawPos = new Vector3Int((int)room.row, (int)room.col, 0);
				value.myTypeMap.SetTile(drawPos, value.mybaseTyle);

				if(room.roomType == RoomType.Enemy)
					playerManager.SetMonsterRoom(room);
			}
		}
	}

    /**
	* Sets connections between rooms
	*/
    protected void SetRoomDoors()
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

				if (x - 1 < 0)
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

    /**
	* Crates the array of rooms
	*/
    protected void CreateRooms()
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

	protected Vector2 NewPosition()
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
				if (positive)
					y += 1;
				else
					y -= 1;
			else
				if (positive)
				x += 1;
			else
				x -= 1;
			checkingPos = new Vector2(x, y);

		} while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY);
		return checkingPos;
	}

	protected void ChooseRoomType(Room currentRoom)
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

	/**
	* Randomize the monster position
	*/
	protected void SetMonsterRoom()
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

    /**
	* Gets the monster room by its vector 2D coordinates
	* 
	* @Param newGridPositionIn 
	*/
    protected Room GetMonsterRoom(Vector2 newGridPositionIn)
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
				if (rooms[x, y].row == newGridPositionIn.x && rooms[x, y].col == newGridPositionIn.y)
				{
					newRoom = rooms[x, y];
					break;
				}
			}
		}

		return newRoom;
	}
}
