using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;

    public float maxHealth;
    public float currentHealth;
    public int itemCount = 0;

    public int kills;
    public int death;

    public WeaponSwap weaponSwap;
    public MeshRenderer model;
    public CapsuleCollider capsuleCollider;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        currentHealth = maxHealth;
    }

    public void SetHealth(float _currentHealth)
    {
        currentHealth = _currentHealth;

        if(currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        model.enabled = false;
        weaponSwap.gameObject.SetActive(false);
    }

    public void Respawn()
    {
        model.enabled = true;
        SetHealth(maxHealth);
        weaponSwap.gameObject.SetActive(true);
    }

}
