using UnityEngine;

namespace Tantan
{
    public class ItemDrop : MonoBehaviour
    {
        [SerializeField] private Renderer m_renderer;
        [SerializeField] private CameraControllerScriptable m_cameraControllerScriptable;
        [SerializeField] private WorldGridScriptable m_worldScriptable;
        [SerializeField] private float m_gravity;
        private Vector2 m_velocity;

        public void Setup(Texture a_texture)
        {
            m_renderer.material.SetTexture("_MainTex", a_texture);
        }

        private void Update()
        {
            if(m_cameraControllerScriptable.CameraController.IsRotating)
                return;

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
        }
    }
}