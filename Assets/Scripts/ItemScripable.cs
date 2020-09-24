using UnityEngine;

namespace Tantan
{
    [CreateAssetMenu(menuName = "Tantan/Item", fileName = "Item_{name}")]
    public class ItemScripable : ScriptableObject
    {
        public Texture m_imageIcon;
        public PhysicsObject m_blockPrefab;   
    }
}
