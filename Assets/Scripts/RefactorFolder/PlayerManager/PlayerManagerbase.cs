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

    [SerializeField] protected Tilemap arrowMap;
    [SerializeField] protected TileBase arrowBase;

    [SerializeField] protected FogOfWadUpdater fogUpdater;

    protected Room currentRoom;

    [SerializeField] protected int currentArrowNumber = 5;
    [SerializeField] protected TextMeshProUGUI currentArrowNumberText;
    protected virtual void Start()
    {
        TurnManager.Instance.GetInputClass().Player.Shoot.started += OnShot;
        TurnManager.Instance.GetInputClass().Player.Move.started += OnMove;

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

    }

    public virtual void OnShot(InputAction.CallbackContext context)
    {
        if (currentArrowNumber > 0)
        {
            ShootMethod(context.ReadValue<Vector2>(), DoorTypes.TopDoor, currentRoom);
            currentArrowNumber--;
            currentArrowNumberText.text = $"Remainig Arrows : {currentArrowNumber.ToString()}";
        }
    }

    protected void ShootMethod(Vector2 shootDirection, DoorTypes usedDoor, Room currentArrowRoom)
    {
        StartCoroutine(ShootCouroutine(shootDirection, usedDoor, currentArrowRoom));
    }

    protected virtual IEnumerator ShootCouroutine(Vector2 shootDirection, DoorTypes usedDoor, Room currentArrowRoom)
    {
        yield return null;
    }

    protected void WinGame(string popUpMessage)
    {
        TurnManager.Instance.DisableInput();

        PopUpManager.Instance.SpawnPopUp(popUpMessage, "WIN", "PlayAgain", delegate { PlayAgain(); }, PopUpButtonNumbers.MainMenuPopUp);
    }

    protected void OnMove(InputAction.CallbackContext context)
    {
        MoveMethod(context.ReadValue<Vector2>(), DoorTypes.TopDoor);
        if (TurnManager.Instance != null)
            TurnManager.Instance.EndTurnMessage();
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
    }

    protected virtual void TranslateArrowSprite(Room previosArrowRoom, Room newCurrentRoom, bool isOpponent = false)
    {
        //clean the position before the movement 
        arrowMap.SetTile(new Vector3Int((int)previosArrowRoom.row, (int)previosArrowRoom.col, 0), null);
        //set the new player base tile
        arrowMap.SetTile(new Vector3Int((int)newCurrentRoom.row, (int)newCurrentRoom.col, 0), arrowBase);

    }

    protected virtual void PlayerDeath(RoomType whatKillsPlayer)
    {
        TurnManager.Instance.DisableInput();

        switch (whatKillsPlayer)
        {
            case RoomType.Enemy:
                LoseGame("The monster kills you");
                break;
            case RoomType.Hole:
                LoseGame("You fall into an endless hole");
                break;
        }
    }

    protected void LoseGame(string message)
    {
        TurnManager.Instance.DisableInput();
        PopUpManager.Instance.SpawnPopUp(message, "Defeat", "PlayAgain", delegate { PlayAgain(); }, PopUpButtonNumbers.MainMenuPopUp);
    }

    protected virtual Room Teleport()
    {
        Room teleportRoom = playerMovement.Teleport();
        TranslateSprite(currentRoom, teleportRoom);
        currentRoom = teleportRoom;
        fogUpdater.UpdateFog(teleportRoom);

        return teleportRoom;
    }

    protected void PlayAgain()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("GameScene");
    }
}

