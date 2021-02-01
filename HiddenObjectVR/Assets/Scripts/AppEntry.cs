using System.Collections;
using Mirror;
using UnityEngine;
using Valve.VR;
using NetworkManager = EmeraldActivities.Network.NetworkManager;

namespace EmeraldActivities
{
    public class AppEntry : NetworkBehaviour
    {
        [SerializeField]
        private NetworkManager _networkManager;
        
        [SerializeField]
        private HiddenObjectController _hiddenObjectController;

        [SerializeField]
        private FishController[] _fishControllers;

        [SerializeField]
        private OctopusController[] _octopusControllers;

        [SerializeField]
        AudioSource _audioSource;
        
        [SerializeField]
        AudioClip _winAudio;

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

            foreach (OctopusController octopusController in _octopusControllers)
            {
                octopusController.AddPlayer(conn, player);
            }
        }

        private void HandlePlayerRemoved(NetworkConnection conn)
        {
            foreach (FishController fishController in _fishControllers)
            {
                fishController.RemovePlayer(conn);
            }

            foreach (OctopusController octopusController in _octopusControllers)
            {
                octopusController.RemovePlayer(conn);
            }
        }

        private void HandleAllHiddenObjectsAttached()
        {
            StartCoroutine(WinSequence());
        }

        private IEnumerator WinSequence()
        {
            _audioSource.PlayOneShot(_winAudio);
            
            yield return new WaitForSeconds(5f);
            
            SteamVR_Fade.Start( Color.clear, 0 );
            SteamVR_Fade.Start( Color.black, 8f );
            
            yield return new WaitForSeconds(8f);
            
            _audioSource.Play();

            _hiddenObjectController.Reset();
            _hiddenObjectController.HideObjects();
            
            yield return new WaitForSeconds(1.5f);
            
            SteamVR_Fade.Start( Color.clear, 1.5f );
        }
    }
}