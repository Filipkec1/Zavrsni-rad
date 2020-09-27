using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoShow : MonoBehaviour
{
    public WeaponSwap weaponSwap;

    public TextMeshProUGUI weaponAmmo;

    private void Awake()
    {
        weaponAmmo.GetComponent<TextMeshPro>();
        weaponSwap.GetComponent<PlayerManager>();
    }

    private void Start()
    {
        weaponAmmo.text = weaponSwap.weapons[weaponSwap.selectedWeapon].currentAmmo.ToString() + "/"
            + weaponSwap.weapons[weaponSwap.selectedWeapon].maxAmmo.ToString();
    }

    private void Update()
    {
        weaponAmmo.text = weaponSwap.weapons[weaponSwap.selectedWeapon].currentAmmo.ToString() + "/"
            + weaponSwap.weapons[weaponSwap.selectedWeapon].maxAmmo.ToString();
    }
}
