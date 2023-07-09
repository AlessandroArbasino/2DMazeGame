using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerPopUp : PopUp
{
    public override void BackToMainMenu()
    {
        PhotonNetwork.LoadLevel("MenuScene");
    }
}
