using UnityEngine;

namespace Tantan {
    class PhysicsObject: MonoBehaviour {
        [SerializeField] private PhysicsType m_physicsType;
        public PhysicsType PhysicsType { get{ return m_physicsType; }}
    }
}