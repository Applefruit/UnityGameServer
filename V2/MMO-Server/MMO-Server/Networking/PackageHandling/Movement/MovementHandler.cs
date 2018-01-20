using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMO_Server.Networking.PackageHandling.Movement
{
    public class MovementHandler
    {
        private BaseNetwork m_BaseNetwork;

        public MovementHandler(BaseNetwork baseNetwork)
        {
            m_BaseNetwork = baseNetwork;
        }

        public void HandleMovement(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            int packetId = buffer.ReadInteger();
            int clientId = buffer.ReadInteger();
            Vector3 newPosition = ByteConverters.ByteToVector3(buffer.ReadBytes(12));

            for (int i = 0; i < BaseNetwork.MAX_PLAYERS; i++)
            {
                if(NetworkTraffic.Instance.Database.ActiveClients[i].ClientID == clientId)
                {
                    CheckIfValidPosition(newPosition, i);
                    return;
                }
            }

            buffer = null;
        }

        private void CheckIfValidPosition(Vector3 newPosition, int clientID)
        {
            if (Vector3.GetDistance(NetworkTraffic.Instance.Database.ActiveClients[clientID].PlayerPosition, newPosition) < 52)
            {
                NetworkTraffic.Instance.Database.ActiveClients[clientID].PlayerPosition = newPosition;
                SendUpdatedPositionToAllClientsInGame(newPosition, clientID);
                return;
            }
            else
            {

            }
        }

        private void SendUpdatedPositionToAllClientsInGame(Vector3 position , int clientId)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)SendPackages.TransformPosition);
            buffer.WriteInteger(clientId);
            buffer.WriteBytes(ByteConverters.Vector3ToByteArray(position));

            NetworkTraffic.Instance.Sender.SendToAllClient(buffer.ToArray());

        }
    }
}
