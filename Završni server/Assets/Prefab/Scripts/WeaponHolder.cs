using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Weapon[] weaponArray;
    public Player player;

    public int selectedWeapon = 0;
    int previousWeapon = 0;

    private void FixedUpdate()
    {
        ServerSend.PlayerGunViewReturn(player.id, transform.rotation);

        if(selectedWeapon == previousWeapon)
        {
            return;
        }

        SelectWeapon();
    }

    void SelectWeapon()
    {
        int i = 0;
        previousWeapon = selectedWeapon;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
                ServerSend.SetWeaponOn(i, player.id);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }

    public void Shoot(Vector3 _viewDirection, int _id, bool _isShooting)
    {
        weaponArray[selectedWeapon].isShooting = _isShooting;
        weaponArray[selectedWeapon].viewDirection = _viewDirection;
        weaponArray[selectedWeapon].id = _id;
        weaponArray[selectedWeapon].playerId = player.id;
    }

    public void Reload(bool _canReloading)
    {
        weaponArray[selectedWeapon].canReloading = _canReloading;
    }

    public void SetAmmoToAll()
    {
        for(int i = 0; i < weaponArray.Length; i++)
        {
            weaponArray[i].currentAmmo = weaponArray[i].maxAmmo;
        }
    }
}
