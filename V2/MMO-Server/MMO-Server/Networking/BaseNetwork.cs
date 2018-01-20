using System;
using System.Net;
using System.Net.Sockets;

namespace MMO_Server.Networking
{
    public class BaseNetwork
    {
        public Client[] GameClients;

        private TcpListener m_ServerListener;

        public const int MAX_PLAYERS = 15;

        public void InitializeServer()
        {
            m_ServerListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5555);
            m_ServerListener.Start();
            m_ServerListener.BeginAcceptTcpClient(OnAcceptNewClient, null);

            InitializeClients();

        }

        private void OnAcceptNewClient(IAsyncResult result)
        {
            TcpClient client = m_ServerListener.EndAcceptTcpClient(result);
            client.NoDelay = false;

            m_ServerListener.BeginAcceptTcpClient(OnAcceptNewClient, null);

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                if(GameClients[i].Client_Socket == null)
                {
                    GameClients[i].Client_ID = i;
                    GameClients[i].Client_Socket = client;
                    GameClients[i].Client_IpAdress = client.Client.RemoteEndPoint.ToString();
                    GameClients[i].InitializeClient();

                    SendClientInitialization(i);

                    UnityGameServer.Instance.DebugLog("A new client has joined! with ID: " + i + " from: " + GameClients[i].Client_IpAdress);

                    return;
                } else
                {
                    //TODO: Handle full server
                }
            }

        }

        private void SendClientInitialization(int ClientID)
        {
            //TODO: Add a update check (Optional lateron)

            ByteBuffer buffer = new ByteBuffer();

            buffer.WriteInteger((int)SendPackages.ClientInitialization);
            buffer.WriteInteger(ClientID);

            NetworkTraffic.Instance.Sender.SendToClientByID(ClientID, buffer.ToArray()); 
        }

        private void InitializeClients()
        {
            GameClients = new Client[MAX_PLAYERS];

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                GameClients[i] = new Client();
            }
            
        }
    }
}
