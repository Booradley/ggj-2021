using Mirror;
using UnityEngine;

namespace EmeraldActivities.Network
{
    public class NetworkManager : Mirror.NetworkManager
    {
        /// <summary>
        /// Locking the game to single player by just starting as a host
        /// </summary>
        public override void Start()
        {
            base.Start();

            if (!NetworkClient.active)
            {
                StartHost();
            }
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
            
            Debug.Log($"Player {conn.address} connected!");
        }
        
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            
            Debug.Log($"Player {conn.address} disconnected!");
        }
    }
}