using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMPro.TMP_InputField createRoomInput;
    [SerializeField] private TMPro.TMP_InputField JoinLobbyINput;

    private void Awake()
    {
        createRoomInput.onSubmit.AddListener(delegate { CreateRoom(); });
        JoinLobbyINput.onSubmit.AddListener(delegate { JoinRoom(); });
    }

    private void OnDestroy()
    {
        createRoomInput.onSubmit.RemoveListener(delegate { CreateRoom(); });
        JoinLobbyINput.onSubmit.RemoveListener(delegate { JoinRoom(); });
    }
    public void CreateRoom()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        if(!PhotonNetwork.CreateRoom(createRoomInput.text,options))
            PopUpManager.Instance.SpawnPopUp("no internetConnection", "NO INTERNET", "Close", delegate { }, PopUpButtonNumbers.ErrorPopUp);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(JoinLobbyINput.text);
    }

    public override void OnJoinedRoom()
    {
        //missing if 2 players are in the lobby
        if ((PhotonNetwork.CurrentRoom.MaxPlayers != PhotonNetwork.CurrentRoom.PlayerCount))
            PhotonNetwork.LoadLevel("WaitingRoomScene");
        else
            PhotonNetwork.LoadLevel("MultiplayerScene");
    }

}
