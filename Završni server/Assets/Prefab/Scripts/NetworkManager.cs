using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject projectilePrefab;
    public static NetworkManager instance;

    public int playerAmount = 4;
    public int portNumber = 26950;

    public Transform playerSpawns;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(playerAmount, portNumber);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        int playerSpawnPicker;
        Vector3 playerSpawnLocation = new Vector3();
        Quaternion playerSpawnRotation = new Quaternion();
        bool canAllow = true;

        do
        {
            playerSpawnPicker = Random.Range(0, playerSpawns.childCount);

            for (int i = 0; i < playerSpawns.childCount; i++)
            {
                playerSpawns.GetChild(i).GetComponent<BoxCollider>().enabled = true;
                if (playerSpawns.GetChild(i).GetComponent<IsTouching>().isTouching)
                {
                    Debug.Log("TEST");
                    break;
                }
                playerSpawns.GetChild(i).GetComponent<BoxCollider>().enabled = false;

                if (playerSpawnPicker == i)
                {
                    playerSpawnLocation = playerSpawns.GetChild(i).transform.position;
                    playerSpawnRotation = playerSpawns.GetChild(i).transform.rotation;
                    canAllow = false;
                    break;
                }
            }

        } while (canAllow);

        Player newPlayer = Instantiate(playerPrefab, playerSpawnLocation, playerSpawnRotation).GetComponent<Player>();
        newPlayer.playerSpawns = playerSpawns;

        return newPlayer;
    }

    public Projectile InstantiateProjectile(Transform _shootOrigin)
    {
        return Instantiate(projectilePrefab, _shootOrigin.position + _shootOrigin.forward * 0.7f, Quaternion.identity).GetComponent<Projectile>();
    }
}
