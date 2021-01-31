using UnityEngine;

namespace EmeraldActivities
{
    [CreateAssetMenu(menuName = "Emerald Activities/Hidden Object Data")]
    public class HiddenObjectData : ScriptableObject
    {
        [SerializeField]
        private Color _color;
        public Color Color => _color;
    }
}