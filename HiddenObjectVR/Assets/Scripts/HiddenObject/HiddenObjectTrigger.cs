using System;
using Mirror;
using UnityEngine;

namespace EmeraldActivities
{
    public class HiddenObjectTrigger : NetworkBehaviour
    {
        public Action<HiddenObject> OnHiddenObjectAttached;
        
        [SerializeField]
        private HiddenObjectData _targetData;

        [SerializeField]
        private Transform _attachPoint;

        [SerializeField]
        private GameObject _slotObject;

        private HiddenObject _attachedObject;

        private void Start()
        {
            _slotObject.GetComponentInChildren<Renderer>().material.SetColor("_Color", _targetData.Color);
        }

        public void Reset()
        {
            _attachedObject = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_attachedObject != null)
                return;
            
            HiddenObject hiddenObject = other.GetComponentInParent<HiddenObject>();
            if (hiddenObject != null)
            {
                if (hiddenObject.Data == _targetData)
                {
                    _attachedObject = hiddenObject;
                    _attachedObject.HandleAttachedToTarget();
                    _attachedObject.transform.position = _attachPoint.position;
                    _attachedObject.transform.rotation = _attachPoint.rotation;
                    
                    OnHiddenObjectAttached?.Invoke(_attachedObject);
                    
                    // TODO: Sound for placed object
                }
                else
                {
                    // TODO: Sound for invalid object
                }
            }
        }
    }
}