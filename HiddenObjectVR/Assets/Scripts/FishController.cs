using System.Collections.Generic;
using Mirror;
using UnityEngine;
using NetworkPlayer = EmeraldActivities.Network.NetworkPlayer;
using Random = UnityEngine.Random;

namespace EmeraldActivities
{
    public class FishController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _fishPrefab;

        [SerializeField]
        private int _numberOfFish;

        [SerializeField]
        private float _radius = 4.0f;

        [SerializeField]
        private float _minSpeed = 0.1f;

        [SerializeField]
        private float _maxSpeed = 0.2f;

        [SerializeField]
        private float _rotationSpeed = 4.0f;
        
        [SerializeField]
        private float _neighborDistance = 2.0f;
        
        [SerializeField]
        private float _playerDistance = 0.25f;

        [SerializeField]
        private float _playerAvoidRotationSpeed = 10.0f;

        [SerializeField]
        private float _playerAvoidSpeed = 5.0f;

        private readonly Dictionary<GameObject, FishData> _fishLookup = new Dictionary<GameObject, FishData>();
        private readonly Dictionary<NetworkConnection, NetworkPlayer> _playerLookup = new Dictionary<NetworkConnection, NetworkPlayer>();
        private Vector3 _goal;

        public void Initialize()
        {
            _goal = transform.position;
            
            for (int i = 0; i < _numberOfFish; i++)
            {
                GameObject fish = Instantiate(_fishPrefab, transform.position + (Random.insideUnitSphere * _radius), Quaternion.identity);
                fish.transform.SetParent(transform);
                _fishLookup.Add(fish, new FishData(Random.Range(_minSpeed, _maxSpeed)));
            }
        }

        private void Update()
        {
            if (_fishLookup.Count == 0)
                return;
            
            if (Random.Range(0, 1000) < 3)
            {
                _goal = transform.position + (Random.insideUnitSphere * _radius);
            }
            
            foreach (KeyValuePair<GameObject, FishData> kvp in _fishLookup)
            {
                GameObject fish = kvp.Key;
                FishData fishData = kvp.Value;

                fishData.IsTurning = Vector3.Distance(fish.transform.position, transform.position) >= _radius;

                if (fishData.IsTurning)
                {
                    Vector3 direction = transform.position - fish.transform.position;
                    fish.transform.rotation = Quaternion.Slerp(fish.transform.rotation, Quaternion.LookRotation(direction), _rotationSpeed * Time.deltaTime);
                    fishData.Speed = Random.Range(_minSpeed, _maxSpeed);
                }
                else if (Random.Range(0, 5) < 1)
                {
                    ApplyRules(fish);
                }

                AvoidPlayers(fish);
                
                fish.transform.Translate(0, 0, Time.deltaTime * fishData.Speed);
            }
        }

        private void ApplyRules(GameObject currentFish)
        {
            Vector3 center = Vector3.zero;
            Vector3 avoid = Vector3.zero;
            float speed = 0f;
            int groupSize = 0;
            
            foreach (KeyValuePair<GameObject, FishData> kvp in _fishLookup)
            {
                GameObject fish = kvp.Key;
                FishData fishData = kvp.Value;
                
                if (fish != currentFish)
                {
                    float distance = Vector3.Distance(fish.transform.position, currentFish.transform.position);
                    if (distance <= _neighborDistance)
                    {
                        center += fish.transform.position;
                        groupSize++;

                        if (distance < 1.0f)
                        {
                            avoid += (currentFish.transform.position - fish.transform.position);
                        }

                        speed += fishData.Speed;
                    }
                }
            }

            if (groupSize > 0)
            {
                center = center / groupSize + (_goal - currentFish.transform.position);
                _fishLookup[currentFish].Speed = speed / groupSize;

                Vector3 direction = (center + avoid) - currentFish.transform.position;
                if (direction != Vector3.zero)
                {
                    currentFish.transform.rotation = Quaternion.Slerp(currentFish.transform.rotation, Quaternion.LookRotation(direction), _rotationSpeed * Time.deltaTime);
                }
            }
        }

        private void AvoidPlayers(GameObject fish)
        {
            FishData fishData = _fishLookup[fish];

            foreach (KeyValuePair<NetworkConnection, NetworkPlayer> kvp in _playerLookup)
            {
                Transform leftHand = kvp.Value.LeftHand.transform;
                Transform rightHand = kvp.Value.RightHand.transform;
                
                float leftHandDistance = Vector3.Distance(fish.transform.position, leftHand.position);
                float rightHandDistance = Vector3.Distance(fish.transform.position, rightHand.position);

                if (leftHandDistance < _playerDistance || rightHandDistance < _playerDistance)
                {
                    Transform avoidTransform = leftHandDistance < rightHandDistance ? leftHand : rightHand;
                    
                    Vector3 direction = avoidTransform.position - fish.transform.position;
                    fish.transform.rotation = Quaternion.Slerp(fish.transform.rotation, Quaternion.LookRotation(direction), _playerAvoidRotationSpeed * Time.deltaTime);
                    fishData.Speed = _playerAvoidSpeed;
                }
            }
        }

        private class FishData
        {
            public float Speed;
            public bool IsTurning;

            public FishData(float speed)
            {
                Speed = speed;
                IsTurning = false;
            }
        }

        public void AddPlayer(NetworkConnection conn, GameObject player)
        {
            NetworkPlayer networkPlayer = player.GetComponentInChildren<NetworkPlayer>();
            if (networkPlayer != null)
            {
                _playerLookup.Add(conn, networkPlayer);
            }
        }

        public void RemovePlayer(NetworkConnection conn)
        {
            _playerLookup.Remove(conn);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(_goal, 0.25f);
            Gizmos.DrawWireSphere(transform.position, _radius);

            foreach (KeyValuePair<GameObject,FishData> kvp in _fishLookup)
            {
                GameObject fish = kvp.Key;
                FishData fishData = kvp.Value;
                
                Gizmos.DrawRay(fish.transform.position, fish.transform.forward);
            }
        }
    }
}