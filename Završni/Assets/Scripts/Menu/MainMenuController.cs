using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionsMenu;

    void Start()
    {
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }
}
