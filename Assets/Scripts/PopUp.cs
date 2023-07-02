using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PopUpManager;

public class PopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI buttonFunctionText;
    [SerializeField] private Button functionalityButton;
    [SerializeField] private GameObject functionalityButtonContainer;
    public void InitializeGenericPopUp(string message, string title, string functionalityButtonString)
    {
        titleText.text = title;
        messageText.text = message;
        buttonFunctionText.text = functionalityButtonString;

        functionalityButtonContainer.SetActive(false);
    }

    public void InitializeFunctionalytyPopUp(string message, string title, string functionalityButtonString, UnityAction function)
    {
        titleText.text = title;
        messageText.text = message;
        buttonFunctionText.text = functionalityButtonString;
        functionalityButton.onClick.AddListener(function);
        functionalityButtonContainer.SetActive(true);

        functionalityButton.onClick.AddListener(delegate { BackToMainMenu(); });
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
