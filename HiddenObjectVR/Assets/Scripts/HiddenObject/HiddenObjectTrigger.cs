using System;
using Mirror;
using UnityEngine;

namespace EmeraldActivities
{
    public class HiddenObjectTrigger : NetworkBehaviour
    {
        [SerializeField]
        private HiddenObjectData _targetData;

        private void OnTriggerEnter(Collider other)
        {
            HiddenObject hiddenObject = other.GetComponentInParent<HiddenObject>();
            if (hiddenObject != null)
            {
                if (hiddenObject.Data == _targetData)
                {
                    Debug.Log("Correct Object");
                }
                else
                {
                    Debug.Log("Wrong Object");
                }
            }
        }
    }
}