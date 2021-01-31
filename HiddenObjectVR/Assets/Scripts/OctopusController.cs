using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using NetworkPlayer = EmeraldActivities.Network.NetworkPlayer;

namespace EmeraldActivities
{
    public class OctopusController : NetworkBehaviour
    {
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private float _avoidDistance;

        [SerializeField]
        private float _avoidHeight;

        private bool _isAvoiding;
        
        private readonly Dictionary<NetworkConnection, NetworkPlayer> _playerLookup = new Dictionary<NetworkConnection, NetworkPlayer>();
        private static readonly int Swim = Animator.StringToHash("Swim");
        private Vector3 _initialPosition;

        private void Start()
        {
            _initialPosition = transform.position;
        }

        private void Update()
        {
            if (_isAvoiding)
            {
                
            }
            else
            {
                foreach (KeyValuePair<NetworkConnection, NetworkPlayer> kvp in _playerLookup)
                {
                    NetworkPlayer player = kvp.Value;

                    if (Vector3.Distance(player.Head.transform.position, transform.position) <= _avoidDistance)
                    {
                        _animator.SetTrigger(Swim);
                        
                        _isAvoiding = true;
                        break;
                    }
                }

                transform.position = Vector3.Lerp(transform.position, _initialPosition, Time.deltaTime);
            }
        }

        public void AddPlayer(NetworkConnection conn, GameObject player)
        {
            NetworkPlayer networkPlayer = player.GetComponentInChildren<NetworkPlayer>();
            if (networkPlayer != null)
            {
                _playerLookup.Add(conn, networkPlayer);
            }
        }

        public void RemovePlayer(NetworkConnection conn)
        {
            _playerLookup.Remove(conn);
        }
    }
}