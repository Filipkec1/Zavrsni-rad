using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwap : MonoBehaviour
{
    public int selectedWeapon = 0;
    int previusSelectedWeapon;
    public int selectedWeaponServer;
    public Weapon[] weapons;

    private void Start()
    {
        previusSelectedWeapon = selectedWeapon;
        ClientSend.PlayerWeaponSelect(0);
    }

    void Update()
    {
        if (!(GameManager.instance.shouldGameRun))
        {
            return;
        }

        if (PauseMenu.instance.inPauseMenu)
        {
            return;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon--;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            selectedWeapon = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
        {
            selectedWeapon = 2;
        }


        if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4)
        {
            selectedWeapon = 3;
        }

        if (previusSelectedWeapon == selectedWeapon)
        {
            return;
        }

        ClientSend.PlayerWeaponSelect(selectedWeapon);
        previusSelectedWeapon = selectedWeapon;
    }

    public void SelectWeapon(int _weaponToSelect)
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == _weaponToSelect)
            {
                weapon.gameObject.SetActive(true);
                selectedWeaponServer = _weaponToSelect;
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }

    public void Shoot()
    {
        weapons[selectedWeaponServer].Shoot();
    }
}
