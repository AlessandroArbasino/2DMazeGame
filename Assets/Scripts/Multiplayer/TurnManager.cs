using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance = null;
    private Inputs myInput;

    protected const byte GetMyTurn = 11;
    protected const byte NewOpponetTurn = 12;

    [SerializeField] private GameObject myTurnGui;
    [SerializeField] private GameObject OpponentTurnGui;

    private void Awake()
    {
        Instance = this;
        myInput = new Inputs();
        myInput.Enable();
        PhotonNetwork.NetworkingClient.EventReceived += OnChangeTurnEvent;

    }


    public Inputs GetInputClass() { return myInput; }
    public void BeginTurnMessage()
    {
        StartTurn();
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOption = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(GetMyTurn, content, raiseEventOption, SendOptions.SendReliable);
    }


    public void StartTurn()
    {
        EnableInput();
        ActivateTurnUI(true);
    }

    public void EndTurnMessage()
    {
        EndTurn();
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOption = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(NewOpponetTurn, content, raiseEventOption, SendOptions.SendReliable);
    }

    public void EndTurn()
    {
        DisableInput();
        ActivateTurnUI(false);
    }
    public void ActivateTurnUI(bool isMyTurn)
    {
        myTurnGui.SetActive(isMyTurn);
        OpponentTurnGui.SetActive(!isMyTurn);
    }

    public void DisableInput()
    {
        myInput.Player.Disable();
    }

    public void EnableInput()
    {
        myInput.Player.Enable();
    }

    public void OnChangeTurnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == GetMyTurn)
        {
            EndTurn();
        }
        if (eventCode == NewOpponetTurn)
        {
            StartTurn();
        }
    }
}
