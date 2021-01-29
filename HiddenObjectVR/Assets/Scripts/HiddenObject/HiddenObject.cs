using Mirror;
using UnityEngine;

namespace EmeraldActivities
{
    public class HiddenObject : NetworkBehaviour
    {
        [SerializeField]
        private HiddenObjectData _data;
        public HiddenObjectData Data => _data;
    }
}