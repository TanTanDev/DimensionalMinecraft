using UnityEngine;

namespace Tantan
{
    [CreateAssetMenu(menuName = "Tantan/Item", fileName = "Item_{name}")]
    public class ItemScriptable : ScriptableObject
    {
        public string Name;
        public Texture m_imageIcon;
        public Texture BlockTexture;
        public PhysicsObject m_blockPrefab;   
    }
}
