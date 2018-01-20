using System;
using System.Threading;
using System.Threading.Tasks;

namespace MMO_Server.Networking.PackageHandling.Login
{
    public class LoginHandler
    {
        private int m_ClientID;
        private int m_LoginToken;
        private int m_PacketID;

        BaseNetwork m_baseNetwork;

        public LoginHandler(BaseNetwork baseNetwork)
        {
            m_baseNetwork = baseNetwork;
        }

        public void HandleLogin(Byte[] data)
        {
            ByteBuffer byteBuffer = new ByteBuffer();
            byteBuffer.WriteBytes(data);

            m_PacketID = byteBuffer.ReadInteger();
            m_ClientID = byteBuffer.ReadInteger();
            m_LoginToken = byteBuffer.ReadInteger();

            Parallel.Invoke(() => ProcessLogin(m_ClientID, m_LoginToken));
            //Task.Factory.StartNew(() => ProcessLogin(m_ClientID, m_LoginToken));
            
        }

        private void ProcessLogin(int clientID, int loginToken)
        {
            Parallel.For(0, BaseNetwork.MAX_PLAYERS,
                (index, State) => {
                    if (index == clientID)
                    {
                        if (m_baseNetwork.GameClients[index].Client_Socket != null)
                        {
                            NetworkTraffic.Instance.Database.InitiliazeLocalDataContainer(m_LoginToken, clientID);
                            CreatePlayer(clientID);
                            UnityGameServer.Instance.DebugLog("Client: " + loginToken + " has logged in");
                            State.Break();
                        }
                        else
                        {
                            //TODO: Handle loss of connection
                        }
                    }
                    }
                );
        }

        private void CreatePlayer(int clientID)
        {
            ByteBuffer byteBuffer = new ByteBuffer();
            byteBuffer.WriteInteger((int)SendPackages.CreateMainPlayer);
            byteBuffer.WriteBytes(ByteConverters.Vector3ToByteArray(new Vector3(2, 2, 1)));

            PlayerData data = NetworkTraffic.Instance.Database.GetPlayerByID(clientID);
            data.PlayerPosition = new Vector3(2, 2, 1);

            NetworkTraffic.Instance.Sender.SendToClientByID(clientID, byteBuffer.ToArray());

            CreatePlayerOnOtherClients(clientID, data.PlayerPosition);
        }

        private void CreatePlayerOnOtherClients(int exceptionID, Vector3 pos)
        {
            ByteBuffer byteBuffer = new ByteBuffer();
            byteBuffer.WriteInteger((int)SendPackages.CreateOnOtherClient);
            byteBuffer.WriteInteger(exceptionID);
            
            byteBuffer.WriteBytes(ByteConverters.Vector3ToByteArray(pos));

            NetworkTraffic.Instance.Sender.SendToAllExceptClient(byteBuffer.ToArray(), exceptionID);

            LoadOtherActiveClients(exceptionID);
        }

        private void LoadOtherActiveClients(int id)
        {
            for (int i = 0; i < m_baseNetwork.GameClients.Length; i++)
            {
                if (m_baseNetwork.GameClients[i].Client_Socket != null)
                {

                    if (m_baseNetwork.GameClients[i].Client_ID != id)
                    {
                        PlayerData data = NetworkTraffic.Instance.Database.GetPlayerByID(i);

                        ByteBuffer byteBuffer = new ByteBuffer();
                        byteBuffer.WriteInteger((int)SendPackages.LoadOtherClients);
                        byteBuffer.WriteInteger(data.ClientID);
                        byteBuffer.WriteBytes(ByteConverters.Vector3ToByteArray(data.PlayerPosition)); 
                        ActualLoad(id, byteBuffer);                        
                    }
                }
            }
        }

        private async void ActualLoad(int id, ByteBuffer buffer)
        {
            await Task.Delay(500);
            NetworkTraffic.Instance.Sender.SendToClientByID(id, buffer.ToArray());
        }
    }
}
