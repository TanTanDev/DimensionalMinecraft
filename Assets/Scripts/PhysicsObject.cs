using UnityEngine;

namespace Tantan
{
    public class PhysicsObject: MonoBehaviour
    {
        [SerializeField] private PhysicsType m_physicsType;
        [SerializeField] private ItemScripable m_item;
        [SerializeField] private ItemDrop m_itemDropPrefab;
        [SerializeField] private Renderer m_renderer;
        [SerializeField] private string m_brokenessShaderName = "_Brokeness";
        [SerializeField] private float m_breakSpeed = 0.5f;
        [SerializeField] private WorldGridScriptable m_worldGridScriptable;

        public PhysicsType PhysicsType { get{ return m_physicsType; }}
        private float m_brokeness;
        private float m_previousBrokeness;
        private float m_recoverySpeed = 0.2f;

        private int m_brokenessHash;

        private void OnEnable(){
            m_brokenessHash = Shader.PropertyToID(m_brokenessShaderName);
        }

        public void OnHold()
        {
            m_brokeness += Time.deltaTime * m_breakSpeed;
            if(m_brokeness > 1.0f)
            {
                // Spawn item drop
                ItemDrop itemDrop = Instantiate(m_itemDropPrefab, transform.position, Quaternion.identity);
                itemDrop.Setup(m_item.BlockTexture);

                // Update world collision
                m_worldGridScriptable.WorldGrid.RemoveAt(new Vector3Int((int)(transform.position.x+0.5f), (int)(transform.position.y +0.5f), (int)(transform.position.z + 0.5f)));
                Destroy(this.gameObject);
            }
        }


        private void LateUpdate()
        {
            if(m_brokeness != m_previousBrokeness)
                m_renderer.material.SetFloat(m_brokenessHash, m_brokeness);
            m_previousBrokeness = m_brokeness;
        }

        private void Update()
        {
            m_brokeness -= Time.deltaTime * m_recoverySpeed;
            m_brokeness = Mathf.Clamp01(m_brokeness);
        }
    }
}