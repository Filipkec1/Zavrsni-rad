﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSpawner : MonoBehaviour
{
    public int spawnerId;
    public bool hasItem;
    public MeshRenderer itemModel;


    public float itemRotationSpeed = 50f;
    public float itemBobSpeed = 2f;
    private Vector3 basePosition;

    private void Update()
    {
        if (hasItem)
        {
            transform.Rotate(Vector3.up, itemRotationSpeed * Time.deltaTime, Space.World);
            transform.position = basePosition + new Vector3(0f, 0.25f * Mathf.Sin(Time.time * itemBobSpeed), 0f);
        }
    }

    public void Initialize(int _spawnerID, bool _hasItem)
    {
        spawnerId = _spawnerID;
        hasItem = _hasItem;
        itemModel.enabled = _hasItem;

        basePosition = transform.position;
    }

    public void HealthSpawned()
    {
        hasItem = true;
        itemModel.enabled = true;
    }

    public void HealthPickedUp()
    {
        hasItem = false;
        itemModel.enabled = false;
    }
}
