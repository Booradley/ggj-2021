using Mirror;
using UnityEngine;

namespace EmeraldActivities.Network
{
    public class NetworkContextToggle : MonoBehaviour
    {
        [SerializeField]
        private NetworkIdentity _networkIdentity;
        
        [SerializeField]
        private Behaviour[] _componentsToDisable;

        [SerializeField]
        private bool _clientOnly;

        private void Awake()
        {
            if (_clientOnly && !_networkIdentity.isClient || !_clientOnly && _networkIdentity.isClient)
            {
                foreach (Behaviour component in _componentsToDisable)
                {
                    Debug.Log($"Disabling {component.name}, Client = {_networkIdentity.isClient}");
                    component.enabled = false;
                }
            }
        }
    }
}