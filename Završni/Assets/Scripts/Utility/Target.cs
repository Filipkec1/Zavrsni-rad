using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float maxhealth;
    public float currentHealth;

    public bool isDead = false;

    void Start()
    {
        currentHealth = maxhealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}
