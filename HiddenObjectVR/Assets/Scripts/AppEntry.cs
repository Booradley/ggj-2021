using Mirror;
using UnityEngine;

namespace EmeraldActivities
{
    public class AppEntry : NetworkBehaviour
    {
        [SerializeField]
        private HiddenObjectController _hiddenObjectController;
        
        private void Start()
        {
            _hiddenObjectController.Initialize();
            _hiddenObjectController.HideObjects();
            _hiddenObjectController.OnAllHiddenObjectsAttached += HandleAllHiddenObjectsAttached;
        }

        private void HandleAllHiddenObjectsAttached()
        {
            // TODO: Win presentation
            
            _hiddenObjectController.Reset();
            _hiddenObjectController.HideObjects();
        }
    }
}