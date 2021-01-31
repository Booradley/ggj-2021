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

        [SerializeField]
        private AnimationCurve _avoidSpeed;

        [SerializeField]
        private AnimationCurve _settleSpeed;

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
                Vector3 avoidPosition = _initialPosition;
                avoidPosition.y += _avoidHeight;
                
                float ratio = (Vector3.Distance(transform.position, avoidPosition) / (avoidPosition.y + _avoidHeight));
                //Debug.Log(ratio);
                
                transform.position = Vector3.Lerp(transform.position, avoidPosition, _avoidSpeed.Evaluate(ratio) * Time.deltaTime);

                if (Mathf.Approximately(ratio, 0.1f))
                {
                    Debug.Log("SETTLE");
                    _isAvoiding = false;
                }
            }
            else
            {
                foreach (KeyValuePair<NetworkConnection, NetworkPlayer> kvp in _playerLookup)
                {
                    NetworkPlayer player = kvp.Value;
                    
                    if (Vector3.Distance(player.Head.transform.position, transform.position) <= _avoidDistance)
                    {
                        _animator.SetTrigger(Swim);
                        Debug.Log("AVOID");
                        _isAvoiding = true;
                        break;
                    }
                }

                float ratio = 1f - (Vector3.Distance(transform.position, _initialPosition) / (_initialPosition.y + _avoidHeight));
                //Debug.Log(ratio);
                
                transform.position = Vector3.Lerp(transform.position, _initialPosition, _settleSpeed.Evaluate(ratio) * Time.deltaTime);
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