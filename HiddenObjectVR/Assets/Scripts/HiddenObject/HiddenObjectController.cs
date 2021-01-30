using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EmeraldActivities
{
    [Serializable]
    public class HiddenObjectController
    {
        public Action OnAllHiddenObjectsAttached;
        
        [SerializeField]
        private Transform _hidingPlaces;
        
        private Dictionary<Transform, HiddenObject> _hidingPlaceLookup = new Dictionary<Transform, HiddenObject>();
        private HiddenObject[] _hiddenObjects;
        private HiddenObjectTrigger[] _hiddenObjectTriggers;
        private int _hiddenObjectsAttached;
        
        public void Initialize()
        {
            _hiddenObjects = GameObject.FindObjectsOfType<HiddenObject>();
            _hiddenObjectTriggers = GameObject.FindObjectsOfType<HiddenObjectTrigger>();

            if (_hidingPlaces.childCount < _hiddenObjects.Length)
            {
                Debug.LogError("Not enough hiding places!");
                return;
            }

            if (_hiddenObjectTriggers.Length < _hiddenObjects.Length)
            {
                Debug.LogError("Not enough attach points!");
                return;
            }

            foreach (Transform hidingPlace in _hidingPlaces)
            {
                _hidingPlaceLookup.Add(hidingPlace, null);
            }

            foreach (HiddenObjectTrigger hiddenObjectTrigger in _hiddenObjectTriggers)
            {
                hiddenObjectTrigger.OnHiddenObjectAttached += HandleHiddenObjectAttached;
            }

            _hiddenObjectsAttached = 0;
        }

        public void Reset()
        {
            List<Transform> allKeys = _hidingPlaceLookup.Keys.ToList();
            foreach (Transform hidingPlace in allKeys)
            {
                _hidingPlaceLookup[hidingPlace]?.Reset();
                _hidingPlaceLookup[hidingPlace] = null;
            }

            foreach (HiddenObjectTrigger hiddenObjectTrigger in _hiddenObjectTriggers)
            {
                hiddenObjectTrigger.Reset();
            }

            _hiddenObjectsAttached = 0;
        }

        public void HideObjects()
        {
            List<Transform> availableKeys = _hidingPlaceLookup.Keys.ToList();
            foreach (HiddenObject hiddenObject in _hiddenObjects)
            {
                int index = Random.Range(0, availableKeys.Count);
                Transform hidingPlace = availableKeys[index];
                
                _hidingPlaceLookup[hidingPlace] = hiddenObject;
                availableKeys.RemoveAt(index);

                hiddenObject.transform.position = hidingPlace.position;
                hiddenObject.transform.rotation = hidingPlace.rotation;
            }
        }

        private void HandleHiddenObjectAttached(HiddenObject hiddenObject)
        {
            _hiddenObjectsAttached++;
            
            if (_hiddenObjectsAttached == _hiddenObjects.Length)
            {
                OnAllHiddenObjectsAttached?.Invoke();
            }
        }
    }
}