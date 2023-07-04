using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PopUpButtonNumbers
{
    ErrorPopUp,
    PlayAgainMainMenuPopUp,
    MainMenuPopUp
}
public class PopUpManager : MonoBehaviour
{
    private static PopUpManager _instance = null;
    public static PopUpManager Instance { get => _instance; }

    [SerializeField] private PopUp popUp;
    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }

    public void SpawnPopUp(string message, string title, string ButtonText, UnityAction function, PopUpButtonNumbers popUpButtonNumbers)
    {
        popUp.gameObject.SetActive(true);
        switch (popUpButtonNumbers)
        {
            case PopUpButtonNumbers.ErrorPopUp:
                popUp.InitializeSingleButtonPopUp(message, title, ButtonText, true);
                break;
            case PopUpButtonNumbers.MainMenuPopUp:
                popUp.InitializeSingleButtonPopUp(message, title, ButtonText, false);
                break;
            case PopUpButtonNumbers.PlayAgainMainMenuPopUp:
                popUp.InitializeFunctionalytyPopUp(message, title, ButtonText, function);
                break;
        }
    }
}