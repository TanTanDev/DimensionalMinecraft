using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tantan
{
    public class PlayerPhysics : MonoBehaviour
    {
        [SerializeField] private WorldGrid m_worldGrid;
        [SerializeField] private CameraController m_cameraController;
        [SerializeField] private float m_gravity = 0.5f;
        [SerializeField] private float m_jumpStrength = 0.9f;
        [SerializeField] private float m_moveSpeed = 0.3f;
        private Vector2 m_velocity;
        private Vector2Int m_previousStandingOn;
       
        private void Update()
        {
            if(m_cameraController.IsRotating)
                return;
            // Check if standing on block
            if(Input.GetKeyDown(KeyCode.Space))
                m_velocity.y = m_jumpStrength;
                
            if(Input.GetKey(KeyCode.A))
                m_velocity.x = - m_moveSpeed;
            else if(Input.GetKey(KeyCode.D))
                m_velocity.x = m_moveSpeed;
            else
                m_velocity.x = 0f;
                


            m_velocity.y -= Time.deltaTime * m_gravity;
            // Only check collision below if falling
            if(m_velocity.y < 0f)
            {
                Vector2Int BelowPlayer = GetPlayerPosition() - new Vector2Int(0, 1);
                PhysicsType belowType = m_worldGrid.GetPhysicsTypeAtLocation(BelowPlayer);
                // Todo, change depth
                if(belowType != PhysicsType.None)
                {
                    m_velocity.y = 0f;
                    if(m_previousStandingOn != BelowPlayer)
                    {
                        // Depth change to the new location depth
                        int belowDepth = m_worldGrid.GetPhysicsDepthAtLocation(BelowPlayer);
                        SetWorldFromDepth(belowDepth);
                        //transform.position = DepthOffset;
                    }
                    m_previousStandingOn = BelowPlayer;
                }
            }

            Vector3 delta = ConvertToVisualPosition(new Vector3(m_velocity.x, m_velocity.y, 0f)* Time.deltaTime);
            transform.position += delta;
        }

        private void SetWorldFromDepth(int depth)
        {
            switch (m_cameraController.GetRotation())
            {
                case Rotation.R_0:
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, (float)depth); 
                    break;
                }
                case Rotation.R_90:
                {
                    transform.position = new Vector3((float)depth, transform.position.y, transform.position.z); 
                    break;
                }
                case Rotation.R_180:
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, (float)depth); 
                    break;
                }
                case Rotation.R_270:
                {
                    transform.position = new Vector3((float)depth, transform.position.y, transform.position.z); 
                    break;
                }
            }
        }

        private Vector3 ConvertToVisualPosition(Vector3 a_current)
        {
            switch (m_cameraController.GetRotation())
            {
                case Rotation.R_0:
                {
                    a_current = new Vector3(-a_current.x, a_current.y, a_current.z);
                    break;
                }
                case Rotation.R_90:
                {
                    a_current = new Vector3(a_current.z, a_current.y, a_current.x);
                    break;
                }
                case Rotation.R_180:
                {
                    a_current = new Vector3(a_current.x, a_current.y, a_current.z);
                    break;
                }
                case Rotation.R_270:
                {
                    a_current = new Vector3(a_current.z, a_current.y, -a_current.x);
                    break;
                }
            }
            return a_current;
        }

        private void OnDrawGizmos(){
            if(m_cameraController.IsRotating)
                return;
            var playerPos = GetPlayerPosition();
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(new Vector3(playerPos.x, playerPos.y, 0.3f), new Vector3(1f, 1f,1f));
        }

        private Vector2Int GetPlayerPosition()
        {
            Vector3Int worldPos = new Vector3Int((int)(transform.position.x+0.5f), (int)(transform.position.y + 0.5f), (int)(transform.position.z+0.5));
            switch (m_cameraController.GetRotation())
            {
                case Rotation.R_0:
                    return new Vector2Int(worldPos.x, worldPos.y);
                case Rotation.R_90:
                    return new Vector2Int(WorldGrid.DEFAULT_WORLD_SIZE - worldPos.z - 1, worldPos.y);
                case Rotation.R_180:
                    return new Vector2Int(WorldGrid.DEFAULT_WORLD_SIZE - worldPos.x - 1, worldPos.y);
                case Rotation.R_270:
                    return new Vector2Int(worldPos.z, worldPos.y);
            }
            return Vector2Int.zero;
        }
    }
}
