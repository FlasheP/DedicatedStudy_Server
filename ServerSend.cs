using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DedicatedStudy_Server;

public static class ServerSend
{
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clientsDic[_toClient].tcp.SendData(_packet);
    }
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        foreach (var client in Server.clientsDic.Values)
            client.tcp.SendData(_packet);
    }
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        foreach (var client in Server.clientsDic)
        {
            if (client.Key != _exceptClient)
                client.Value.tcp.SendData(_packet);
        }
    }
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clientsDic[_toClient].udp.SendData(_packet);
    }
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        foreach (var client in Server.clientsDic.Values)
            client.udp.SendData(_packet);
    }
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        foreach (var client in Server.clientsDic)
        {
            if (client.Key != _exceptClient)
                client.Value.udp.SendData(_packet);
        }
    }
    #region Packets
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.Welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }
    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.SpawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.position);
            _packet.Write(_player.rotation);

            //플레이어 스폰은 한번만 실행 하고 손실 없이 반드시 실행되어야해서 확실하게 하기위해 tcp로 보낸다.
            SendTCPData(_toClient, _packet);
        }

    }
    public static void UDPTest(int _toClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.UDPTest))
        {
            _packet.Write("UDP Test Message Sent from Server");

            SendUDPData(_toClient, _packet);
        }
    }

    public static void PlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.PlayerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.position);

            SendUDPDataToAll(_packet);
        }
    }

    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.PlayerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.rotation);

            SendUDPDataToAll(_player.id, _packet);
        }
    }
    #endregion
}