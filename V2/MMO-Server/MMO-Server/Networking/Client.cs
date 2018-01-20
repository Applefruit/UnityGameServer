using System;
using System.Net;
using System.Net.Sockets;

namespace MMO_Server.Networking
{
    public class Client
    {
        public int Client_Token;
        public int Client_ID;
        public string Client_IpAdress;

        public TcpClient Client_Socket;
        public NetworkStream Client_Stream;


        private byte[] m_ByteBuffer;

        private const int BUFFERSIZE = 4096;


        public void InitializeClient()
        {
            Client_Socket.ReceiveBufferSize = BUFFERSIZE;
            Client_Socket.SendBufferSize = BUFFERSIZE;

            Client_Stream = Client_Socket.GetStream();

            Array.Resize(ref m_ByteBuffer, BUFFERSIZE);

            Client_Stream.BeginRead(m_ByteBuffer, 0, BUFFERSIZE, OnReceiveData, null);
        }

        private void OnReceiveData(IAsyncResult result)
        {
            try
            {
                int readBytes = Client_Stream.EndRead(result);

                if (readBytes == 0 || Client_Socket == null)
                {
                    CloseConnection();
                    return;
                }

                byte[] tempBuffer = null;

                Array.Resize(ref tempBuffer, readBytes);

                Buffer.BlockCopy(m_ByteBuffer, 0, tempBuffer, 0, readBytes);
                
                NetworkTraffic.Instance.Receiver.HandleData(m_ByteBuffer, Client_ID);

                Client_Stream.BeginRead(m_ByteBuffer, 0, BUFFERSIZE, OnReceiveData, null);

            }
            catch
            {
                CloseConnection();
            }
        }

        private void CloseConnection()
        {
            Client_Socket.Close();
            Client_Socket = null;
        }
    }
}
