using System;
using Mirror;
using UnityEngine;

namespace EmeraldActivities.Network
{
    public class NetworkManager : Mirror.NetworkManager
    {
        public Action<NetworkConnection, GameObject> OnPlayerAdded;
        public Action<NetworkConnection> OnPlayerRemoved;
        
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
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

            NetworkServer.AddPlayerForConnection(conn, player);
            
            OnPlayerAdded?.Invoke(conn, player);
            
            Debug.Log($"Player {conn.address} connected!");
        }
        
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            
            OnPlayerRemoved?.Invoke(conn);
            
            Debug.Log($"Player {conn.address} disconnected!");
        }
    }
}