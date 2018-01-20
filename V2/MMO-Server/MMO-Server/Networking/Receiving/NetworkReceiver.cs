using MMO_Server.Networking.PackageHandling.Login;
using MMO_Server.Networking.PackageHandling.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMO_Server.Networking.Receiving
{
    public class NetworkReceiver
    {
        private LoginHandler m_LoginHandler;
        private MovementHandler m_MovementHandler;

        //private BaseNetwork m_BaseNetwork;

        public NetworkReceiver(BaseNetwork baseNetwork)
        {
            //m_BaseNetwork = baseNetwork;

            m_LoginHandler = new LoginHandler(baseNetwork);
            m_MovementHandler = new MovementHandler(baseNetwork);
        }

        public void HandleData(byte[] data, int clientID)
        {
            ByteBuffer buffer = new ByteBuffer();

            buffer.WriteBytes(data);

            int packetID = buffer.ReadInteger();

            buffer = null;

            if(packetID == 0)
            {
                //TODO: Handle bad packet;
                return;
            }

            PacketHandler(data, packetID);
            
        }

        private void PacketHandler(byte[] data, int packetID)
        {
            ReceivePackages receivePackages;
            receivePackages = (ReceivePackages)packetID;

            switch (receivePackages)
            {
                case ReceivePackages.ClientLogin:
                    m_LoginHandler.HandleLogin(data);
                    break;

                case ReceivePackages.MovementPackage:
                    m_MovementHandler.HandleMovement(data);
                    break;


                default:
                    UnityGameServer.Instance.DebugLog("A unknown packet number has been received");
                    break;
            }
        }
    }
}
