using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public void MuliplayerButton()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.JoinLobby())
            PopUpManager.Instance.SpawnPopUp("no internetConnection", "NO INTERNET","Close", delegate { }, PopUpButtonNumbers.ErrorPopUp);
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("CreateLobby");
    }
}
