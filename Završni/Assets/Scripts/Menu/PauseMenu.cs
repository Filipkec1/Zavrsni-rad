using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    public GameObject playerGUI;
    public GameObject pauseMenu;

    public GameObject MainPauseMenu;
    public GameObject OptionsPauseMenu;

    public bool inPauseMenu = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        playerGUI.SetActive(true);
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inPauseMenu)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    
    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        inPauseMenu = false;
        pauseMenu.SetActive(false);
        MainPauseMenu.SetActive(false);
        OptionsPauseMenu.SetActive(false);
        playerGUI.SetActive(true);
    }

    void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        inPauseMenu = true;
        pauseMenu.SetActive(true);
        MainPauseMenu.SetActive(true);
        OptionsPauseMenu.SetActive(false);
        playerGUI.SetActive(false);
    }

    public void DisconnectFromServer()
    {
        Client.instance.Disconnect();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
