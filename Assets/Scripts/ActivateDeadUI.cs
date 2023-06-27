using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateDeadUI : MonoBehaviour
{
    
    void Awake()
    {
        PlayerManager.OnDeath += ShowUI;
    }

    void OnDestroy()
    {
        PlayerManager.OnDeath -= ShowUI;
    }

    private void ShowUI()
    {
        gameObject.SetActive(true);
    }
}
