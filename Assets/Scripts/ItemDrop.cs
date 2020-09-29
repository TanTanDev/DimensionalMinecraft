using UnityEngine;

namespace Tantan
{
    public class ItemDrop : MonoBehaviour
    {
        [SerializeField] private Renderer m_renderer;
        [SerializeField] private CameraControllerScriptable m_cameraControllerScriptable;
        [SerializeField] private WorldGridScriptable m_worldScriptable;
        [SerializeField] private float m_gravity;
        [SerializeField] private ReactiveEventItemScriptable m_pickupItemEvent;
        [SerializeField] private ReactiveVector3 m_playerPos;
        [SerializeField] private float m_magnetizeDistance = 1.0f;
        [SerializeField] private float m_pickupDistance = 0.2f;
        [SerializeField] private float m_magnetizedSpeed = 10.0f;
        ItemScriptable m_itemScriptable;

        private Vector2 m_velocity;
        private bool m_magnetizedToPlayer;

        public void Setup(ItemScriptable a_item)
        {
            m_renderer.material.SetTexture("_MainTex", a_item.BlockTexture);
            m_itemScriptable = a_item;
        }

        private void Update()
        {
            if(m_cameraControllerScriptable.CameraController.IsRotating)
                return;
            if(m_magnetizedToPlayer)
                HandleMagnetized();
            else
                handleGravity();
        }

        private void HandleMagnetized()
        {
            Vector3 toPlayer = m_playerPos.Property.Value - transform.position;
            if(toPlayer.magnitude < m_pickupDistance)
            {
                // Hurrah invoke pickup event
                m_pickupItemEvent.OnNext(m_itemScriptable);
                Destroy(this.gameObject);
                return;
            }
            toPlayer.Normalize();
            transform.position += toPlayer * Time.deltaTime * m_magnetizedSpeed;
        }

        private void handleGravity()
        {
            Vector3 currentGridPosVec3 = m_cameraControllerScriptable
                .CameraController
                //.ConvertToVisualPosition(new Vector3(Mathf.Floor(transform.position.x+0.5f), Mathf.Floor(transform.position.y+0.5f), Mathf.Floor(transform.position.z+0.5f)));
                .ConvertToVisualPosition(transform.position);
            Vector2Int currentGridPos = new Vector2Int((int)currentGridPosVec3.x, (int)currentGridPosVec3.y);
            Vector2Int posBelow = currentGridPos;// + Vector2Int.down;
            PhysicsType? belowType = m_worldScriptable.WorldGrid.GetPhysicsTypeAtLocation(posBelow);
            m_velocity.y -= Time.deltaTime * m_gravity;
            if(belowType != null && belowType.Value != PhysicsType.None)
            {
                m_velocity.y = 0.0f;
            }

            Vector3 moveDelta = m_cameraControllerScriptable
                .CameraController
                .ConvertToVisualDelta(new Vector3(m_velocity.x, m_velocity.y, 0f) * Time.deltaTime);
            transform.position += moveDelta;

            // magnetize in range based on visual positions
            Vector3 visualPos = m_cameraControllerScriptable.CameraController.ConvertToVisualPosition(transform.position);
            visualPos.z = 0;
            Vector3 playerVisualPos = m_cameraControllerScriptable.CameraController.ConvertToVisualPosition(m_playerPos.Property.Value);
            playerVisualPos.z = 0;
            if((playerVisualPos - visualPos).magnitude < m_magnetizeDistance)
                m_magnetizedToPlayer = true;
            //if((transform.position - m_playerPos.Property.Value).magnitude < m_magnetizeDistance)
        }
    }
}