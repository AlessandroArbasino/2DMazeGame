using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public void SpawnPopUp(string message, string title, string ButtonText, UnityAction function)
    {
        popUp.gameObject.SetActive(true);
        popUp.InitializeFunctionalytyPopUp(message, title, ButtonText, function);
    }
}