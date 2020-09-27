using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //enable
    public string weaponName;
    public bool isShooting = false;

    //stats
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    public float impactForce = 20f;

    //fire type
    private float nextTimeToFire = 0f;
    public bool isAutomatic = true;
    public bool canFire = true;

    //ammo
    public int maxAmmo = 10;
    public int currentAmmo;
    public float reloadTime = 2f;

    //shoot
    public GameObject playerViewDirection;
    public Vector3 viewDirection;
    public int id;
    public int playerId;

    //reload
    public bool canReloading = false;
    public bool isReloading = false;
    public WeaponHolder weaponHolder;


    void Awake()
    {
        weaponHolder.GetComponent<WeaponHolder>();
        weaponName = gameObject.name;
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    private void FixedUpdate()
    {
        SendAmmoInfo();

        if (!(GameManager.gameManager.shouldGameRun))
        {
            isShooting = false;
            return;
        }


        if (isReloading)
        {
            return;
        }

        if(currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if(canReloading && currentAmmo < maxAmmo)
        {
            canReloading = false;
            StartCoroutine(Reload());
            return;
        }

        if (isAutomatic)
        {
            if (isShooting && (Time.time >= nextTimeToFire))
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        else
        {
            if (isShooting && (Time.time >= nextTimeToFire) && canFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                canFire = false;
                Shoot();
            }

            if (!isShooting)
            {
                canFire = true;
            }
        }
    }

    public void Shoot()
    {
        ServerSend.PlayerFire(playerId);
        currentAmmo--;

        if (Physics.Raycast(playerViewDirection.transform.position, viewDirection, out RaycastHit _hit, range))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(damage, id);
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        ServerSend.PlayerReload(weaponHolder.player.id ,reloadTime, weaponHolder.selectedWeapon);

        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    public void SendAmmoInfo()
    {
        ServerSend.PlayerSendAmmoInfo(weaponHolder.player.id, weaponHolder.selectedWeapon, maxAmmo, currentAmmo);
    }

}
