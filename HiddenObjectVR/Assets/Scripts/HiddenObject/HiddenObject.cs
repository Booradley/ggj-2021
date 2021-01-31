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

        [SerializeField]
        private Rigidbody _rigidBody;

        [SerializeField]
        private MeshRenderer _renderer;

        private void Start()
        {
            _renderer.material.SetColor("_Color", _data.Color);
            _rigidBody.drag = 2.5f;
            _rigidBody.angularDrag = 1.0f;
        }

        public void Reset()
        {
            _rigidBody.isKinematic = true;
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
            _rigidBody.isKinematic = false;
        }
    }
}