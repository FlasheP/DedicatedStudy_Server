using System.Numerics;

namespace DedicatedStudy_Server;

class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Console.WriteLine($"{Server.clientsDic[_fromClient].tcp.socket.Client.RemoteEndPoint} connected via TCP. Username : {_username} / ID : {_fromClient}");

        if (_fromClient != _clientIdCheck)
        {
            Console.WriteLine($"Player {_username} has the wrong client ID // fromClientID{_fromClient} is different from checkedID{_clientIdCheck}");
        }
        //플레이어 게임에 접속 시키기.
        Server.clientsDic[_fromClient].SendPlayerInGame(_username);
    }
    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        if (Server.clientsDic[_fromClient].player == null) return;

        float horizontal = _packet.ReadFloat();
        float vertical = _packet.ReadFloat();
        Quaternion rotation = _packet.ReadQuaternion();

        Server.clientsDic[_fromClient].player.SetInput(horizontal, vertical, rotation);
    }
    public static void UDPTestReceived(int _fromClient, Packet _packet)
    {
        string _msg = _packet.ReadString();
        Console.WriteLine(_msg);
    }
}