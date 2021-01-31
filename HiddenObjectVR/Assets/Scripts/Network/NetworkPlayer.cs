using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace EmeraldActivities.Network
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField]
        private GameObject _head;
        public GameObject Head => _head;

        [SerializeField]
        private GameObject _leftHand;
        public GameObject LeftHand => _leftHand;

        [SerializeField]
        private GameObject _rightHand;
        public GameObject RightHand => _rightHand;

        [SerializeField]
        private GameObject _body;

        [SerializeField]
        private GameObject _legs;

        [SerializeField]
        private Vector3 _bodyOffset;

        [SerializeField]
        private Vector3 _legsOffset;

        [SerializeField]
        private float _bodyRotationSpeed;

        [SerializeField]
        private float _legsRotationSpeed;

        private List<Interactable> _interactables = new List<Interactable>();
        private readonly Dictionary<Hand, Interactable> _handLookup = new Dictionary<Hand, Interactable>();

        private void Awake()
        {
            _interactables = FindObjectsOfType<Interactable>().ToList();
            foreach (Interactable interactable in _interactables)
            {
                interactable.onAttachedToHand += HandleInteractablePickedUp;
                interactable.onDetachedFromHand += HandleInteractableDropped;
            }
        }

        private void OnDestroy()
        {
            foreach (Interactable interactable in _interactables)
            {
                interactable.onAttachedToHand -= HandleInteractablePickedUp;
                interactable.onDetachedFromHand -= HandleInteractableDropped;
            }
        }

        private void HandleInteractablePickedUp(Hand hand)
        {
            if (hand.currentAttachedObject != null)
            {
                NetworkIdentity networkIdentity = hand.currentAttachedObject.GetComponentInParent<NetworkIdentity>();
                if (networkIdentity != null)
                {
                    _handLookup[hand] = hand.currentAttachedObject.GetComponent<Interactable>();
                    CmdPickupItem(networkIdentity);
                }
            }
        }

        [Command]
        private void CmdPickupItem(NetworkIdentity networkIdentity)
        {
            networkIdentity.AssignClientAuthority(connectionToClient);
        }

        private void HandleInteractableDropped(Hand hand)
        {
            if (_handLookup.TryGetValue(hand, out Interactable interactable))
            {
                NetworkIdentity networkIdentity = interactable.GetComponentInParent<NetworkIdentity>();
                if (networkIdentity != null)
                {
                    _handLookup.Remove(hand);
                    CmdDropItem(networkIdentity);
                }
            }
        }

        [Command]
        private void CmdDropItem(NetworkIdentity networkIdentity)
        {
            networkIdentity.RemoveClientAuthority();
        }

        private void Update()
        {
            if (hasAuthority)
            {
                if (!Player.instance.rig2DFallback.activeInHierarchy)
                {
                    _head.transform.position = Player.instance.hmdTransform.position;
                    _head.transform.rotation = Player.instance.hmdTransform.rotation;

                    _leftHand.transform.position = Player.instance.leftHand.transform.position;
                    _leftHand.transform.rotation = Player.instance.leftHand.transform.rotation;

                    _rightHand.transform.position = Player.instance.rightHand.transform.position;
                    _rightHand.transform.rotation = Player.instance.rightHand.transform.rotation;

                    _body.transform.position = _head.transform.position + _bodyOffset;
                    _body.transform.rotation = Quaternion.Slerp(_body.transform.rotation, Quaternion.LookRotation(_head.transform.forward), _bodyRotationSpeed * Time.deltaTime);

                    _legs.transform.position = _head.transform.position + _legsOffset;
                    _legs.transform.rotation = Quaternion.Slerp(_legs.transform.rotation, Quaternion.LookRotation(_head.transform.forward), _legsRotationSpeed * Time.deltaTime);
                }
            }
        }
    }
}