using UnityEngine;

namespace Tantan
{
    public class PhysicsObject: MonoBehaviour
    {
        [SerializeField] private PhysicsType m_physicsType;
        [SerializeField] private Renderer m_renderer;
        public PhysicsType PhysicsType { get{ return m_physicsType; }}

        private void Awake(){
        }

        public void SetRed()
        {
            m_renderer.material.color = Color.red;
        }
    }
}