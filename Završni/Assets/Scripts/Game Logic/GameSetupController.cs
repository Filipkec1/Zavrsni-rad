using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupController : MonoBehaviour
{
    //Entities
    public GameObject PlayerObject;

    //SpawnPoints
    public Transform[] PlayerSpawnPoints;

    void Start()
    {

        InstancePlayer();
    }

    private void InstancePlayer()
    {
        int playerSpawnPicker = UnityEngine.Random.Range(0, PlayerSpawnPoints.Length);

        GameObject player = Instantiate(PlayerObject, PlayerSpawnPoints[playerSpawnPicker].position, PlayerSpawnPoints[playerSpawnPicker].rotation);
    }
}
