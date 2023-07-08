using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonFunctions : MonoBehaviour
{
    public void PlaySinglePlayerMatch()
    {
        SceneManager.LoadScene("SinglePlayerScene");
    }

    public void PlayMultiPlayerMatch()
    {

    }
}
