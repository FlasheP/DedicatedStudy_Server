using System;
using System.Net;
using System.Net.Sockets;

namespace DedicatedServer_Test1;

public static class Server
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, Client> clientsDic = new Dictionary<int, Client>();
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;
    private static TcpListener tcpListener;
    private static UdpClient udpListener;

    public static void Start(int _maxPlayers, int _port)
    {
        MaxPlayers = _maxPlayers;
        Port = _port;

        Console.WriteLine("Starting Server...");
        InitializeServerData();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        Console.WriteLine($"Server started on port {Port}.");
    }

    private static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        Console.WriteLine($" Incoming connection from {_client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clientsDic[i].tcp.socket == null)
            {
                clientsDic[i].tcp.Connect(_client);
                return;
            }
        }

        Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect : Server is full");
    }
    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if (_data.Length < 4)
            {
                return;
            }

            using (Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();

                if (_clientId == 0) return;

                //새로운 연결이라는 뜻, 이말은 즉슨 해당 클라이언트 쪽 코드에서
                //Client 스크립트에서 UDP 클래스 안의 Connect가 처음 실행하면서 빈 패킷 보낸거임ㅇㅇ
                if (clientsDic[_clientId].udp.endPoint == null)
                {
                    //endPoint만 설정하기 위한 것.
                    clientsDic[_clientId].udp.Connect(_clientEndPoint);
                    return;//빈 패킷이라 HandleData할 필요가 없음.
                }
                //endPoint가 설정된 클라이언트가 패킷을 보냈을때
                if (clientsDic[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                {
                    //HandleData로 데이터 열어보기
                    clientsDic[_clientId].udp.HandleData(_packet);
                }
            }
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Error receiving UDP Data : {_ex}");
        }
    }
    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP {_ex}");
        }
    }
    private static void InitializeServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clientsDic.Add(i, new Client(i));
        }
        //*!나중에 이부분 Reflection 이용하도록 바꾸면 굳이 매번 패킷 만들때마다 일일이 딕셔네리 안써도 될듯.
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ClientPackets.WelcomeReceived, ServerHandle.WelcomeReceived},
            {(int)ClientPackets.UDPTestReceived, ServerHandle.UDPTestReceived}
        };
        Console.WriteLine("Initialized Packets");
    }
}