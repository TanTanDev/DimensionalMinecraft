using UnityEngine;

namespace Tantan
{
    public class PlayerPhysics : MonoBehaviour
    {
        [SerializeField] private WorldGridScriptable m_worldGridScriptable;
        [SerializeField] private CameraController m_cameraController;
        [SerializeField] private float m_gravity = 0.5f;
        [SerializeField] private float m_jumpStrength = 0.9f;
        [SerializeField] private float m_moveSpeed = 0.3f;
        [SerializeField] private Animator m_currentAnimator;
        [SerializeField] private Transform m_modelOffsetTransform;
        private Vector3 m_lastGroundedPosition;
        private Vector2 m_velocity;
        private Vector2Int m_previousStandingOn;
        private bool m_isGrounded = true;
        private float m_playerWidth;

        public void SetAnimator(Animator a_animator)
        {
            m_currentAnimator = a_animator;
        }
       
        private void Update()
        {
            // todo: this is very ugly schmugly
            if(m_cameraController.IsRotating)
            {
                Vector2Int BelowPlayer = GetPlayerGridPosition() - new Vector2Int(0, 1);
                m_previousStandingOn = BelowPlayer;
                return;
            }
            if(transform.position.y <= 0.6f)
                transform.position = m_lastGroundedPosition;
            // Check if standing on block
            if(Input.GetKeyDown(KeyCode.Space) && m_isGrounded)
            {
                m_velocity.y = m_jumpStrength;
                m_isGrounded = false;
            }
                
            if(Input.GetKey(KeyCode.A))
                m_velocity.x = - m_moveSpeed;
            else if(Input.GetKey(KeyCode.D))
                m_velocity.x = m_moveSpeed;
            else
                m_velocity.x = 0f;
                
            if(m_velocity.x != 0f)
            {
                m_currentAnimator.SetBool("IsRunning", true);
                float moveOffset = 0f;
                moveOffset = m_velocity.x < 0f?90f:-90f;
                m_modelOffsetTransform.rotation = Quaternion.Lerp(m_modelOffsetTransform.rotation, Quaternion.Euler(0f, m_cameraController.GetAngle() + moveOffset, 0f), Time.deltaTime * 20.0f);
            }
            else
                m_currentAnimator.SetBool("IsRunning", false);

            m_velocity.y -= Time.deltaTime * m_gravity;
            // Only check collision below if falling
            if(m_velocity.y < 0f)
            {
                Vector2Int BelowPlayer = GetPlayerGridPosition() - new Vector2Int(0, 1);
                PhysicsType? belowType = m_worldGridScriptable.WorldGrid.GetPhysicsTypeAtLocation(BelowPlayer);
                // IS block non-air aka solid
                if(belowType.HasValue && belowType.Value != PhysicsType.None)
                {
                    m_velocity.y = 0f;
                    if(m_previousStandingOn != BelowPlayer)
                    {
                        // Depth change to the new location depth
                        int belowDepth = m_worldGridScriptable.WorldGrid.GetPhysicsDepthAtLocation(BelowPlayer);
                        SetWorldFromDepth(belowDepth);
                        m_lastGroundedPosition = transform.position;
                    }
                    m_previousStandingOn = BelowPlayer;
                    m_isGrounded = true;
                }
            } else {
                // hit head
                Vector2Int abovePlayer = GetPlayerGridPosition() + new Vector2Int(0, 1);
                PhysicsType? aboveType = m_worldGridScriptable.WorldGrid.GetPhysicsTypeAtLocation(abovePlayer);
                if(aboveType.HasValue && aboveType.Value != PhysicsType.None)
                {
                    m_velocity.y = 0f;
                }
            }

            //if(m_velocity.x > 0) {
            //    // get player in grid, and convert delta to grid  
            //    Vector2Int belowPlayer = GetPlayerGridPosition() + GridToTrueGrid(new Vector2Int(1, 0));
            //    PhysicsType? belowType = m_worldGridScriptable.WorldGrid.GetPhysicsTypeAtLocation(belowPlayer);
            //    // Todo, change depth
            //    if(belowType.HasValue && belowType.Value != PhysicsType.None)
            //    {
            //        m_velocity.x = 0f;
            //    }

            //} else {
            //    Vector2Int BelowPlayer = GetPlayerGridPosition() + GridToTrueGrid(new Vector2Int(-1, 0));
            //    PhysicsType? belowType = m_worldGridScriptable.WorldGrid.GetPhysicsTypeAtLocation(BelowPlayer);
            //    // Todo, change depth
            //    if(belowType.HasValue && belowType.Value != PhysicsType.None)
            //    {
            //        m_velocity.x = 0f;
            //    }

            //}

            Vector3 delta = ConvertToVisualPosition(new Vector3(m_velocity.x, m_velocity.y, 0f)* Time.deltaTime);
            Debug.Log("delta:" + delta);
            transform.position += delta;
        }

        private Vector2Int GridToTrueGrid(Vector2Int a_current) {
            switch (m_cameraController.GetRotation())
            {
                case Rotation.R_0:
                {
                    a_current = new Vector2Int(-a_current.x, a_current.y);
                    break;
                }
                case Rotation.R_90:
                {
                    a_current = new Vector2Int(a_current.x, a_current.y);
                    break;
                }
                case Rotation.R_180:
                {
                    a_current = new Vector2Int(a_current.x, a_current.y);
                    break;
                }
                case Rotation.R_270:
                {
                    a_current = new Vector2Int(a_current.x, a_current.y);
                    break;
                }
            }
            return a_current;
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
            var playerPos = GetPlayerGridPosition();
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(new Vector3(playerPos.x, playerPos.y, 0.3f), new Vector3(1f, 1f,1f));
        }

        private Vector3Int GetPlayerWorldVector3Int() {
            return new Vector3Int((int)(transform.position.x+0.5f), (int)(transform.position.y + 0.5f), (int)(transform.position.z+0.5));
        }

        private Vector2Int WorldToGrid(Vector3 world)
        {
            Vector3Int worldPos = new Vector3Int((int)(world.x+0.5f), (int)(world.y + 0.5f), (int)(world.z+0.5));
            switch (m_cameraController.GetRotation())
            {
                case Rotation.R_0:
                    return new Vector2Int(worldPos.x, worldPos.y);
                case Rotation.R_90:
                    //return new Vector2Int(WorldGrid.DEFAULT_WORLD_SIZE - worldPos.z - 1, worldPos.y);
                    return new Vector2Int(worldPos.z, worldPos.y);
                case Rotation.R_180:
                    //return new Vector2Int(WorldGrid.DEFAULT_WORLD_SIZE - worldPos.x - 1, worldPos.y);
                    // todo verify
                    return new Vector2Int(worldPos.x, worldPos.y);
                case Rotation.R_270:
                    return new Vector2Int(worldPos.z, worldPos.y);
            }
            return Vector2Int.zero;
        }

        private Vector2Int GetPlayerGridPosition()
        {
            Vector3Int worldPos = new Vector3Int((int)(transform.position.x+0.5f), (int)(transform.position.y + 0.5f), (int)(transform.position.z+0.5));
            switch (m_cameraController.GetRotation())
            {
                case Rotation.R_0:
                    return new Vector2Int(worldPos.x, worldPos.y);
                case Rotation.R_90:
                    //return new Vector2Int(WorldGrid.DEFAULT_WORLD_SIZE - worldPos.z - 1, worldPos.y);
                    return new Vector2Int(worldPos.z, worldPos.y);
                case Rotation.R_180:
                    //return new Vector2Int(WorldGrid.DEFAULT_WORLD_SIZE - worldPos.x - 1, worldPos.y);
                    // todo verify
                    return new Vector2Int(worldPos.x, worldPos.y);
                case Rotation.R_270:
                    return new Vector2Int(worldPos.z, worldPos.y);
            }
            return Vector2Int.zero;
        }
    }
}
