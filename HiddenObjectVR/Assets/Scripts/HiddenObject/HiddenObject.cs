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
        private Throwable _throwable;

        [SerializeField]
        private Rigidbody _rigidBody;

        [SerializeField]
        private MeshRenderer _renderer;

        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private AudioClip _collectObjSound;

        [SerializeField]
        private AudioClip _droppedObjSound;

        private void Start()
        {
            _renderer.material.SetColor("_Color", _data.Color);
            _renderer.material.EnableKeyword("_EMISSION");
            _renderer.material.SetColor("_EmissionColor", Color.black);
            
            _rigidBody.drag = 2.5f;
            _rigidBody.angularDrag = 1.0f;

            _interactable.onAttachedToHand += hand => PlayCollectSound(hand);
            _interactable.onDetachedFromHand += hand => PlayDropSound(hand);
        }

        public void Reset()
        {
            _rigidBody.isKinematic = false;
            _interactable.enabled = true;
            _throwable.enabled = true;
            
            _renderer.material.SetColor("_EmissionColor", Color.black);
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
            _throwable.enabled = false;
            _rigidBody.isKinematic = true;
            
            _renderer.material.SetColor("_EmissionColor", _data.Color);
        }

        public void PlaySound(AudioClip clip){
            _audioSource.PlayOneShot(clip);
        }

        private void PlayCollectSound(Hand hand){
            PlaySound(_collectObjSound);
        }

        private void PlayDropSound(Hand hand){
            PlaySound(_droppedObjSound);
        }
    }
}