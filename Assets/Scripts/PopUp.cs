using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PopUpManager;

public abstract class PopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI buttonFunctionText;
    [SerializeField] private Button functionalityButton;
    [SerializeField] private Button OtherButton;
    [SerializeField] private GameObject functionalityButtonContainer;
    public void InitializeSingleButtonPopUp(string message, string title, string functionalityButtonString,bool isError)
    {
        titleText.text = title;
        messageText.text = message;
        buttonFunctionText.text = functionalityButtonString;

        functionalityButtonContainer.SetActive(false);

        if(isError)
        {
            OtherButton.onClick.AddListener(delegate { ClosePopUP(); });
        }
        else
        {
            OtherButton.onClick.AddListener(delegate { BackToMainMenu(); });
        }
    }

    public void InitializeFunctionalytyPopUp(string message, string title, string functionalityButtonString, UnityAction function)
    {
        titleText.text = title;
        messageText.text = message;
        buttonFunctionText.text = functionalityButtonString;
        functionalityButton.onClick.AddListener(function);
        functionalityButtonContainer.SetActive(true);

        functionalityButton.onClick.AddListener(delegate { function(); });
        OtherButton.onClick.AddListener(delegate { BackToMainMenu(); });
    }

    public virtual void BackToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void ClosePopUP()
    {
        gameObject.SetActive(false);
    }
}
