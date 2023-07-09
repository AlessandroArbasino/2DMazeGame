using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public void MuliplayerButton()
    {
        if(!PhotonNetwork.IsConnected)
        PhotonNetwork.ConnectUsingSettings();
        else
            SceneManager.LoadScene("CreateLobby");

    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PopUpManager.Instance.SpawnPopUp("Something went wrong try again", "Error", "Close", delegate { }, PopUpButtonNumbers.ErrorPopUp);
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("CreateLobby");
    }
}
