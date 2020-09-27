using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TMPro.TMP_InputField ipInput;

    public GameObject serverError;

    public string ipAddress;
    public int port;

    public GameObject startMenu;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        serverError.SetActive(false);
    }

    /// <summary>Attempts to connect to the server.</summary>
    public void ConnectToServer()
    {
        if (string.IsNullOrEmpty(ipInput.text))
        {
            return;
        }

        string[] subStrings = ipInput.text.Split(':');

        if(subStrings.Length > 2)
        {
            return;
        }

        ipAddress = subStrings[0];
        port = Int32.Parse(subStrings[1]);

        startMenu.SetActive(false);
        Client.instance.ConnectToServer();
    }

    public void ConnectionError()
    {
        startMenu.SetActive(true);
        serverError.SetActive(true);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
