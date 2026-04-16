using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsButton : MonoBehaviour
{
    public GameObject options;
    public GameObject shop;
    public Button buyButton;
    public bool buyButtonBool = true;


    // void Awake()
    // {
    //     options.SetActive(true);
    // }
    // void Start()
    // {
    //     options.SetActive(false);
    // }


    public void OpenOptionsMenu()
    {
        // Time.timeScale = 0;
        // options.SetActive(true);

        if(options.activeSelf)
        {
            Time.timeScale = 1;
            options.SetActive(false);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("activate settings");
            options.SetActive(true);
            Time.timeScale = 0;
        } 
    }

    public void CloseOptionsMenu()
    {
        Time.timeScale = 1;
        options.SetActive(false);
    }

    public void OpenShopMenu()
    {
        if(options.activeSelf)
        {
            Time.timeScale = 1;
            shop.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            shop.SetActive(true);
            buyButton.interactable = buyButtonBool;
        } 
    }

    public void DisableButton()
    {
        buyButtonBool = false;
    }

    public void CloseShopMenu()
    {
        Time.timeScale = 1;
        shop.SetActive(false);
    }
}
