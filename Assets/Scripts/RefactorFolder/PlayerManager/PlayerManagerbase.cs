using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public abstract class PlayerManagerbase : MonoBehaviour
{

    protected PlayerMovement playerMovement;
    protected PlayerShoot playerShoot;

    [SerializeField] protected Tilemap playerMap;
    [SerializeField] protected TileBase playerBase;

    [SerializeField] protected Tilemap monsterMap;
    [SerializeField] protected TileBase monsterBase;

    [SerializeField] protected Tilemap arrowMap;
    [SerializeField] protected TileBase arrowBase;

    [SerializeField] protected FogOfWadUpdater fogUpdater;

    protected Room currentRoom;
    protected Room currentMonsterRoom;

    [SerializeField] protected int currentArrowNumber = 5;
    [SerializeField] protected TextMeshProUGUI currentArrowNumberText;

    [SerializeField] protected Transform playerPos;
    protected virtual void Start()
    {
        TurnManager.Instance.GetInputClass().Player.Shoot.started += OnShot;
        TurnManager.Instance.GetInputClass().Player.Move.started += OnMove;

        currentArrowNumberText.text = $"Remaining Arrows: {currentArrowNumber.ToString()}";
    }

    protected virtual void OnDestroy()
    {
        TurnManager.Instance.GetInputClass().Player.Shoot.started += OnShot;
        TurnManager.Instance.GetInputClass().Player.Move.started += OnMove;

    }
    public virtual void InitPlayer(Room[,] rooms, List<Vector2> takenPositions, Room currentRoom)
    {
        playerMovement = new PlayerMovement(rooms, takenPositions);
        playerShoot = new PlayerShoot(rooms, takenPositions);
        fogUpdater.UpdateFog(currentRoom);
        this.currentRoom = currentRoom;

        playerPos.position = new Vector3(currentRoom.row, currentRoom.col, 0);
    }

    public void SetMonsterRoom(Room monsterRoom)
    {
        this.currentMonsterRoom = monsterRoom;
    }
    public virtual void OnShot(InputAction.CallbackContext context)
    {
        if (currentArrowNumber > 0)
        {
            ShootMethod(context.ReadValue<Vector2>(), DoorTypes.TopDoor, currentRoom);
            currentArrowNumber--;
            currentArrowNumberText.text = $"Remaining Arrows: {currentArrowNumber.ToString()}";
        }

        TurnManager.Instance.DisableInput();
    }

    protected void ShootMethod(Vector2 shootDirection, DoorTypes usedDoor, Room currentArrowRoom)
    {
        StartCoroutine(ShootCouroutine(shootDirection, usedDoor, currentArrowRoom));
    }

    protected virtual IEnumerator ShootCouroutine(Vector2 shootDirection, DoorTypes usedDoor, Room currentArrowRoom)
    {
        yield return null;
    }

    protected virtual void WinGame(string popUpMessage)
    {
        TurnManager.Instance.DisableInput();

        PopUpManager.Instance.SpawnPopUp(popUpMessage, "WIN", "Play Again", delegate { PlayAgain(); }, PopUpButtonNumbers.MainMenuPopUp);
    }

    protected virtual void OnMove(InputAction.CallbackContext context)
    {
        MoveMethod(context.ReadValue<Vector2>(), DoorTypes.TopDoor);

    }

    protected void MoveMethod(Vector2 moveDirection, DoorTypes usedDoor)
    {
        StartCoroutine(MoveCouroutine(moveDirection, usedDoor));
    }

    protected virtual IEnumerator MoveCouroutine(Vector2 moveDirection, DoorTypes usedDoor)
    {
        yield return null;
    }

    protected virtual void TranslateSprite(Room CurrentPlayerRoom, Room newCurrentRoom, bool isOpponent = false)
    {
        //clean the position before the movement 
        playerMap.SetTile(new Vector3Int((int)CurrentPlayerRoom.row, (int)CurrentPlayerRoom.col, 0), null);
        //set the new player base tile
        playerMap.SetTile(new Vector3Int((int)newCurrentRoom.row, (int)newCurrentRoom.col, 0), playerBase);

        playerPos.position = new Vector3(newCurrentRoom.row, newCurrentRoom.col, 0);
    }
    protected virtual Room NewMonsterPosition()
    {
        Room newMonsterPosition = null;
        do
        {
            Vector2 chosenGripPositoin = playerMovement.RandomizePositionInTakenPosition();

            newMonsterPosition = playerMovement.GetNextRoom(chosenGripPositoin);
        } while (!fogUpdater.CheckFogTile(newMonsterPosition)&& newMonsterPosition.myCellType != CellType.Tunnel);

        //translate sprite
        TranslateMonsterSprite(currentMonsterRoom, newMonsterPosition);

        //delete old blood ui
        UIUpdater.Instance.InitNeightbours(currentMonsterRoom, RoomType.Enemy,true);
        //changing room type according to the translate
        currentMonsterRoom.roomType = RoomType.Normal;
        newMonsterPosition.roomType = RoomType.Enemy;
        currentMonsterRoom = newMonsterPosition;
        //draw the new blood ui
        UIUpdater.Instance.InitNeightbours(newMonsterPosition, RoomType.Enemy);
        return newMonsterPosition;
    }
    protected void TranslateMonsterSprite(Room previousRoom, Room newMonsterRoom)
    {
        monsterMap.SetTile(new Vector3Int((int)previousRoom.row, (int)previousRoom.col, 0), null);

        monsterMap.SetTile(new Vector3Int((int)newMonsterRoom.row, (int)newMonsterRoom.col, 0), monsterBase);
    }
    protected virtual void TranslateArrowSprite(Room previosArrowRoom, Room newCurrentRoom, bool isOpponent = false)
    {

        arrowMap.SetTile(new Vector3Int((int)previosArrowRoom.row, (int)previosArrowRoom.col, 0), null);

        arrowMap.SetTile(new Vector3Int((int)newCurrentRoom.row, (int)newCurrentRoom.col, 0), arrowBase);

    }

    protected virtual void PlayerDeath(RoomType whatKillsPlayer)
    {
        TurnManager.Instance.DisableInput();

        switch (whatKillsPlayer)
        {
            case RoomType.Enemy:
                LoseGame("The monster killed you");
                break;
            case RoomType.Hole:
                LoseGame("You fell into an endless hole");
                break;
            case RoomType.Arrow:
                LoseGame("Your arrow killed you");
                break;
        }
    }

    protected virtual void LoseGame(string message)
    {
        TurnManager.Instance.DisableInput();
        PopUpManager.Instance.SpawnPopUp(message, "Defeat", "Play Again", delegate { PlayAgain(); }, PopUpButtonNumbers.MainMenuPopUp);
    }

    protected virtual void Teleport()
    {
        Room teleportRoom = playerMovement.Teleport();
        TranslateSprite(currentRoom, teleportRoom);
        currentRoom = teleportRoom;
        fogUpdater.UpdateFog(teleportRoom);
    }

    protected void PlayAgain()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("SinglePlayerScene");
    }
}

