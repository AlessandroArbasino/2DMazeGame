using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMPro.TMP_InputField createRoomInput;
    [SerializeField] private TMPro.TMP_InputField JoinLobbyINput;
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoomInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(JoinLobbyINput.text);
    }

    public override void OnJoinedLobby()
    {
        //missing if 2 players are in the lobby
        PhotonNetwork.LoadLevel("GameScene");
    }

}
