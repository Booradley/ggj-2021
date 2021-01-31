using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using Valve.VR.InteractionSystem;
using NetworkManager = EmeraldActivities.Network.NetworkManager;

namespace EmeraldActivities
{
    public class AppEntry : NetworkBehaviour
    {
        [SerializeField]
        private OpenVRLoader _openVRLoader;
        
        [SerializeField]
        private NetworkManager _networkManager;
        
        [SerializeField]
        private HiddenObjectController _hiddenObjectController;

        [SerializeField]
        private FishController[] _fishControllers;

        private void Start()
        {
            _networkManager.OnPlayerAdded += HandlePlayerAdded;
            _networkManager.OnPlayerRemoved += HandlePlayerRemoved;
            
            _hiddenObjectController.Initialize();
            _hiddenObjectController.HideObjects();
            _hiddenObjectController.OnAllHiddenObjectsAttached += HandleAllHiddenObjectsAttached;

            foreach (FishController fishController in _fishControllers)
            {
                fishController.Initialize();
            }
        }

        private void HandlePlayerAdded(NetworkConnection conn, GameObject player)
        {
            foreach (FishController fishController in _fishControllers)
            {
                fishController.AddPlayer(conn, player);
            }
        }

        private void HandlePlayerRemoved(NetworkConnection conn)
        {
            foreach (FishController fishController in _fishControllers)
            {
                fishController.RemovePlayer(conn);
            }
        }

        private void HandleAllHiddenObjectsAttached()
        {
            // TODO: Win presentation
            
            _hiddenObjectController.Reset();
            _hiddenObjectController.HideObjects();
        }
    }
}