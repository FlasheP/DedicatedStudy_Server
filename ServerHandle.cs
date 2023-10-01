namespace DedicatedServer_Test1;

class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Console.WriteLine($"{Server.clientsDic[_fromClient].tcp.socket.Client.RemoteEndPoint} connected via TCP. Username : {_username} / ID : {_fromClient}");

        if(_fromClient != _clientIdCheck)
        {
            Console.WriteLine($"Player {_username} has the wrong client ID // fromClientID{_fromClient} is different from checkedID{_clientIdCheck}");
        }
        //TODO : 플레이어 게임에 접속 시키기.
    }
    public static void UDPTestReceived(int _fromClient, Packet _packet)
    {
        string _msg = _packet.ReadString();
        Console.WriteLine(_msg);
    }
}