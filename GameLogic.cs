namespace DedicatedStudy_Server;

public static class GameLogic
{
    public static void Update()
    {
        foreach (Client _client in Server.clientsDic.Values)
        {
            if (_client.player != null)
                _client.player.Update();
        }
        ThreadManager.UpdateMain();
    }
}