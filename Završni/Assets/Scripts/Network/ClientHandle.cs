using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Permissions;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        bool _serverAnswer = _packet.ReadBool();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        Client.instance.serverAnswer = _serverAnswer;
        ClientSend.WelcomeReceived();
        GameManager.instance.shouldGameRun = true;

        // Now that we have the client's id, connect UDP
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.players[_id].transform.position = _position;
    }

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.players[_id].transform.rotation = _rotation;
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    public static void PlayerHealth(Packet _packet)
    {
        int _id = _packet.ReadInt();
        float _health = _packet.ReadFloat();

        GameManager.players[_id].SetHealth(_health);
    }

    public static void PlayerRespawned(Packet _packet)
    {
        int _id = _packet.ReadInt();

        GameManager.players[_id].Respawn();
    }

    public static void CreateItemSpawner(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        Vector3 _spawnerPosition = _packet.ReadVector3();
        bool _hasItem = _packet.ReadBool();

        GameManager.instance.CreateItemSpawner(_spawnerId, _spawnerPosition, _hasItem);
    }

    public static void ItemSpawned(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();

        GameManager.itemSpawners[_spawnerId].ItemSpawned();
    }

    public static void ItemPickedUp(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        int _byPlayer = _packet.ReadInt();

        GameManager.itemSpawners[_spawnerId].ItemPickedUp();
        GameManager.players[_byPlayer].itemCount++;
    }

    public static void SpawnProjectile(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        int _thrownByPlayer = _packet.ReadInt();

        GameManager.instance.SpawnProjectile(_projectileId, _position);
        GameManager.players[_thrownByPlayer].itemCount--;
    }

    public static void ProjectilePosition(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.projectiles[_projectileId].transform.position = _position;
    }

    public static void ProjectileExploded(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.projectiles[_projectileId].Explode(_position);
    }

    public static void SetKD(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _kills = _packet.ReadInt();
        int _deaths = _packet.ReadInt();

        GameManager.players[_id].kills = _kills;
        GameManager.players[_id].death = _deaths;
    }

    public static void CreateHealthSpawner(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        Vector3 _spawnerPosition = _packet.ReadVector3();
        bool _hasItem = _packet.ReadBool();

        GameManager.instance.CreateHealthSpawner(_spawnerId, _spawnerPosition, _hasItem);
    }

    public static void HealthSpawned(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();

        GameManager.healthSpawners[_spawnerId].HealthSpawned();
    }

    public static void HealthPickedUp(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        float _healAmount = _packet.ReadFloat();
        int _byPlayer = _packet.ReadInt();

        GameManager.healthSpawners[_spawnerId].HealthPickedUp();
        GameManager.players[_byPlayer].currentHealth = _healAmount;
    }

    public static void SetWeaponOn(Packet _packet)
    {
        int _setWeaponOn = _packet.ReadInt();
        int _byPlayer = _packet.ReadInt();

        GameManager.players[_byPlayer].weaponSwap.SelectWeapon(_setWeaponOn);
    }

    public static void PlayerFire(Packet _packet)
    {
        int _byPlayer = _packet.ReadInt();

        GameManager.players[_byPlayer].weaponSwap.Shoot();
    }

    public static void PlayerGunViewReturn(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.players[_id].weaponSwap.transform.rotation = _rotation;
    }

    public static void PlayerReload(Packet _packet)
    {
        int _id = _packet.ReadInt();
        float _reloadTime = _packet.ReadFloat();
        int _slectedWeapon = _packet.ReadInt();

        
        GameManager.players[_id].weaponSwap.weapons[_slectedWeapon].reloadTime = _reloadTime;
        GameManager.players[_id].weaponSwap.weapons[_slectedWeapon].canReload = true;
    }

    public static void PlayerSendAmmoInfo (Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _selectedWeapon = _packet.ReadInt();
        int _maxAmmo = _packet.ReadInt();
        int _currentAmmo = _packet.ReadInt();

        GameManager.players[_id].weaponSwap.weapons[_selectedWeapon].maxAmmo = _maxAmmo;
        GameManager.players[_id].weaponSwap.weapons[_selectedWeapon].currentAmmo = _currentAmmo;
    }

    public static void GameShouldRun (Packet _packet)
    {
        bool _shouldGameRun = _packet.ReadBool();

        GameManager.instance.shouldGameRun = _shouldGameRun;
    }

    public static void EndGame(Packet _packet)
    {
        bool _shouldGameRun = _packet.ReadBool();
        string _serverMessage = _packet.ReadString();

        GameManager.instance.EndGame(_serverMessage, _shouldGameRun);
    }

    public static void StartGame(Packet _packet)
    {
        bool _shouldGameRun = _packet.ReadBool();

        GameManager.instance.StartGame(_shouldGameRun);
    }
}
