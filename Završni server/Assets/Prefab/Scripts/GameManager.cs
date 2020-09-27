using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public int killGoal;

    public bool shouldGameRun = true;
    public float restartTime = 3f;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }
        else if (gameManager != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void EndGame(Player player)
    {
        shouldGameRun = false;
        string serverMessage = "Player " + player.username + " has won the match";

        ServerSend.EndGame(shouldGameRun, serverMessage);

        Debug.Log(serverMessage);
        StartCoroutine(RestartGame());
    }

    public void StartGame()
    {
        shouldGameRun = true;

        for (int i = 1; i <= Server.MaxPlayers; i++)
        {

            if (Server.clients[i].player == null)
            {
                continue;
            }

            Server.clients[i].player.PlayerNewPosition();
            Server.clients[i].player.killCount = 0;
            Server.clients[i].player.deathCount = 0;
            Server.clients[i].player.health = Server.clients[i].player.maxHealth;
            Server.clients[i].player.weaponHolder.SetAmmoToAll();

            ServerSend.SetKD(i, Server.clients[i].player.killCount, Server.clients[i].player.deathCount);
        }

        ServerSend.StartGame(shouldGameRun);
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(restartTime);

        StartGame();
    }

}
