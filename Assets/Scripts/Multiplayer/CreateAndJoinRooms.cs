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
        PhotonNetwork.CreateRoom(createRoomInput.text, options);
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

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        PopUpManager.Instance.SpawnPopUp(message, "Error", "Close", delegate { }, PopUpButtonNumbers.ErrorPopUp);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);

        PopUpManager.Instance.SpawnPopUp(message, "Error", "Close", delegate { }, PopUpButtonNumbers.ErrorPopUp);
    }

}
