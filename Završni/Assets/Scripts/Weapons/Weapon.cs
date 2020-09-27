using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public float range = 100f;
    public float reloadTime;
    public bool canReload = false;
    public bool isReloading = false;

    //ammo
    public int maxAmmo;
    public int currentAmmo;

    public AudioSource audioSourceFire;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public GameObject viewDirection;
    public Animator animator;

    private void Awake()
    {
        weaponName = gameObject.name;
    }

    private void Update()
    {
        if (isReloading)
        {
            return;
        }

        if (canReload)
        {
            canReload = false;
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButtonDown("Reload"))
        {
            ClientSend.PlayerInitiateReload(true);
        }
    }

    public void Shoot()
    {
        if (!(GameManager.instance.shouldGameRun))
        {
            return;
        }

        muzzleFlash.Play();
        audioSourceFire.Play();

        RaycastHit hit;
        if (Physics.Raycast(viewDirection.transform.position, viewDirection.transform.forward, out hit, range))
        {
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 0.5f);
        }
    }
   
    
    IEnumerator Reload()
    {
        isReloading = true;

        animator.SetBool("Reloading", true);

        yield return new WaitForSeconds(reloadTime - 0.25f);
        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(0.25f);

        isReloading = false;
    }

}
