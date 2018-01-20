using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMO_Server.Networking
{
    public class LocalDatabase
    {
        public PlayerData[] ActiveClients = new PlayerData[BaseNetwork.MAX_PLAYERS];

        public LocalDatabase()
        {
            for (int i = 0; i < ActiveClients.Length; i++)
            {
                ActiveClients[i] = new PlayerData();
            }
        }

        public void InitiliazeLocalDataContainer(int client_Token, int ClientID)
        {
            for (int i = 0; i < ActiveClients.Length; i++)
            {
                if (!ActiveClients[i].Occupied)
                {
                    ActiveClients[i].ClientID = ClientID;
                    ActiveClients[i].UniqueToken = client_Token;
                    ActiveClients[i].Occupied = true;
                    UnityGameServer.Instance.DebugLog("A new local datacontainer has been created");
                    return;
                }
            }

        }

        public PlayerData GetPlayerByID (int id)
        {
            for (int i = 0; i < ActiveClients.Length; i++)
            {
                if (ActiveClients[i].ClientID == id)
                {
                    UnityGameServer.Instance.DebugLog("At here it has its values: " + ActiveClients[i].UniqueToken);
                    return ActiveClients[i];
                }
            }

            UnityGameServer.Instance.DebugLog("Something went wrong");

            return new PlayerData();
        }

    }

    public class PlayerData
    {
        public int ClientID;
        public int UniqueToken;
        public Vector3 PlayerPosition;
        public bool Occupied;
    }
}
