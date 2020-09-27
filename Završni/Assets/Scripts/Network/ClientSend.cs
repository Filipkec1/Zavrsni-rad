using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    /// <summary>Sends a packet to the server via TCP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to the server via UDP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    /// <summary>Lets the server know that the welcome message was received.</summary>
    public static void WelcomeReceived()
    {

        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(PlayerPrefs.GetString("username"));

            SendTCPData(_packet);
        }
    }

    /// <summary>Sends player input to the server.</summary>
    /// <param name="_inputs"></param>
    public static void PlayerMovement(bool[] _inputs)
    {

        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }
            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

            SendUDPData(_packet);
        }
    }

    public static void PlayerShoot(Vector3 _facing, bool _isShooting)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerShoot))
        {
            _packet.Write(_facing);
            _packet.Write(_isShooting);

            SendTCPData(_packet);
        }
    }

    public static void PlayerThrowItem(Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerThrowItem))
        {
            _packet.Write(_facing);

            SendTCPData(_packet);
        }
    }

    public static void PlayerGrappleStart(Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerGrappleStart))
        {
            _packet.Write(_facing);

            SendTCPData(_packet);
        }
    }

    public static void PlayerGrappleEnd()
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerGrappleEnd))
        {
            SendTCPData(_packet);
        }
    }

    public static void PlayerWeaponSelect(int _selectedWeapon)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerWeaponSelect))
        {
            _packet.Write(_selectedWeapon);

            SendTCPData(_packet);
        }
    }

    public static void PlayerGunView(Quaternion _camView)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerGunView))
        {
            _packet.Write(_camView);

            SendUDPData(_packet);
        }
    }

    public static void PlayerInitiateReload(bool _canReloading)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerInitiateReload))
        {
            _packet.Write(_canReloading);

            SendTCPData(_packet);
        }
    }

    #endregion
}
