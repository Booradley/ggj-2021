using Mirror;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace EmeraldActivities
{
    public class HiddenObject : NetworkBehaviour
    {
        [SerializeField]
        private HiddenObjectData _data;
        public HiddenObjectData Data => _data;

        [SerializeField]
        private Interactable _interactable;

        public void Reset()
        {
            _interactable.enabled = true;
        }

        public void HandleAttachedToTarget()
        {
            // Drop it
            if (_interactable.attachedToHand != null)
            {
                _interactable.attachedToHand.DetachObject(_interactable.gameObject);
            }
            
            // Disable it
            _interactable.enabled = false;
        }
    }
}