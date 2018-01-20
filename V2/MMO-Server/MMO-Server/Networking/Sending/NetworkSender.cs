using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMO_Server.Networking.Sending
{
    public class NetworkSender
    {
        private BaseNetwork m_BaseNetwork;

        public NetworkSender(BaseNetwork baseNetwork)
        {
            m_BaseNetwork = baseNetwork;
        }

        public void SendToClientByID(int clientID, byte[] data)
        {
            ByteBuffer byteBuffer = new ByteBuffer();
            byteBuffer.WriteBytes(data);

            if (m_BaseNetwork.GameClients[clientID].Client_Socket != null)
                m_BaseNetwork.GameClients[clientID].Client_Stream.BeginWrite(byteBuffer.ToArray(), 0, byteBuffer.ToArray().Length, null, null);
            else
            {
                //TODO: Handle the closed socket
            }
            

            byteBuffer = null;
        }

        public async void SendToAllClient(byte[] data)
        {
            for (int i = 0; i < BaseNetwork.MAX_PLAYERS; i++)
            {
                if(m_BaseNetwork.GameClients[i].Client_Socket != null)
                {
                    await Task.Delay(90);
                    SendToClientByID(i, data);
                }
            }
        }

        public void SendToAllExceptClient(byte[] data, int exceptionID)
        {
            for (int i = 0; i < BaseNetwork.MAX_PLAYERS; i++)
            {
                if (m_BaseNetwork.GameClients[i].Client_Socket != null)
                {
                    if(m_BaseNetwork.GameClients[i].Client_ID != exceptionID)
                    {
                        SendToClientByID(i, data);
                    }
                }
            }
        }
    }
}
