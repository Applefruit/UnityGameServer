using System;
using MMO_Server.Networking.Receiving;
using MMO_Server.Networking.Sending;
using MMO_Server.Networking.PackageHandling.Login;


namespace MMO_Server.Networking
{
    public class NetworkTraffic
    {
        public static NetworkTraffic Instance;

        public NetworkReceiver Receiver;
        public NetworkSender Sender;
        public LocalDatabase Database;

        public NetworkTraffic(BaseNetwork baseNetwork)
        {
            Instance = this;

            Sender = new NetworkSender(baseNetwork);
            Receiver = new NetworkReceiver(baseNetwork);

            Database = new LocalDatabase();

            UnityGameServer.Instance.DebugLog("Network traffic is setup!");
        }
    }
}
