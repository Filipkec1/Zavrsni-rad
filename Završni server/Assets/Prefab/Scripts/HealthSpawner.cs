using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSpawner : MonoBehaviour
{
    public static Dictionary<int, HealthSpawner> healthSpawners = new Dictionary<int, HealthSpawner>();
    private static int nextSpawnerId = 1;

    public int healthSpawnerId;
    public bool hasHealth = false;
    public float spawnTimer = 5f;
    public float healAmount = 25f;

    private void Start()
    {
        hasHealth = false;
        healthSpawnerId = nextSpawnerId;
        nextSpawnerId++;
        healthSpawners.Add(healthSpawnerId, this);

        StartCoroutine(SpawnHealth());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHealth && other.CompareTag("Player"))
        {
            Player _player = other.GetComponent<Player>();
            if (_player.AttemptPickupHeal(healAmount))
            {
                HealthPickedUp(_player.id);
            }
        }
    }

    private IEnumerator SpawnHealth()
    {
        yield return new WaitForSeconds(spawnTimer);

        hasHealth = true;
        ServerSend.HealthSpawned(healthSpawnerId);
    }

    private void HealthPickedUp(int _byPlayer)
    {
        hasHealth = false;
        ServerSend.HealthPickedUp(healthSpawnerId, _byPlayer, Server.clients[_byPlayer].player.health);

        StartCoroutine(SpawnHealth());
    }

}
